using System.Collections.Generic;
using PortableDeviceApiLib;
using WindowsPortableDeviceNet.Model;

namespace WindowsPortableDeviceNet
{
    public class Utility
    {
        public List<Device> Get()
        {
            List<Device> connectedPortableDevices = new List<Device>();
            PortableDeviceManager manager = new PortableDeviceManager();

            manager.RefreshDeviceList();
            uint count = 1;
            manager.GetDevices(null, ref count);

            if (count == 0) return connectedPortableDevices;

            // Call the above again because we now know how many devices there are.

            string[] deviceIds = new string[count];
            manager.GetDevices(ref deviceIds[0], ref count);

            ExtractDeviceInformation(deviceIds, connectedPortableDevices);
            return connectedPortableDevices;
        }

        private void ExtractDeviceInformation(string[] deviceIds, List<Device> connectedPortableDevices)
        {
            foreach (string deviceId in deviceIds)
            {
                connectedPortableDevices.Add(new Device(deviceId)); 
            }            
        }
    }
}
