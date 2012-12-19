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

        public BaseWPDProperties()
        {
            DeviceProperties = null;
        }

        public void AddTo(IPortableDeviceValues values, string parentObjectId)
        {
            if (values == null) throw new ArgumentNullException("values");

            _tagpropertykey wpdObjectParentIdProperty = ToTagPropertyKey();
            values.SetStringValue(ref wpdObjectParentIdProperty, parentObjectId);
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

        public _tagpropertykey ToTagPropertyKey()
        {
            return CreateProperty(FormatId, PositionId);
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
