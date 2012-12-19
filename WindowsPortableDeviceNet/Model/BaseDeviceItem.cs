using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model
{
    public class BaseDeviceItem
    {
        public string Id { get; protected set; }
        public ItemCollection DeviceItems { get; private set; }

        public BaseDeviceItem(string id)
        {
            Id = id;
            DeviceItems = new ItemCollection();
        }

        /// <summary>
        /// This method enumerates/cycles through sub objects within this current object.
        /// </summary>
        /// <param name="content"></param>
        protected void LoadDeviceItems(IPortableDeviceContent content)
        {
            // Enumerate the items contained by the current object

            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, Id, null, out objectIds);

            // Cycle through each device item and add it to the device items list.

            uint fetched = 0;
            do
            {
                string objectId;
                objectIds.Next(1, out objectId, ref fetched);

                // Check if anything was retrieved.

                if (fetched > 0)
                {
                    DeviceItems.Add(new Item(objectId, content));
                }
            } while (fetched > 0);
        }
    }
}
