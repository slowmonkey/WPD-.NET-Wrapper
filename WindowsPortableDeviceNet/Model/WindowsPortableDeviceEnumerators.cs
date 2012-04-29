
namespace WindowsPortableDeviceNet.Model
{
    public class WindowsPortableDeviceEnumerators
    {
        public enum ContentType
        {
            Unknown = 0,
            FunctionalObject,
            Folder,
            Image,
            Document,
            Contact,
            ContactGroup,
            Audio,
            Video,
            Television,
            Playlist,
            MixedContentAlbum,
            AudioAlbum,
            ImageAlbum,
            VideoAlbum,
            Memo,
            Email,
            Appointment,
            Task,
            Program,
            GenericFile,
            Calendar,
            GenericMessage,
            NetworkAssociation,
            Certificate,
            WirelessProfile,
            MediaCast,
            Section,
            Unspecified,
            All
        };

        public enum DeviceType
        {
            Unknown = 0,
            Generic,
            Camera,
            MediaPlayer,
            Phone,
            Video,
            PersonalInformationManager,
            AudioRecorder
        };
    }
}
