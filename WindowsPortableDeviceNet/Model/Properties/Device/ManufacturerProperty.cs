using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties.Device
{
    public class ManufacturerProperty : BaseWPDProperties
    {
        public string Value { get; private set; }
        public Guid FormatId { get; private set; }
        public uint PositionId { get; private set; }

        public ManufacturerProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("26D4979A-E643-4626-9E2B-736DC0C92FDC");
            PositionId = 7;
            Value = GetStringPropertyValue(FormatId, PositionId);
        }
    }
}
