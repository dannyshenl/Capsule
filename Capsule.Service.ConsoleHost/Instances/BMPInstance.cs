using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class BMPInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".bmp";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x42, 0x4D } };
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
                FileTypeValue = (int)FileTypes.BMP,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.BMP,
                MagickFormat = MagickFormat.Bmp,
                ContenType = "application/x-bmp"
            };
        }
    }
}
