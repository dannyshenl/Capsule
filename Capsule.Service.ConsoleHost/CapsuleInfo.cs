﻿using ImageMagick;

namespace XBMall.Server.Image
{
    internal class CapsuleInfo
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string RelativePath { get; set; }

        public string RelativeDirectory { get; set; }

        public string AbsolutelyPath { get; set; }

        public string AbsolutelyDirectory { get; set; }

        public string Extension { get; set; }

        public int FileTypeValue { get; set; }

        public FileGroups FileGroup { get; set; }

        public FileTypes FileType { get; set; }

        public MagickFormat MagickFormat { get; set; }

        public string ContenType { get; set; }
    }
}
