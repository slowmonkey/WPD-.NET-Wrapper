using System;
using WindowsPortableDeviceNet.Model;

namespace WindowsPortableDeviceNet
{
    public class Utility
    {
        public DeviceCollection GetConnectedPortableDevices()
        {
            DeviceCollection connectedPortableDevices = new DeviceCollection();
            connectedPortableDevices.LoadConnectedDevices();
            return connectedPortableDevices;
        }

        public void CopyToPC(string deviceName, string source, string destination, bool overwrite)
        {
            //if (copyDataDetails == null) throw new ArgumentNullException("copyDataDetails");
            CopyDataDetails copyDataDetails =
                new CopyDataDetails
                {
                    DeviceName = deviceName,
                    SourcePath = source,
                    DestinationPath = destination,
                    IsOverwrite = overwrite
                };

            Device device = GetConnectedPortableDevices().GetDevice(deviceName);

            ValidateDeviceIsConnected(deviceName, device);

            device
                .Connect()
                .CopyFromDevice(copyDataDetails);
        }

        public void CopyToDevice(string deviceName, string source, string destination, bool overwrite)
        {
            //if (copyDataDetails == null) throw new ArgumentNullException("copyDataDetails");
            CopyDataDetails copyDataDetails = 
                new CopyDataDetails
                    {
                        DeviceName = deviceName,
                        SourcePath = source,
                        DestinationPath = destination,
                        IsOverwrite = overwrite
                    };

            Device device = GetConnectedPortableDevices().GetDevice(deviceName);

            ValidateDeviceIsConnected(deviceName, device);

            device
                .Connect()
                .CopyToDevice(copyDataDetails);
        }

        private static void ValidateDeviceIsConnected(string deviceName, Device device)
        {
            if (device != null) return;

            string errorMessage = String.Format("Target device: \"{0}\" is not connected!", deviceName);
            throw new Exception(errorMessage);
        }

    }
}
