using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsPortableDeviceNet.Model;

namespace WindowsPortableDeviceNet.Test
{
    [TestClass]
    public class UtilityTestSuite
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void Get_GetConnectedDevices_1DeviceDetected()
        {
            MessageBox.Show(
                "Please ensure that a camera is connected.",
                "Test Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Stop);
            Utility utility = new Utility();
            List<Device> device = utility.GetConnectedPortableDevices();

            Assert.AreEqual(1, device.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TransferData_WithNoFolderStructure_OnlyFilesCopied()
        {
            MessageBox.Show(
                "Please ensure that a camera is connected.",
                "Test Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Stop);
            Utility utility = new Utility();
            List<Device> device = utility.GetConnectedPortableDevices();
            device[0].Connect();
            device[0].TransferData("C:\\Users\\Gav\\Desktop\\test", isOverwrite: false);
            device[0].Disconnect();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TransferData_WithFolderStructure_FilesCopiedWithFolderStructure()
        {
            MessageBox.Show(
                "Please ensure that a camera is connected.",
                "Test Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Stop);
            Utility utility = new Utility();
            List<Device> device = utility.GetConnectedPortableDevices();
            device[0].Connect();
            device[0].TransferData("C:\\Users\\Gav\\Desktop\\test", isOverwrite: false);
            device[0].Disconnect();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void CopyToDevice_JustOneFile_FileCopied()
        {
            const string deviceName = "Canon EOS 400D DIGITAL";
            Utility utility = new Utility();
            utility.CopyFileToDevice(deviceName, "C:\\Temp\\test.jpg", "CF\\DCIM", true);
        }
    }
}