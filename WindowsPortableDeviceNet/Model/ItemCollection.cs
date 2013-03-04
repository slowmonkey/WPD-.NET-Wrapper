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

        /// <summary>
        /// This method finds the parent object for the destination path.
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public Item FindParentObject(string destinationPath)
        {
            // Split the destination path into directories etc...

            string[] destinationPathItems = destinationPath.Split('\\');
            Item item = FindItemByName(destinationPathItems[0]);

            // If the destination path is only one level deep return the item.

            if (destinationPathItems.Length == 1)
            {
                if (item != null) return item;
                return null;
            }

            // If the destination path has multiple levels check the drill down the directory path structure.

            if ((destinationPathItems.Length > 1) && item.ContentType.IsFolder())
            {
                if (item == null) return null;
                    
                return item.FindParentObject(destinationPath.Remove(0, destinationPathItems[0].Length + 1));                    
            }

            return null;
        }

        private Item FindItemByName(string name)
        {
            return this.Find(i => i.Name.Value == name);
        }
    }
}
