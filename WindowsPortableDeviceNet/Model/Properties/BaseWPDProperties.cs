using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    public class BaseWPDProperties
    {
        public IPortableDeviceValues DeviceProperties { get; private set; }

        public Guid FormatId { get; set; }
        public uint PositionId { get; set; }

        public BaseWPDProperties(IPortableDeviceValues deviceProperties)
        {
            DeviceProperties = deviceProperties;
        }

        public string GetStringPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            string propertyValue;
            DeviceProperties.GetStringValue(ref property, out propertyValue);
            return propertyValue;
        }

        public Guid GetGUIDPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            Guid propertyValue;
            DeviceProperties.GetGuidValue(ref property, out propertyValue);
            return propertyValue;
        }

        public uint GetUIntPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            uint propertyValue;
            DeviceProperties.GetUnsignedIntegerValue(ref property, out propertyValue);
            return propertyValue;
        }

        public static _tagpropertykey CreateProperty(Guid formatId, uint positionId)
        {
            return new PortableDeviceApiLib._tagpropertykey()
            {
                fmtid = formatId,
                pid = positionId,
            };
        }
    }
}
