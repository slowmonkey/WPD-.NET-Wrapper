using System;
namespace WindowsPortableDeviceNet
{
    public class CopyDataDetails
    {
        public string DeviceName { get; set; }
        public string SourcePath{ get; set; }
        public string DestinationPath { get; set; }
        public bool IsOverwrite { get; set; }
        public bool IsKeepFolderStructure { get; set; }

        public CopyDataDetails()
        {
            DeviceName = String.Empty;
            SourcePath = String.Empty;
            DestinationPath = String.Empty;
            IsOverwrite = false;
            IsKeepFolderStructure = true;
        }
    }
}
