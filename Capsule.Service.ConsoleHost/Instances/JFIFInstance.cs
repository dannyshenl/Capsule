using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class JFIFInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".jfif";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 } };
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
                FileTypeValue = (int)FileTypes.JFIF,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.JFIF,
                MagickFormat = MagickFormat.Jpg,
                ContenType = "image/jpeg"
            };
        }
    }
}
