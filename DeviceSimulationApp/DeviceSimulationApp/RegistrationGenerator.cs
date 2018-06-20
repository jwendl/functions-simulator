using DeviceSimulationApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceSimulationApp
{
    public static class RegistrationGenerator
    {
        [FunctionName("RegistrationGenerator")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var simulationItems = JsonConvert.DeserializeObject<IEnumerable<SimulationItem>>(requestBody);

            var batchSize = 1000;
            foreach (var simulationItem in simulationItems)
            {
                var numberOfDevices = Enumerable.Range(0, simulationItem.NumberOfDevices);
                for (var batchNumber = 0; batchNumber <= numberOfDevices.Count() / batchSize; batchNumber++)
                {
                    var eventGridEvents = new List<EventGridEvent>(batchSize);
                    foreach (var deviceNumber in numberOfDevices)
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

            return new AcceptedResult();
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
