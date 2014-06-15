using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindowsPortableDeviceNet.Test
{
    [TestClass]
    public class WindowsPortableDevicesManagerTestSuite
    {
        [TestMethod]
        public void Constructor_NoDevicesOnInstantiation_NoDevicesVerified()
        {
            // Arrange
            // Act

            var manager = CreateWindowsPortableDeviceAccessor();

            // Assert

            Assert.AreEqual(0, manager.Count);
        }

        private static WindowsPortableDevicesManager CreateWindowsPortableDeviceAccessor()
        {
            return new WindowsPortableDevicesManager();
        }
    }
}
