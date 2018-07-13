#!/bin/bash

# Usage ./generate.sh -g FunctionDeployToAzure -f jwfdafunapi

# Setup variables...
while [[ "$#" > 0 ]]; do case $1 in
  -g|--resource-group) resourceGroupName="$2"; shift;;
  -f|--function-name) functionName="$2"; shift;
esac; shift; done

echo "Running script using group name: $resourceGroupName and function name: $functionName"

# Rest we can figure out...
functionId=`az functionapp show -g $resourceGroupName -n $functionName --query "id" | tr -d '"'`
topicName=`az eventgrid topic list --query "[0].name" | tr -d '"'`
subscriptionId=`az account show --query "id" |  tr -d '"'`
rbac=`az ad sp create-for-rbac --scopes /subscriptions/$subscriptionId/resourceGroups/$resourceGroupName --query "{ clientId:appId, clientSecret:password, tenantId:tenant }"`
echo "RBAC created, now parsing out the json"

clientId=`./jq-linux64 '.clientId'`
clientSecret=`./jq-linux64 '.clientSecret'`
tenantId=`./jq-linux64 '.tenantId'`
echo "ClientId is $clientId on TenantId $tenantId"

# Get access token from Azure AD
accessToken=$(curl -s --header "accept: application/json" --request POST "https://login.windows.net/$tenantId/oauth2/token" --data-urlencode "resource=https://management.core.windows.net/" --data-urlencode "client_id=$clientId" --data-urlencode "grant_type=client_credentials" --data-urlencode "client_secret=$clientSecret" | jq -r '.access_token')
echo "Fetched access token"

# Management endpoint for Azure Functions...
managementEndpointUri="https://management.azure.com$functionId/functions/admin/masterkey?api-version=2016-08-01"

masterKeyResponse=`curl -s --header "accept: application/json" --header "Authorization: BEARER $accessToken" --request GET $managementEndpointUri`

masterKey=`./jq-linux64 -r '.masterKey'`
echo "Fetched master key value $masterKey"

az eventgrid event-subscription create -g $resourceGroupName -n RegistrationConsumer --topic-name $topicName  --endpoint "https://$functionName.azurewebsites.net/runtime/webhooks/EventGridExtensionConfig?functionName=RegistrationConsumer&code=$masterKey"
az eventgrid event-subscription create -g $resourceGroupName -n SendEventConsumer --topic-name $topicName --endpoint "https://$functionName.azurewebsites.net/runtime/webhooks/EventGridExtensionConfig?functionName=SendEventConsumer&code=$masterKey"

echo "Created subscriptions"
az ad sp delete --id "$clientId"
