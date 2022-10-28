using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBMall.Server.Image;

namespace Capsule.Service.ConsoleHost
{
    internal interface ICapsuleInstance
    {
        string Extension { get; }

        List<byte[]> HeadBytes { get; }

        CapsuleInfo CreateInfo();
    }
}
