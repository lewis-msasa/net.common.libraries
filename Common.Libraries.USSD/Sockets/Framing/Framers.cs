using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Sockets.Framing
{
   

    public interface IProtocolFramer
    {
        /// <summary>
        /// Reads a complete message from the socket according to framing rules
        /// </summary>
        Task<byte[]> ReadMessageAsync(Socket socket, CancellationToken cancellationToken);

        /// <summary>
        /// Writes a complete framed message to the socket according to framing rules
        /// </summary>
        Task WriteMessageAsync(Socket socket, byte[] message, CancellationToken cancellationToken);
    }
    public class LengthPrefixedFramer : IProtocolFramer
    {
        public async Task<byte[]> ReadMessageAsync(Socket socket, CancellationToken cancellationToken)
        {
            var lengthBuffer = new byte[4];
            await ReceiveExactAsync(socket, lengthBuffer, 0, 4, cancellationToken);

            int length = BitConverter.ToInt32(lengthBuffer, 0);
            if (length <= 0 || length > 10_000_000)
                throw new InvalidOperationException($"Invalid message length: {length}");

            var buffer = new byte[length];
            await ReceiveExactAsync(socket, buffer, 0, length, cancellationToken);

            return buffer;
        }

        public async Task WriteMessageAsync(Socket socket, byte[] message, CancellationToken cancellationToken)
        {
            var lengthBuffer = BitConverter.GetBytes(message.Length);
            await socket.SendAsync(lengthBuffer, SocketFlags.None, cancellationToken);
            await socket.SendAsync(message, SocketFlags.None, cancellationToken);
        }

        private async Task ReceiveExactAsync(Socket socket, byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            int received = 0;
            while (received < size)
            {
                int read = await socket.ReceiveAsync(buffer.AsMemory(offset + received, size - received), SocketFlags.None, cancellationToken);
                if (read == 0) throw new SocketException((int)SocketError.ConnectionReset);
                received += read;
            }
        }
    }


    public class DelimiterFramer : IProtocolFramer
    {
        private readonly byte[] _delimiter;
        private readonly List<byte> _buffer = new();

        public DelimiterFramer(byte[] delimiter)
        {
            _delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
            if (_delimiter.Length == 0) throw new ArgumentException("Delimiter cannot be empty");
        }

        public async Task<byte[]> ReadMessageAsync(Socket socket, CancellationToken cancellationToken)
        {
            while (true)
            {
                // Check if buffer already contains a message with delimiter
                int index = IndexOf(_buffer, _delimiter);
                if (index >= 0)
                {
                    var message = _buffer.GetRange(0, index);
                    _buffer.RemoveRange(0, index + _delimiter.Length);
                    return message.ToArray();
                }

                var tempBuffer = new byte[1024];
                int bytesRead = await socket.ReceiveAsync(tempBuffer, SocketFlags.None, cancellationToken);
                if (bytesRead == 0) throw new SocketException((int)SocketError.ConnectionReset);

                _buffer.AddRange(tempBuffer.AsSpan(0, bytesRead).ToArray());
            }
        }

        public async Task WriteMessageAsync(Socket socket, byte[] message, CancellationToken cancellationToken)
        {
            await socket.SendAsync(message, SocketFlags.None, cancellationToken);
            await socket.SendAsync(_delimiter, SocketFlags.None, cancellationToken);
        }

        private static int IndexOf(List<byte> buffer, byte[] pattern)
        {
            for (int i = 0; i <= buffer.Count - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (buffer[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return i;
            }
            return -1;
        }
    }


    public class FixedLengthFramer : IProtocolFramer
    {
        private readonly int _fixedLength;

        public FixedLengthFramer(int fixedLength)
        {
            if (fixedLength <= 0) throw new ArgumentOutOfRangeException(nameof(fixedLength));
            _fixedLength = fixedLength;
        }

        public async Task<byte[]> ReadMessageAsync(Socket socket, CancellationToken cancellationToken)
        {
            var buffer = new byte[_fixedLength];
            int received = 0;
            while (received < _fixedLength)
            {
                int read = await socket.ReceiveAsync(buffer.AsMemory(received, _fixedLength - received), SocketFlags.None, cancellationToken);
                if (read == 0) throw new SocketException((int)SocketError.ConnectionReset);
                received += read;
            }
            return buffer;
        }

        public Task WriteMessageAsync(Socket socket, byte[] message, CancellationToken cancellationToken)
        {
            if (message.Length != _fixedLength)
                throw new ArgumentException($"Message length must be exactly {_fixedLength} bytes");

            return socket.SendAsync(message, SocketFlags.None, cancellationToken).AsTask();
        }
    }


    public class NoFraming : IProtocolFramer
    {
        private readonly int _bufferSize;

        public NoFraming(int bufferSize = 1024)
        {
            _bufferSize = bufferSize;
        }

        public async Task<byte[]> ReadMessageAsync(Socket socket, CancellationToken cancellationToken)
        {
            var buffer = new byte[_bufferSize];
            int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
            if (bytesRead == 0) throw new SocketException((int)SocketError.ConnectionReset);

            if (bytesRead == _bufferSize)
                return buffer;
            else
            {
                var result = new byte[bytesRead];
                Array.Copy(buffer, result, bytesRead);
                return result;
            }
        }

        public Task WriteMessageAsync(Socket socket, byte[] message, CancellationToken cancellationToken)
        {
            return socket.SendAsync(message, SocketFlags.None, cancellationToken).AsTask();
        }
    }

}
