using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    public class ResourceDefaultProperty : BaseWPDProperties
    {
        public ResourceDefaultProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            PositionId = 0;
        }

        public ResourceDefaultProperty()
        {
            FormatId = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            PositionId = 0;
        }
    }
}
