using System;
using WindowsPortableDeviceNet.Model;
namespace WindowsPortableDeviceNet
{
    public class CopyDataDetails
    {
        public string DeviceName { get; set; }
        public string SourcePath{ get; set; }
        public string DestinationPath { get; set; }
        public bool IsOverwrite { get; set; }
        public bool IsKeepFolderStructure { get; set; }
        public PortableDeviceApiLib.IStream WpdTargetStream { get; set; }

        public Item DestinationPathParentObject{ get; set; }

        public CopyDataDetails()
        {
            DeviceName = String.Empty;
            SourcePath = String.Empty;
            DestinationPath = String.Empty;
            DestinationPathParentObject = null;
            IsOverwrite = false;
            IsKeepFolderStructure = true;
            WpdTargetStream = null;
        }

        public void ValidateDestinationPathParentObject()
        {
            if (!String.IsNullOrEmpty(DestinationPathParentObject.Id)) return;

            string errorMessage = String.Format("Destination folder: {0} - cannot be found.", DestinationPath);
            throw new Exception(errorMessage);
        }

    }
}
