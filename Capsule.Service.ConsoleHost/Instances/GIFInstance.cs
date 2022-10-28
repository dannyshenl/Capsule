using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class GIFInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".gif";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x47, 0x49, 0x46, 0x38 } };
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
                FileTypeValue = (int)FileTypes.GIF,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.GIF,
                MagickFormat = MagickFormat.Gif,
                ContenType = "image/gif"
            };
        }
    }
}
