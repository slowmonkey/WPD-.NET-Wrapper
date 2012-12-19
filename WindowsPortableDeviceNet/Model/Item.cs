using System;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;
using WindowsPortableDeviceNet.Model.Properties;

namespace WindowsPortableDeviceNet.Model
{
    public class Item : BaseDeviceItem
    {
        public ContentTypeProperty ContentType { get; private set; }
        public NameProperty Name { get; private set; }
        public OriginalFileNameProperty OriginalFileName { get; private set; }

        private IPortableDeviceContent DeviceContent { get; set; }

        public Item(string objectId, IPortableDeviceContent content)
            : base(objectId)
        {
            DeviceContent = content;

            IPortableDeviceProperties properties;
            content.Properties(out properties);

            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            ContentType = new ContentTypeProperty(values);
            Name = new NameProperty(values);

            // Only load the sub information if the current object is a folder or functional object.

            switch (ContentType.Type)
            {
                case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                {
                    LoadDeviceItems(content);
                    break;
                }

                case WindowsPortableDeviceEnumerators.ContentType.Folder:
                {
                    OriginalFileName = new OriginalFileNameProperty(values);
                    LoadDeviceItems(content);
                    break;
                }

                default:
                {
                    OriginalFileName = new OriginalFileNameProperty(values);
                    break;
                }
            }
        }

        public void TransferFiles(CopyDataDetails copyDataDetails)
        {
            if (String.IsNullOrEmpty(copyDataDetails.SourcePath))
                throw new ApplicationException("TransferFiles(): No source path is specified");

            string[] sourcePathItems = copyDataDetails.SourcePath.Split('\\');

            // If there is only one source path item left it means it is either the file to be transfered or the files
            // within that folder are to be transferred.

            bool transferAllItems = false;
            if (sourcePathItems.Length == 1) transferAllItems = true;

            // Find the device item to transfer.

            foreach (Item item in DeviceItems)
            {
                // If the current source path name does not match the device item then go to the next item.

                if (item.Name.ToString() != sourcePathItems[0]) continue;

                if (transferAllItems)
                {
                    item.TransferFiles(copyDataDetails.DestinationPath, copyDataDetails.IsOverwrite, copyDataDetails.IsKeepFolderStructure);
                }
                else
                {
                    // Remove the first source path item from the source string.

                    copyDataDetails.SourcePath = copyDataDetails.SourcePath.Remove(0, sourcePathItems[0].Length + 1);
                    item.TransferFiles(copyDataDetails);
                }
                return;
            }
        }

        public void TransferFiles(string destinationPath, bool isOverwrite, bool isKeepFolderStructure)
        {
            if (!Directory.Exists(destinationPath))
            {
                throw new ApplicationException("TransferFiles() - destinationPath does not exist: " + destinationPath);
            }

            switch (ContentType.Type)
            {
                case WindowsPortableDeviceEnumerators.ContentType.Folder:
                case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                {
                    if (isKeepFolderStructure)
                    {
                        destinationPath = Path.Combine(destinationPath, Name.Value);
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                    }

                    foreach (Item item in DeviceItems)
                    {
                        item.TransferFiles(destinationPath, isOverwrite, isKeepFolderStructure);
                    }
                }
                break;

                default:
                {
                    TransferFile(destinationPath, isOverwrite);
                }
                break;
            }
        }

        /// <summary>
        /// This method copies the file from the device to the destination path.
        /// </summary>
        /// <param name="destinationPath"></param>
        private void TransferFile(string destinationPath, bool isOverwrite)
        {
            IPortableDeviceResources resources;
            DeviceContent.Transfer(out resources);

            IStream wpdStream = null;
            uint optimalTransferSize = 0;

            _tagpropertykey property = new ResourceDefaultProperty().ToTagPropertyKey();

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = null;

            try
            {
                resources.GetStream(Id, ref property, 0, ref optimalTransferSize, out wpdStream);
                sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

                FileMode fileMode = FileMode.Create;
                if (isOverwrite) fileMode = FileMode.CreateNew;
                FileStream targetStream = new FileStream(
                    Path.Combine(destinationPath, OriginalFileName.Value),
                    fileMode,
                    FileAccess.Write);

                unsafe
                {
                    try
                    {
                        var buffer = new byte[1024];
                        int bytesRead;
                        do
                        {
                            sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
                            targetStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead > 0);
                    }
                    finally
                    {
                        targetStream.Close();
                    }
                }
            }
            catch (IOException)
            {
                throw new Exception("Destination file already exist!");
            }
            finally
            {
                Marshal.ReleaseComObject(sourceStream);
                Marshal.ReleaseComObject(wpdStream);
            }
        }

        // TODO: Should be transfer file.
        internal void CopyFromDevice(string source, string destination, bool isOverwrite)
        {
            // Split the source so that it is folder/folder/file. There might be x number of folders.
            // Split the source into directories and file names and traverse through all the folders until the file is reached.

            string[] sourcePathItems = source.Split('\\');

            if (sourcePathItems.Length == 0) return;

            if (sourcePathItems.Length > 1)
            {
                // If the current item is folder and it matches the name of this time copy 
                // all the files from the folder to the destination location.

                if ((ContentType.IsFolder()) && (Name.Value == sourcePathItems[0]))
                {
                    // TODO: Change to use device items copy from device.

                    foreach (Item item in DeviceItems)
                    {
                        item.CopyFromDevice(source.Remove(0, sourcePathItems[0].Length + 1), destination, isOverwrite);
                    }
                }
            }
            else if (sourcePathItems.Length == 1)
            {
                if (Name.Value == sourcePathItems[0])
                {
                    TransferFile(destination, isOverwrite);
                }
            }
        }

        public string FindParentObjectId(string destinationPath)
        {
            string[] destinationPathItems = destinationPath.Split('\\');

            // If the destination path's first value does not match the current 
            // item name it has gone down the wrong path to search for the items.

            if (Name.Value != destinationPathItems[0]) return String.Empty;

            if (destinationPathItems.Length == 1) return Id;

            if ((destinationPathItems.Length > 1) && (ContentType.IsFolder()))
            {
                return DeviceItems.FindParentObjectId(destinationPath.Remove(0, destinationPathItems[0].Length + 1));                    
            }

            return String.Empty;
        }
    }
}
