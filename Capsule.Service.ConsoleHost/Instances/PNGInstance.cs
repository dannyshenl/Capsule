using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class PNGInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".png";
            }
        }

        private static readonly List<byte[]> headBytes = new() { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } };
        public List<byte[]> HeadBytes
        {
            get
            {
                return headBytes;
            }
        }

        public CapsuleInfo CreateInfo()
        {
            return new CapsuleInfo()
            {
                Extension = Extension,
                FileTypeValue = (int)FileTypes.PNG,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.PNG,
                MagickFormat = MagickFormat.Png,
                ContenType = "image/png"
            };
        }
    }
}
