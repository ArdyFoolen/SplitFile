using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SplitFile
{
    public class FlowStream : Stream
    {
        private Stream Reader { get; }
        private Stream Writer { get; }
        public FlowStream(Stream reader, Stream writer)
        {
            Reader = reader;
            Writer = writer;
        }

        public async Task FlowAsync(long position, long lengthToRead)
        {
            byte[] buffer = new byte[4000];
            int read;
            int length;

            if (Length > position)
            {
                Seek(position, SeekOrigin.Begin);

                int partRead = 0;
                do
                {
                    length = buffer.Length < (int)(lengthToRead - partRead) ? buffer.Length : (int)(lengthToRead - partRead);
                    read = await ReadAsync(buffer, 0, length);
                    partRead += read;

                    await WriteAsync(buffer, 0, read);
                } while (read == length && partRead < lengthToRead);
            }

        }

        #region Stream

        public override bool CanRead => Reader.CanRead;

        public override bool CanSeek => Reader.CanSeek;

        public override bool CanWrite => Writer.CanWrite;

        public override long Length => Reader.Length;

        public override long Position { get => Reader.Position; set => Reader.Position = value; }

        public override void Flush()
        {
            Reader.Flush();
            Writer.Flush();
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Reader.FlushAsync(cancellationToken);
            await Writer.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
            => Reader.Read(buffer, offset, count);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Reader.ReadAsync(buffer, offset, count, cancellationToken);

        public override long Seek(long offset, SeekOrigin origin)
            => Reader.Seek(offset, origin);

        public override void SetLength(long value)
            => Writer.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
            => Writer.Write(buffer, offset, count);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Writer.WriteAsync(buffer, offset, count, cancellationToken);

        #endregion

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Reader.Dispose();
                Writer.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposed = true;
        }

        #endregion
    }
}
