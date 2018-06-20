using DeviceSimulationApp.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceSimulationApp
{
    public static class RegistrationBackgroundWorker
    {
        [FunctionName("RegistrationGenerator")]
        public static async Task<HttpResponseMessage> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient durableOrchestrationClient, TraceWriter log)
        {
            var simulationItems = await req.Content.ReadAsAsync<IEnumerable<SimulationItem>>();

            // Function input comes from the request content.
            string instanceId = await durableOrchestrationClient.StartNewAsync("RegistrationBackgroundWorker", simulationItems);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            return durableOrchestrationClient.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("RegistrationBackgroundWorker")]
        public static async Task RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var simulationItems = context.GetInput<IEnumerable<SimulationItem>>();
            var batchSize = 1000;
            foreach (var simulationItem in simulationItems)
            {
                var numberOfDevices = Enumerable.Range(0, simulationItem.NumberOfDevices);
                for (var batchNumber = 0; batchNumber <= numberOfDevices.Count() / batchSize; batchNumber++)
                {
                    var eventGridEvents = new List<EventGridEvent>(batchSize);
                    var deviceNumbers = numberOfDevices.Take(batchSize).Skip(batchNumber);
                    foreach (var deviceNumber in deviceNumbers)
                    {
                        var deviceId = simulationItem.Id;
                        var deviceItem = new DeviceItem()
                        {
                            Id = deviceId.ToString(),
                            Name = String.Format(CultureInfo.CurrentCulture, simulationItem.DeviceNamePattern, deviceId, deviceNumber),
                            DeviceType = simulationItem.DeviceType,
                            InitialState = simulationItem.InitialState,
                            Interval = simulationItem.DeviceInterval,
                            Properties = simulationItem.Properties,
                        };

                        var eventGridEvent = new EventGridEvent()
                        {
                            Id = Guid.NewGuid().ToString(),
                            EventTime = DateTime.UtcNow,
                            Data = JsonConvert.SerializeObject(deviceItem),
                            EventType = "registrationItem",
                            Subject = "simulation/devices/registration",
                            DataVersion = "1.0",
                        };

                        eventGridEvents.Add(eventGridEvent);
                    }

                    await SendEventGridEvents(eventGridEvents);
                }
            }
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