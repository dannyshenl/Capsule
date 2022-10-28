using ImageMagick;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class PDFInstance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".pdf";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x25, 0x50, 0x44, 0x46 } };
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
                FileTypeValue = (int)FileTypes.PDF,
                FileGroup = FileGroups.File,
                FileType = FileTypes.PDF,
                MagickFormat = MagickFormat.Pdf,
                ContenType = "application/pdf"
            };
        }
    }
}
