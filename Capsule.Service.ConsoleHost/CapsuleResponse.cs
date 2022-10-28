using Microsoft.AspNetCore.Http;

namespace Capsule.Service.ConsoleHost
{
    internal class CapsuleResponse
    {
        public int StatusCode { get; set; }

        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

        public CapsuleResponseBody Body { get; set; }
    }
}
