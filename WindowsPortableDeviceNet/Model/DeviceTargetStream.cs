using System.IO;
using PortableDeviceApiLib;
using WindowsPortableDeviceNet.Model.Properties;

namespace WindowsPortableDeviceNet.Model
{
    /// <summary>
    /// This class represents a target stream on the device.
    /// </summary>
    public class DeviceTargetStream
    {
        public PortableDeviceApiLib.IStream WpdTargetStream { get; set; }
        public uint OptimalTransferSize { get; private set; }

        /// <summary>
        /// This contructor obtains the target stream from the copy details and the actual device.
        /// </summary>
        /// <param name="physicalDeviceObject"></param>
        /// <param name="copyDataDetails"></param>
        /// <param name="deviceItems"></param>
        public DeviceTargetStream(
            PortableDeviceClass physicalDeviceObject,            
            ItemCollection deviceItems,
            CopyDataDetails copyDataDetails)
        {
            // Get the destination path's parent object.

            copyDataDetails.DestinationPathParentObject = deviceItems.FindParentObject(copyDataDetails.DestinationPath);
            copyDataDetails.ValidateDestinationPathParentObject();

            PortableDeviceApiLib.IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(copyDataDetails.SourcePath, copyDataDetails.DestinationPath, copyDataDetails.DestinationPathParentObject.Id);
            PortableDeviceApiLib.IStream wpdStream = null;

            uint optimalTransferSize = 0;
            IPortableDeviceContent content;
            physicalDeviceObject.Content(out content);

            // Extract target generic file details into a stream object.

            content.CreateObjectWithPropertiesAndData(
                values,
                out wpdStream,
                ref optimalTransferSize,
                null);

            OptimalTransferSize = optimalTransferSize;
            WpdTargetStream = wpdStream;
        }

        private PortableDeviceApiLib.IPortableDeviceValues GetRequiredPropertiesForContentType(
            string source,
            string destination,
            string parentObjectId)
        {
            PortableDeviceApiLib.IPortableDeviceValues values =
                new PortableDeviceTypesLib.PortableDeviceValues() as PortableDeviceApiLib.IPortableDeviceValues;

            PortableDeviceApiLib._tagpropertykey wpdObjectParentId = new WpdObjectParentIdProperty().ToTagPropertyKey();
            values.SetStringValue(ref wpdObjectParentId, parentObjectId);

            FileInfo fileInfo = new FileInfo(source);
            PortableDeviceApiLib._tagpropertykey wpdObjectSize = new SizeProperty().ToTagPropertyKey();
            values.SetUnsignedLargeIntegerValue(wpdObjectSize, (ulong)fileInfo.Length);

            // TODO: Broken?? Seems maybe it's only copying file to file.
            
            PortableDeviceApiLib._tagpropertykey wpdObjectOriginalFileName = new OriginalFileNameProperty().ToTagPropertyKey();
            values.SetStringValue(wpdObjectOriginalFileName, Path.GetFileName(destination));

            PortableDeviceApiLib._tagpropertykey wpdObjectName = new NameProperty().ToTagPropertyKey();
            values.SetStringValue(wpdObjectName, Path.GetFileName(destination));

            return values;
        }

    }
}
