using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class DOCInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".doc";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } };
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
                FileTypeValue = (int)FileTypes.DOC,
                FileGroup = FileGroups.File,
                FileType = FileTypes.DOC,
                MagickFormat = MagickFormat.Null,
                ContenType = "application/msword"
            };
        }
    }
}
