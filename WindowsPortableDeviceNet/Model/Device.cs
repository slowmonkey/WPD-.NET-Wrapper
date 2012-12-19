using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using WindowsPortableDeviceNet.Model.Properties;
using WindowsPortableDeviceNet.Model.Properties.Device;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WindowsPortableDeviceNet.Model
{
    public class Device : BaseDeviceItem
    {
        public string DeviceId { get; set; }

        public ContentTypeProperty ContentType { get; set; }
        public TypeProperty DeviceType { get; set; }
        public FirmwareVersionProperty FirmwareVersion { get; private set; }
        public FriendlyNameProperty FriendlyName { get; private set; }
        public ManufacturerProperty Manufacturer { get; private set; }
        public ModelProperty Model { get; private set; }
        public NameProperty Name { get; private set; }
        public SerialNumberProperty SerialNumber { get; private set; }

        // COM related objects.
        
        private PortableDeviceClass ComDeviceObject { get; set; }
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Construct the device by obtainin the device properties and the sub data.
        /// 
        /// NOTE: 
        /// There is a difference with the device id and the id.
        /// The id indicates the object id within the windows portable device.
        /// The device id indicates the object id within the operating system.
        /// 
        /// The initial id for all windows portable devices is hard coded to "DEVICE"
        /// </summary>
        /// <param name="deviceId"></param>
        public Device(string deviceId) : base("DEVICE")
        {
            DeviceId = deviceId;
            ComDeviceObject = new PortableDeviceClass();
            Connect();
            IPortableDeviceValues deviceProperties = ExtractDeviceProperties(ComDeviceObject);

            ContentType = new ContentTypeProperty(deviceProperties);
            DeviceType = new TypeProperty(deviceProperties);
            FirmwareVersion = new FirmwareVersionProperty(deviceProperties);
            FriendlyName = new FriendlyNameProperty(deviceProperties);
            Manufacturer = new ManufacturerProperty(deviceProperties);
            Model = new ModelProperty(deviceProperties);
            Name = new NameProperty(deviceProperties);
            SerialNumber = new SerialNumberProperty(deviceProperties);

            LoadDeviceData(ComDeviceObject);

            Disconnect();
        }
        
        /// <summary>
        /// This method opens a connection to the device.
        /// </summary>
        public void Connect()
        {
            if (IsConnected) { return; }

            var clientInfo = (IPortableDeviceValues)new PortableDeviceValuesClass();
            ComDeviceObject.Open(DeviceId, clientInfo);
            IsConnected = true;
        }
        
        /// <summary>
        /// This method closes the connection to the device.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected) { return; }
            ComDeviceObject.Close();
            IsConnected = false;
        }

        /// <summary>
        /// This method transfers the data on the device to the destination path.
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <param name="isKeepFolderStructure"></param>
        public void TransferData(string destinationPath, bool isKeepFolderStructure)
        {
            try
            {
                Connect();
                foreach (Item item in DeviceItems)
                {
                    item.TransferFiles(destinationPath, isKeepFolderStructure);
                }
            }
            finally
            {
                Disconnect();
            }
        }

        public void Refresh(string deviceId)
        {
            DeviceId = deviceId;
            DeviceItems.Clear();

            try
            {
                Connect();
                LoadDeviceData(ComDeviceObject);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// This method gets the list of properties from the properties list that pertain only to the device.
        /// </summary>
        /// <param name="portableDeviceItem"></param>
        /// <returns></returns>
        private IPortableDeviceValues ExtractDeviceProperties(PortableDeviceClass portableDeviceItem)
        {
            IPortableDeviceContent content;
            IPortableDeviceProperties properties;
            portableDeviceItem.Content(out content);
            content.Properties(out properties);

            // Retrieve the values for the properties

            IPortableDeviceValues propertyValues;
            properties.GetValues(Id, null, out propertyValues);

            return propertyValues;
        }

        /// <summary>
        /// This method loads the sub folders and files within the device.
        /// NOTE: It only loads the subfolder and file information.
        /// No actual binary is loaded as this could potentially be a very
        /// piece of data in memory.
        /// </summary>
        /// <param name="portableDeviceItem"></param>
        private void LoadDeviceData(PortableDeviceClass portableDeviceItem)
        {
            IPortableDeviceContent content;
            portableDeviceItem.Content(out content);
            LoadDeviceItems(content);
        }

        internal void CopyToPC(string source, string destination, bool overwrite)
        {
            foreach (Item item in DeviceItems)
            {
                item.CopyToPC(source, destination, overwrite);
            }
        }

        internal void CopyToDevice(string source, string destination, bool overwrite)
        {
            IPortableDeviceContent content;
            ComDeviceObject.Content(out content);
            string parentObjectId = null;
            FindParentObjectId(Path.GetDirectoryName(destination), ref parentObjectId);
            if (string.IsNullOrEmpty(parentObjectId))
                throw new Exception("destination folder has not been found.");

            IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(source, destination, parentObjectId);
            PortableDeviceApiLib.IStream wpdStream = null;
            uint optimalTransferSize = 0;
            content.CreateObjectWithPropertiesAndData(
                values,
                out wpdStream,
                ref optimalTransferSize,
                null);
            System.Runtime.InteropServices.ComTypes.IStream targetStream = null;
            try
            {
                targetStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;
                FileStream sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read);
                unsafe
                {
                    try
                    {
                        var buffer = new byte[optimalTransferSize];
                        int bytesRead;
                        do
                        {
                            bytesRead = sourceStream.Read(buffer, 0, (int)optimalTransferSize);
                            IntPtr pcbWritten = IntPtr.Zero;
                            if (bytesRead < (int)optimalTransferSize)
                                targetStream.Write(buffer, bytesRead, pcbWritten);
                            else
                                targetStream.Write(buffer, (int)optimalTransferSize, pcbWritten);
                        } while (bytesRead > 0);
                        targetStream.Commit(0);
                    }
                    catch (ArgumentException e)
                    {
                        throw new Exception("Some argument has problem, remember this API can't overwrite" +
                            "file on the device side! message: " + e.Message); 
                    }
                    finally
                    {
                        sourceStream.Close();
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(wpdStream);
                Marshal.ReleaseComObject(targetStream);
            }
        }


        private void FindParentObjectId(string destinationPath, ref string parentObjectId)
        {
            foreach (Item item in DeviceItems)
            {
                item.FindParentObjectId(destinationPath, ref parentObjectId);
            }
        }

        private IPortableDeviceValues GetRequiredPropertiesForContentType(string source,
            string destination, string parentObjectId)
        {
            IPortableDeviceValues values =
                new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;

            var WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentObjectId);

            FileInfo fileInfo = new FileInfo(source);
            var WPD_OBJECT_SIZE = new _tagpropertykey();
            WPD_OBJECT_SIZE.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;
            values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong)fileInfo.Length);

            var WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(destination));

            var WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(destination));

            return values;
        }
    }
}
