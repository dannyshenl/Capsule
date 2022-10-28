using ImageMagick;

namespace Capsule.Service.ConsoleHost.Instances
{
    internal class MP3Instance : ICapsuleInstance
    {
        public string Extension
        {
            get
            {
                return ".mp3";
            }
        }

        private static readonly List<byte[]> _headBytes = new() { new byte[] { 0x49, 0x44, 0x33, 0x03 } };
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
                FileTypeValue = (int)FileTypes.MP3,
                FileGroup = FileGroups.Audio,
                FileType = FileTypes.MP3,
                MagickFormat = MagickFormat.Null,
                ContenType = "audio/mp3"
            };
        }
    }
}
