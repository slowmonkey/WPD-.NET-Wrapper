using System;
using System.Collections.Generic;

namespace WindowsPortableDeviceNet.Model
{
    public class ItemCollection : List<Item>
    {
        public void CopyFromDevice(CopyDataDetails copyDataDetails)
        {
            if (String.IsNullOrEmpty(copyDataDetails.SourcePath)) throw new ApplicationException("CopyFromDevice(): No source path is specified");

            string[] sourcePathItems = copyDataDetails.SourcePath.Split('\\');
            bool transferAllItems = false;
            if (sourcePathItems.Length == 1) transferAllItems = true;

            foreach (Item item in this)
            {
                if (item.Name.ToString() != sourcePathItems[0]) continue;

                if (transferAllItems)
                {
                    item.TransferFiles(copyDataDetails);
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

        public string FindParentObjectId(string destinationPath)
        {
            string[] destinationPathItems = destinationPath.Split('\\');
            Item item = FindItemByName(destinationPathItems[0]);

            if (destinationPathItems.Length == 1)
            {
                if (item != null) return item.Id;
                return String.Empty;
            }

            if ((destinationPathItems.Length > 1) && item.ContentType.IsFolder())
            {
                if (item == null) return String.Empty;
                    
                return item.FindParentObjectId(destinationPath.Remove(0, destinationPathItems[0].Length + 1));                    
            }

            return String.Empty;
        }

        private Item FindItemByName(string name)
        {
            return this.Find(i => i.Name.Value == name);
        }
    }
}
