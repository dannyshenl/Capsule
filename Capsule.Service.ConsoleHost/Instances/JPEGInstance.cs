using ImageMagick;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class JPEGInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".jpeg";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 } };
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
                FileTypeValue = (int)FileTypes.JPEG,
                FileGroup = FileGroups.Image,
                FileType = FileTypes.JPEG,
                MagickFormat = MagickFormat.Jpeg,
                ContenType = "image/jpeg"
            };
        }
    }
}
