using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class DOCXInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".docx";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x50, 0x4B, 0x03, 0x04 } };
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
                FileTypeValue = (int)FileTypes.DOCX,
                FileGroup = FileGroups.File,
                FileType = FileTypes.DOCX,
                MagickFormat = MagickFormat.Null,
                ContenType = "application/msword"
            };
        }
    }
}
