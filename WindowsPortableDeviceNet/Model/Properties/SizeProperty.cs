using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    public class SizeProperty  : BaseWPDProperties
    {
        public SizeProperty()
        {
            FormatId = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            PositionId = 11;
        }

        public SizeProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            PositionId = 11;
        }
    }
}
