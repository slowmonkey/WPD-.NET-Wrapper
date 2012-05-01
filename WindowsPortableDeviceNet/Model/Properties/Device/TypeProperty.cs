using System;
using PortableDeviceApiLib;

namespace WindowsPortableDeviceNet.Model.Properties.Device
{
    /// <summary>
    /// This class extracts the device type property from the windows portable device
    /// </summary>
    public class TypeProperty : BaseWPDProperties
    {
        // These uint values are obtained from the Windows Driver Kit Tools - WpdInfo.Values file.

        private enum WpdDeviceType
        {
            Generic = 0,
            Camera = 1,
            MediaPlayer = 2,
            Phone = 3,
            Video = 4,
            PersonalInformationManager = 5,
            AudioRecorder = 6,
        };

        public WindowsPortableDeviceEnumerators.DeviceType Type { get; private set; }
        public uint Value { get; private set; }

        public TypeProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("26D4979A-E643-4626-9E2B-736DC0C92FDC");
            PositionId = 15;
            Value = GetUIntPropertyValue(FormatId, PositionId);
            ExtrapolateDeviceType(Value);
        }

        private void ExtrapolateDeviceType(uint deviceType)
        {
            switch ((WpdDeviceType)deviceType)
            {
                case WpdDeviceType.Generic:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Generic;
                }
                break;

                case WpdDeviceType.Camera:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Camera;
                }
                break;

                case WpdDeviceType.MediaPlayer:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.MediaPlayer;
                }
                break;

                case WpdDeviceType.Phone:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Phone;
                }
                break;

                case WpdDeviceType.Video:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Video;
                }
                break;

                case WpdDeviceType.PersonalInformationManager:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.PersonalInformationManager;
                }
                break;

                case WpdDeviceType.AudioRecorder:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.AudioRecorder;
                }
                break;

                default:
                {
                    Type = WindowsPortableDeviceEnumerators.DeviceType.Unknown;
                }
                break;
            }
        }
    }
}
