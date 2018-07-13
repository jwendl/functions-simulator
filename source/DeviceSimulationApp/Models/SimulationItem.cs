using System;
using System.Collections.Generic;

namespace DeviceSimulationApp.Models
{
    public class SimulationItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DeviceNamePattern { get; set; }

        public string DeviceType { get; set; }

        public string MessageType { get; set; }

        public string InitialState { get; set; }

        public int DeviceInterval { get; set; }

        public int NumberOfDevices { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public IEnumerable<string> Configuration { get; set; }
    }
}
