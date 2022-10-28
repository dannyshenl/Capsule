using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class ZIPInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".zip";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x55 }, new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 }, new byte[] { 0x50, 0x4B, 0x05, 0x06 }, new byte[] { 0x50, 0x4B, 0x07, 0x08 }, new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 } };

        public List<byte[]> HeadBytes
        {
            get
            {
                return _headBytes;
            }
        }

        public CapsuleInfo CreateInfo()
        {
            return new CapsuleInfo()
            {
                Extension = Extension,
                FileTypeValue = (int)FileTypes.ZIP,
                FileGroup = FileGroups.File,
                FileType = FileTypes.ZIP,
                MagickFormat = MagickFormat.Null,
                ContenType = "application/x-zip-compressed"
            };
        }
    }
}
