using WindowsPortableDeviceNet.Model;

namespace WindowsPortableDeviceNet
{
    public class WindowsPortableDevicesManager : DeviceCollection
    {
        //private WpdWrapper WpdWrapper { get; set; }
        //public DeviceCollection DeviceCollection { get; private set; }

        public WindowsPortableDevicesManager()
        {
            //WpdWrapper = new WpdWrapper();
            //DeviceCollection = new DeviceCollection();
            LoadConnectedDevices();
        }

        public void ReloadConnectedDevices()
        {
            //DeviceCollection.Reload();
        }

    }
}
