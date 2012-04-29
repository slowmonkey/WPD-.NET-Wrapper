using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties.Device
{
    /// <summary>
    /// This class extracts the device type property from the windows portable device
    /// </summary>
    public class TypeProperty : BaseWPDProperties
    {
        private const uint WPD_DEVICE_TYPE_CAMERA = 1;

        public WindowsPortableDeviceEnumerators.DeviceType Type { get; private set; }
        public uint Value { get; private set; }
        public Guid FormatId { get; private set; }
        public uint PositionId { get; private set; }

        public TypeProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("26D4979A-E643-4626-9E2B-736DC0C92FDC");
            PositionId = 15;
            Value = GetUIntPropertyValue(FormatId, PositionId);
            ExtrapolateDeviceType(Value);
        }

        private void ExtrapolateDeviceType(uint deviceType)
        {
            switch (deviceType)
            {
                case WPD_DEVICE_TYPE_CAMERA:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Camera;
                }
                break;

                default:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Unknown;
                }
                break;
            }
        }
    }
}
