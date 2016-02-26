﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;
using WindowsPortableDeviceNet.Model.Properties;

namespace WindowsPortableDeviceNet.Model
{
    public class Item : BaseDeviceItem
    {
        public ContentTypeProperty ContentType { get; set; }
        public NameProperty Name { get; set; }
        public OriginalFileNameProperty OriginalFileName { get; set; }

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

                case WindowsPortableDeviceEnumerators.ContentType.Image:
                {
                    OriginalFileName = new OriginalFileNameProperty(values);
                    break;
                }
            }
        }

        public void TransferFiles(string destinationPath, bool isKeepFolderStructure)
        {
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
                        item.TransferFiles(destinationPath, isKeepFolderStructure);
                    }
                }
                break;

                case WindowsPortableDeviceEnumerators.ContentType.Image:
                {
                    TransferFile(destinationPath);
                }
                break;
            }
        }

        /// <summary>
        /// This method copies the file from the device to the destination path.
        /// </summary>
        /// <param name="destinationPath"></param>
        private void TransferFile(string destinationPath)
        {
            // TODO: Clean this up.

            IPortableDeviceResources resources;
            DeviceContent.Transfer(out resources);

            IStream wpdStream = null;
            uint optimalTransferSize = 0;

            var property = new _tagpropertykey
                               {
                                   fmtid = new Guid("E81E79BE-34F0-41BF-B53F-F1A06AE87842"),
                                   pid = 0
                               };

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = null;
            try
            {
                resources.GetStream(Id, ref property, 0, ref optimalTransferSize, out wpdStream);
                sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

                FileStream targetStream = new FileStream(
                    Path.Combine(destinationPath, OriginalFileName.Value),
                    FileMode.Create,
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
            finally
            {
                Marshal.ReleaseComObject(sourceStream);
                Marshal.ReleaseComObject(wpdStream);
            }
        }

        internal void CopyToPC(string source, string destination, bool overwrite)
        {
            string[] str = source.Split('\\');

            if (str.Length > 1)
            {
                if (ContentType.Type == WindowsPortableDeviceEnumerators.ContentType.Folder ||
                    ContentType.Type == WindowsPortableDeviceEnumerators.ContentType.FunctionalObject)
                {
                            if (Name.Value == str[0])
                            {
                                foreach (Item item in DeviceItems)
                                {
                                    item.CopyToPC(source.Remove(0, str[0].Length + 1), destination, overwrite);
                                }
                            }
                }
            }
            else if (str.Length == 1)
            {
                if (Name.Value == str[0])
                {
                    TransferFile(destination, overwrite);
                }
            }
        }

        private void TransferFile(string destination, bool overwrite)
        {
            IPortableDeviceResources resources;
            DeviceContent.Transfer(out resources);

            IStream wpdStream = null;
            uint optimalTransferSize = 0;

            var property = new _tagpropertykey
            {
                fmtid = new Guid("E81E79BE-34F0-41BF-B53F-F1A06AE87842"),
                pid = 0
            };

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = null;
            try
            {
                resources.GetStream(Id, ref property, 0, ref optimalTransferSize, out wpdStream);
                sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;
                FileMode mode;
                if (overwrite)
                {
                    mode = FileMode.Create;
                }
                else
                {
                    mode = FileMode.CreateNew;
                }
                FileStream targetStream = new FileStream(
                destination,
                mode,
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
                            targetStream.Write(buffer, 0, 1024);
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


        internal void FindParentObjectId(string destinationPath, ref string parentObjectId)
        {
            string[] str = destinationPath.Split('\\');
            if (str.Length > 1)
            {
                if (ContentType.Type == WindowsPortableDeviceEnumerators.ContentType.Folder ||
                    ContentType.Type == WindowsPortableDeviceEnumerators.ContentType.FunctionalObject)
                {
                    if (Name.Value == str[0])
                    {
                        foreach (Item item in DeviceItems)
                        {
                            item.FindParentObjectId(destinationPath.Remove(0, str[0].Length + 1), ref parentObjectId);
                        }
                    }
                }
            }
            else if (str.Length == 1)
            {
                if (Name.Value == str[0])
                {
                    parentObjectId = Id;
                }
            }
        }
    }
}
