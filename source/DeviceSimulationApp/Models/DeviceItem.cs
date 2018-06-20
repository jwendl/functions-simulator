using System.Collections.Generic;

namespace DeviceSimulationApp.Models
{
    public class DeviceItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DeviceType { get; set; }

        public string MessageType { get; set; }

        public string InitialState { get; set; }

        public string CurrentState { get; set; }

        public int Interval { get; set; }

        public Dictionary<string, string> Properties { get; set; }
    }
}
