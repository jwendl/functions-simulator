using DeviceSimulationApp.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientIotHubConnectionStringBuilder = Microsoft.Azure.Devices.Client.IotHubConnectionStringBuilder;
using IotHubConnectionStringBuilder = Microsoft.Azure.Devices.IotHubConnectionStringBuilder;

namespace DeviceSimulationApp
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Default URL for triggering event grid function in the local environment.
    /// http://localhost:7071/runtime/webhooks/EventGridExtensionConfig?functionName=RegistrationConsumer
    /// </remarks>
    public static class RegistrationConsumer
    {
        [FunctionName("RegistrationConsumer")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, TraceWriter log)
        {
            var iotHubConnectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString");
            var deviceItem = JsonConvert.DeserializeObject<DeviceItem>(eventGridEvent.Data.ToString());

            var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            await AddDeviceAsync(registryManager, deviceItem, log);
            await UpdateTwinAsync(registryManager, deviceItem, log);

            var deviceConnectionString = await BuildConnectionStringAsync(registryManager, deviceItem, log);
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            await deviceClient.OpenAsync();

            var connectedDeviceItem = new ConnectedDeviceItem(deviceItem)
            {
                DeviceConnectionString = deviceConnectionString
            };

            var sendEventGridEvent = new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                EventTime = DateTime.UtcNow,
                Data = JsonConvert.SerializeObject(connectedDeviceItem),
                EventType = "sendEvent",
                Subject = "simulation/devices/event",
                DataVersion = "1.0",
            };

            await SendEventGridEvents(new List<EventGridEvent>(1) { sendEventGridEvent });
        }

        private static async Task AddDeviceAsync(RegistryManager registryManager, DeviceItem deviceItem, TraceWriter log)
        {
            var addDevicePolicy = Policy
                .Handle<Exception>()
                .RetryAsync(15, (exception, retryCount, context) =>
                {
                    log.Error($"Error Retrying for Device Item {deviceItem.Name}", exception);
                });

            await addDevicePolicy.ExecuteAsync(
                async () =>
                {
                    var device = await registryManager.GetDeviceAsync(deviceItem.Name);
                    if (device == null)
                    {
                        device = await registryManager.AddDeviceAsync(new Device(deviceItem.Name));
                    }
                });
        }

        private static async Task UpdateTwinAsync(RegistryManager registryManager, DeviceItem deviceItem, TraceWriter log)
        {
            var updateTwinPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(10, (exception, retryCount, context) =>
                {
                    log.Error($"Error Retrying for Device Item {deviceItem.Name}", exception);
                });

            var device = await registryManager.GetDeviceAsync(deviceItem.Name);
            var twin = new Twin()
            {
                Tags = { ["IsSimulated"] = "Y" }
            };

            await updateTwinPolicy.ExecuteAsync(
                async () =>
                {
                    await registryManager.UpdateTwinAsync(device.Id, twin, "*");
                });
        }

        private static async Task<string> BuildConnectionStringAsync(RegistryManager registryManager, DeviceItem deviceItem, TraceWriter log)
        {
            var iotHubConnectionString = IotHubConnectionStringBuilder.Create(Environment.GetEnvironmentVariable("IoTHubConnectionString"));
            var device = await registryManager.GetDeviceAsync(deviceItem.Name);
            var deviceKeyInfo = new DeviceAuthenticationWithRegistrySymmetricKey(deviceItem.Name, device.Authentication.SymmetricKey.PrimaryKey);
            var deviceConnectionString = ClientIotHubConnectionStringBuilder.Create(iotHubConnectionString.HostName, deviceKeyInfo);
            return deviceConnectionString.ToString();
        }

        private static async Task SendEventGridEvents(IList<EventGridEvent> eventGridEvents)
        {
            var topicEndpoint = Environment.GetEnvironmentVariable("TopicEndpoint");
            var topicHostName = new Uri(topicEndpoint).Host;
            var topicKey = Environment.GetEnvironmentVariable("TopicKey");
            var topicCredentials = new TopicCredentials(topicKey);
            var eventGridClient = new EventGridClient(topicCredentials);

            await eventGridClient.PublishEventsAsync(topicHostName, eventGridEvents);
        }
    }
}
