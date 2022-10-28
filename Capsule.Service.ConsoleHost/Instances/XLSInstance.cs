using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class XLSInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".xls";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF } };
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
                FileTypeValue = (int)FileTypes.XLS,
                FileGroup = FileGroups.File,
                FileType = FileTypes.XLS,
                MagickFormat = MagickFormat.Null,
                ContenType = "application/vnd.ms-excel"
            };
        }
    }
}
