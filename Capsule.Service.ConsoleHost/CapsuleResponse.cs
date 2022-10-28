using Microsoft.AspNetCore.Http;

namespace XBMall.Server.Image
{
    internal class CapsuleResponse
    {
        public int StatusCode { get; set; }

        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

        public CapsuleResponseBody Body { get; set; }
    }
}
