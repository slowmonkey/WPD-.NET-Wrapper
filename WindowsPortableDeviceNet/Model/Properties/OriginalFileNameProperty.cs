using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    public class OriginalFileNameProperty : BaseWPDProperties
    {
        public string Value { get; private set; }

        public OriginalFileNameProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");
            PositionId = 12;
            Value = GetStringPropertyValue(FormatId, PositionId);
        }
    }
}
