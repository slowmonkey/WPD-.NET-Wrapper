using System.Collections.Generic;
using PortableDeviceApiLib;
using WindowsPortableDeviceNet.Model;
using System;

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


        public void CopyToPC(string deviceName, string source, string destination, bool overwrite)
        {
            FindTargetDevice(deviceName);
            _Device.Connect();
            _Device.CopyToPC(source, destination, overwrite);
        }

        public void CopyToDevice(string device, string source, string destination, bool overwrite)
        {
            FindTargetDevice(device);
            _Device.Connect();
            _Device.CopyToDevice(source, destination, overwrite);
        }

        private void FindTargetDevice(string deviceName)
        {
            List<Device> devices = this.Get();
            foreach (Device device in devices)
            {
                if (device.FriendlyName.Value == deviceName)
                {
                    _Device = device;
                }
            }
            if (_Device == null)
            {
                throw new Exception("target device not connected!");
            }
        }

        private Device _Device;
    }
}
