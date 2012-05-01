using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties.Device
{
    public class FriendlyNameProperty : BaseWPDProperties
    {
        public string Value { get; private set; }

        public FriendlyNameProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("26D4979A-E643-4626-9E2B-736DC0C92FDC");
            PositionId = 12;
            Value = GetStringPropertyValue(FormatId, PositionId);
        }
    }
}
