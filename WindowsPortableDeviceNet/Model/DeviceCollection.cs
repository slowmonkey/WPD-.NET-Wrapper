using System.Collections.Generic;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model
{
    public class DeviceCollection : List<Device>
    {
        /// <summary>
        /// This static method loads a list of connected devices.
        /// </summary>
        /// <returns></returns>
        public void LoadConnectedDevices()
        {
            PortableDeviceManager manager = new PortableDeviceManager();

            manager.RefreshDeviceList();
            uint count = 1;
            manager.GetDevices(null, ref count);

            if (count == 0) return;

            // Call the above again because we now know how many devices there are.

            string[] deviceIds = new string[count];
            manager.GetDevices(ref deviceIds[0], ref count);

            ExtractDeviceInformation(deviceIds);
        }

        public Device GetDevice(string deviceName)
        {
            foreach (Device device in this)
            {
                if (device.FriendlyName.Value == deviceName) return device;
            }

            return null;
        }

        private void ExtractDeviceInformation(string[] deviceIds)
        {
            foreach (string deviceId in deviceIds)
            {
                Add(new Device(deviceId));
            }
        }

    }
}
