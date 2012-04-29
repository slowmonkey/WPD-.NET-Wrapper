using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    /// <summary>
    /// This class extracts the device type property from the windows portable device
    /// </summary>
    public class ContentTypeProperty : BaseWPDProperties
    {
        private const string WPD_CONTENT_TYPE_FOLDER_OBJECT = "27E2E392-A111-48E0-AB0C-E17705A05F85";
        private const string WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT = "99ED0160-17FF-4C44-9D98-1D7A6F941921";
        private const string WPD_CONTENT_TYPE_IMAGE = "EF2107D5-A52A-4243-A26B-62D4176D7603";

        public WindowsPortableDeviceEnumerators.ContentType Type { get; private set; }
        public string Value { get; private set; }
        public Guid FormatId { get; private set; }
        public uint PositionId { get; private set; }

        public ContentTypeProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");
            PositionId = 7;
            Value = GetGUIDPropertyValue(FormatId, PositionId).ToString();
            ExtrapolateDeviceType(Value);
        }


        private void ExtrapolateDeviceType(string deviceType)
        {
            switch (deviceType.ToUpper())
            {
                case WPD_CONTENT_TYPE_FOLDER_OBJECT:
                {
                    Type = WindowsPortableDeviceEnumerators.ContentType.Folder;
                }
                break;

                case WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT:
                {
                    Type = WindowsPortableDeviceEnumerators.ContentType.FunctionalObject;
                }
                break;

                case WPD_CONTENT_TYPE_IMAGE:
                {
                    Type = WindowsPortableDeviceEnumerators.ContentType.Image;
                }
                break;

                default:
                {
                    Type = WindowsPortableDeviceEnumerators.ContentType.Unknown;
                }
                break;
            }
        }
    }
}
