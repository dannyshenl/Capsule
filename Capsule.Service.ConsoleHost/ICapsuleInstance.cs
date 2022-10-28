namespace Capsule.Service.ConsoleHost
{
    internal interface ICapsuleInstance
    {
        string Extension { get; }

        List<byte[]> HeadBytes { get; }

        CapsuleInfo CreateInfo();
    }
}
