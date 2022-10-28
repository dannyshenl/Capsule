using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace XBMall.Server.Image
{

    internal class CapsuleResponseBody
    {
        public CapsuleResponseBody(List<byte[]> segments, long length)
        {
            Segments = segments;
            Length = length;
        }

        public List<byte[]> Segments { get; }

        public long Length { get; }

        public async Task CopyToAsync(PipeWriter destination, CancellationToken cancellationToken)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            foreach (var segment in Segments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Copy(segment, destination);
                await destination.FlushAsync(cancellationToken);
            }
        }

        private static void Copy(byte[] segment, PipeWriter destination)
        {
            var span = destination.GetSpan(segment.Length);
            segment.CopyTo(span);
            destination.Advance(segment.Length);
        }
    }
}