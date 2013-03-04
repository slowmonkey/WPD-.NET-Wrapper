using System;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using WindowsPortableDeviceNet.Model.Properties;
using WindowsPortableDeviceNet.Model.Properties.Device;

namespace WindowsPortableDeviceNet.Model
{
    /// <summary>
    /// This class represents a windows portable device.
    /// </summary>
    public class Device : BaseDeviceItem
    {
        public string DeviceId { get; set; }

        public ContentTypeProperty ContentType { get; private set; }
        public TypeProperty DeviceType { get; private set; }
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
        public Device(string deviceId)
            : base("DEVICE")
        {
            DeviceId = deviceId;
            ComDeviceObject = new PortableDeviceClass();
            Connect();
            PortableDeviceApiLib.IPortableDeviceValues deviceProperties = ExtractDeviceProperties(ComDeviceObject);

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
        public Device Connect()
        {
            if (IsConnected) { return this; }

            var clientInfo = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceValuesClass();
            ComDeviceObject.Open(DeviceId, clientInfo);
            IsConnected = true;

            return this;
        }

        /// <summary>
        /// This method closes the connection to the device.
        /// </summary>
        public Device Disconnect()
        {
            if (!IsConnected) { return this; }
            ComDeviceObject.Close();
            IsConnected = false;
            return this;
        }

        /// <summary>
        /// This method transfers the data on the device to the destination path.
        /// 
        /// TODO: Check if this is still required because there is CopyFromDevice method now.
        /// </summary>
        /// <param name="destinationPath">The desitnation path to transfer the data to (Not on the device!)</param>
        /// <param name="isOverwrite">Whether to override the data if it exists or not.</param>
        public Device TransferData(string destinationPath, bool isOverwrite)
        {
            try
            {
                Connect();
                foreach (Item item in DeviceItems)
                {
                    item.TransferFiles(destinationPath, isOverwrite);
                }
            }
            finally
            {
                Disconnect();
            }

            return this;
        }

        /// <summary>
        /// Reload all the device items.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public Device Refresh(string deviceId)
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
            return this;
        }

        /// <summary>
        /// This method gets the list of properties from the properties list that pertain only to the device.
        /// </summary>
        /// <param name="portableDeviceItem"></param>
        /// <returns></returns>
        private PortableDeviceApiLib.IPortableDeviceValues ExtractDeviceProperties(PortableDeviceClass portableDeviceItem)
        {
            IPortableDeviceContent content;
            IPortableDeviceProperties properties;
            portableDeviceItem.Content(out content);
            content.Properties(out properties);

            // Retrieve the values for the properties

            PortableDeviceApiLib.IPortableDeviceValues propertyValues;
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

        /// <summary>
        /// This method copies the data from the device to a local file.
        /// </summary>
        /// <param name="copyDataDetails"></param>
        /// <returns></returns>
        public Device CopyFromDevice(CopyDataDetails copyDataDetails)
        {
            DeviceItems.CopyFromDevice(copyDataDetails);
            return this;
        }

        /// <summary>
        /// This method copies data onto the device.
        /// </summary>
        /// <param name="copyDataDetails"></param>
        /// <returns></returns>
        public Device CopyToDevice(CopyDataDetails copyDataDetails)
        {
            DeviceTargetStream deviceTargetStream = new DeviceTargetStream(ComDeviceObject, DeviceItems, copyDataDetails);

            System.Runtime.InteropServices.ComTypes.IStream targetStream = null;
            try
            {
                targetStream = (System.Runtime.InteropServices.ComTypes.IStream)deviceTargetStream.WpdTargetStream;
                FileStream sourceStream = new FileStream(copyDataDetails.SourcePath, FileMode.Open, FileAccess.Read);
                unsafe
                {
                    try
                    {
                        var buffer = new byte[deviceTargetStream.OptimalTransferSize];
                        int bytesRead;
                        do
                        {
                            bytesRead = sourceStream.Read(buffer, 0, (int)deviceTargetStream.OptimalTransferSize);
                            IntPtr pcbWritten = IntPtr.Zero;
                            if (bytesRead < (int)deviceTargetStream.OptimalTransferSize)
                            {
                                targetStream.Write(buffer, bytesRead, pcbWritten);
                            }
                            else
                            {
                                targetStream.Write(buffer, (int)deviceTargetStream.OptimalTransferSize, pcbWritten);
                            }
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
                Marshal.ReleaseComObject(deviceTargetStream.WpdTargetStream);
                Marshal.ReleaseComObject(targetStream);

                deviceTargetStream.WpdTargetStream = null;
            }

            return this;
        }

        private static void ValidateParentObjectIdOfDestinationPath(CopyDataDetails copyDataDetails, string parentObjectId)
        {
            if (!String.IsNullOrEmpty(parentObjectId)) return;
            
            string errorMessage = String.Format("Destination folder: {0} - cannot be found.", copyDataDetails.DestinationPath);
            throw new Exception(errorMessage);            
        }
    }
}
