using System;
using System.Collections.Generic;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties
{
    /// <summary>
    /// This class extracts the device type property from the windows portable device
    /// </summary>
    public class ContentTypeProperty : BaseWPDProperties
    {
        private static readonly Dictionary<string, WindowsPortableDeviceEnumerators.ContentType> WpdContentTypeGuids =
            new Dictionary<string, WindowsPortableDeviceEnumerators.ContentType>
                {
                    {"99ED0160-17FF-4C44-9D98-1D7A6F941921", WindowsPortableDeviceEnumerators.ContentType.FunctionalObject},
                    {"27E2E392-A111-48E0-AB0C-E17705A05F85", WindowsPortableDeviceEnumerators.ContentType.Folder},
                    {"EF2107D5-A52A-4243-A26B-62D4176D7603", WindowsPortableDeviceEnumerators.ContentType.Image},
                    {"680ADF52-950A-4041-9B41-65E393648155", WindowsPortableDeviceEnumerators.ContentType.Document},
                    {"EABA8313-4525-4707-9F0E-87C6808E9435", WindowsPortableDeviceEnumerators.ContentType.Contact},
                    {"346B8932-4C36-40D8-9415-1828291F9DE9", WindowsPortableDeviceEnumerators.ContentType.ContactGroup},
                    {"4AD2C85E-5E2D-45E5-8864-4F229E3C6CF0", WindowsPortableDeviceEnumerators.ContentType.Audio},
                    {"9261B03C-3D78-4519-85E3-02C5E1F50BB9", WindowsPortableDeviceEnumerators.ContentType.Video},
                    {"60A169CF-F2AE-4E21-9375-9677F11C1C6E", WindowsPortableDeviceEnumerators.ContentType.Television},
                    {"1A33F7E4-AF13-48F5-994E-77369DFE04A3", WindowsPortableDeviceEnumerators.ContentType.Playlist},
                    {"00F0C3AC-A593-49AC-9219-24ABCA5A2563", WindowsPortableDeviceEnumerators.ContentType.MixedContentAlbum},
                    {"AA18737E-5009-48FA-AE21-85F24383B4E6", WindowsPortableDeviceEnumerators.ContentType.AudioAlbum},
                    {"75793148-15F5-4A30-A813-54ED8A37E226", WindowsPortableDeviceEnumerators.ContentType.ImageAlbum},
                    {"012B0DB7-D4C1-45D6-B081-94B87779614F", WindowsPortableDeviceEnumerators.ContentType.VideoAlbum},
                    {"9CD20ECF-3B50-414F-A641-E473FFE45751", WindowsPortableDeviceEnumerators.ContentType.Memo},
                    {"8038044A-7E51-4F8F-883D-1D0623D14533", WindowsPortableDeviceEnumerators.ContentType.Email},
                    {"0FED060E-8793-4B1E-90C9-48AC389AC631", WindowsPortableDeviceEnumerators.ContentType.Appointment},
                    {"63252F2C-887F-4CB6-B1AC-D29855DCEF6C", WindowsPortableDeviceEnumerators.ContentType.Task},
                    {"D269F96A-247C-4BFF-98FB-97F3C49220E6", WindowsPortableDeviceEnumerators.ContentType.Program},
                    {"0085E0A6-8D34-45D7-BC5C-447E59C73D48", WindowsPortableDeviceEnumerators.ContentType.GenericFile},
                    {"A1FD5967-6023-49A0-9DF1-F8060BE751B0", WindowsPortableDeviceEnumerators.ContentType.Calendar},
                    {"E80EAAF8-B2DB-4133-B67E-1BEF4B4A6E5F", WindowsPortableDeviceEnumerators.ContentType.GenericMessage},
                    {"031DA7EE-18C8-4205-847E-89A11261D0F3", WindowsPortableDeviceEnumerators.ContentType.NetworkAssociation},
                    {"DC3876E8-A948-4060-9050-CBD77E8A3D87", WindowsPortableDeviceEnumerators.ContentType.Certificate},
                    {"0BAC070A-9F5F-4DA4-A8F6-3DE44D68FD6C", WindowsPortableDeviceEnumerators.ContentType.WirelessProfile},
                    {"5E88B3CC-3E65-4E62-BFFF-229495253AB0", WindowsPortableDeviceEnumerators.ContentType.MediaCast},
                    {"821089F5-1D91-4DC9-BE3C-BBB1B35B18CE", WindowsPortableDeviceEnumerators.ContentType.Section},
                    {"28D8D31E-249C-454E-AABC-34883168E634", WindowsPortableDeviceEnumerators.ContentType.Unspecified},
                    {"80E170D2-1055-4A3E-B952-82CC4F8A8689", WindowsPortableDeviceEnumerators.ContentType.All}
                };

        public WindowsPortableDeviceEnumerators.ContentType Type { get; private set; }
        public string Value { get; private set; }

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
            if (WpdContentTypeGuids.ContainsKey(deviceType.ToUpper()))
            {
                Type = WpdContentTypeGuids[deviceType.ToUpper()];
            }
            else
            {
                Type = WindowsPortableDeviceEnumerators.ContentType.Unknown;
            }
        }
    }
}
