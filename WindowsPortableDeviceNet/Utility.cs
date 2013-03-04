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

        public void CopyFileToDevice(string deviceName, string sourceFilePath, string destinationDirectoryPath, bool overwrite)
        {
            CopyDataDetails copyDataDetails = 
                new CopyDataDetails
                    {
                        DeviceName = deviceName,
                        SourcePath = sourceFilePath,
                        DestinationPath = destinationDirectoryPath,
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
