using System.Collections.Generic;

namespace DeviceSimulationApp.Models
{
    public class ConnectedDeviceItem
    {
        public ConnectedDeviceItem()
        {

        }

        public ConnectedDeviceItem(DeviceItem deviceItem)
        {
            Id = deviceItem.Id;
            Name = deviceItem.Name;
            DeviceType = deviceItem.DeviceType;
            MessageType = deviceItem.MessageType;
            InitialState = deviceItem.InitialState;
            CurrentState = deviceItem.CurrentState;
            Interval = deviceItem.Interval;
            Properties = deviceItem.Properties;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string DeviceType { get; set; }

        public string MessageType { get; set; }

        public string InitialState { get; set; }

        public string CurrentState { get; set; }

        public int Interval { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public string DeviceConnectionString { get; set; }
    }
}
