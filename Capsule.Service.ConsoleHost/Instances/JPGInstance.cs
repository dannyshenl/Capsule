using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class JPGInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".jpg";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 } };
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
                FileTypeValue = (int)FileTypes.JPG,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.JPG,
                MagickFormat = MagickFormat.Jpg,
                ContenType = "image/jpg"
            };
        }
    }
}
