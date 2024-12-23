using System;
using System.IO;

namespace SilentHunter.FileFormats.IO;

/// <summary>
/// Wraps a stream into a specific isolated region.
/// </summary>
public class RegionStream : Stream
{
    private bool _disposed;
    private readonly bool _forReading;

    private long _origin;
    private long _count;

    /// <summary>
    /// Initializes a new instance of <see cref="RegionStream" />. The specified stream will be wrapped and access is only allowed from the current stream position and with a fixed <paramref name="length" />.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="length"></param>
    /// <param name="forReading"></param>
    public RegionStream(Stream stream, long length, bool forReading = true)
    {
        BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

        if (length + stream.Position > stream.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        _origin = stream.Position;
        _count = length;

        _forReading = forReading;
    }

    /// <summary>
    /// Moves the stream region to the new start position and length. Note that this method can only be called if the stream is in 'read' mode. A stream in write mode, cannot be moved.
    /// </summary>
    /// <param name="startPosition">The start position in the base stream.</param>
    /// <param name="length">The length of the region in the base stream.</param>
    public void Move(long startPosition, long length)
    {
        if (!_forReading)
        {
            throw new NotSupportedException("The stream does not support reading.");
        }

        if (length + startPosition > BaseStream.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        _origin = startPosition;
        _count = length;

        // Move stream.
        BaseStream.Position = startPosition;
    }

    /// <summary>
    /// Gets the base stream.
    /// </summary>
    public Stream BaseStream
    {
        get;
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
    /// </summary>
    /// <returns>
    /// true if the stream supports reading; otherwise, false.
    /// </returns>
    public override bool CanRead
    {
        get
        {
            return !_disposed && BaseStream.CanRead && _forReading;
        }
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
    /// </summary>
    /// <returns>
    /// true if the stream supports seeking; otherwise, false.
    /// </returns>
    public override bool CanSeek
    {
        get
        {
            return !_disposed && BaseStream.CanSeek;
        }
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
    /// </summary>
    /// <returns>
    /// true if the stream supports writing; otherwise, false.
    /// </returns>
    public override bool CanWrite
    {
        get
        {
            return !_disposed && BaseStream.CanWrite && !_forReading;
        }
    }

    /// <summary>
    /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    public override void Flush()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        BaseStream.Flush();
    }

    /// <summary>
    /// When overridden in a derived class, gets the length in bytes of the stream.
    /// </summary>
    /// <returns>
    /// A long value representing the length of the stream in bytes.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Length
    {
        get
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (_forReading)
            {
                return _count;
            }

            return BaseStream.Length - _origin;
        }
    }

    /// <summary>
    /// When overridden in a derived class, gets or sets the position within the current stream.
    /// </summary>
    /// <returns>
    /// The current position within the stream.
    /// </returns>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Position
    {
        get
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return BaseStream.Position - _origin;
        }
        set
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            Seek(value, SeekOrigin.Begin);
            //				_baseStream.Position = _startPosition + value;
        }
    }

    /// <summary>
    /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
    /// </returns>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source. </param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream. </param>
    /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
    /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length. </exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> is negative. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentException();
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        if (!CanRead)
        {
            throw new NotSupportedException();
        }

        // We are at end of stream?
        if (Position >= Length)
        {
            return 0;
        }

        // If requested bytes exceeds this stream, reduce number of bytes to read from base stream.
        if (Position + count > Length)
        {
            count = (int)(Length - Position);
        }

        return BaseStream.Read(buffer, offset, count);
    }

    /// <summary>
    /// When overridden in a derived class, sets the position within the current stream.
    /// </summary>
    /// <returns>
    /// The new position within the current stream.
    /// </returns>
    /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter. </param>
    /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position. </param>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPosition;
        switch (origin)
        {
            case SeekOrigin.Begin:
                newPosition = _origin + offset;
                if (offset < 0 || newPosition < _origin)
                {
                    throw new IOException("IO.IO_SeekBeforeBegin");
                }

                if (newPosition > _origin + _count)
                {
                    throw new IOException("IO.IO_SeekAfterEnd");
                }

                break;

            case SeekOrigin.Current:
                newPosition = _origin + offset + Position;
                if (newPosition < _origin)
                {
                    throw new IOException("IO.IO_SeekBeforeBegin");
                }

                if (newPosition > _origin + _count)
                {
                    throw new IOException("IO.IO_SeekAfterEnd");
                }

                break;

            case SeekOrigin.End:
                newPosition = _origin + _count - offset;
                if (newPosition < _origin)
                {
                    throw new IOException("IO.IO_SeekBeforeBegin");
                }

                if (offset < 0 || newPosition > _origin + _count)
                {
                    throw new IOException("IO.IO_SeekAfterEnd");
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }

        BaseStream.Seek(newPosition, SeekOrigin.Begin);
        return Position;
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes. </param>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException("The stream region cannot be modified.");
    }

    /// <summary>
    /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream. </param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream. </param>
    /// <param name="count">The number of bytes to be written to the current stream. </param>
    /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
    /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
    /// <exception cref="T:System.ObjectDisposedException"><see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was closed.</exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        if (!CanWrite)
        {
            throw new NotSupportedException();
        }

        // TODO: we can actually exceed the bounds of the region. Add range checks.

        BaseStream.Write(buffer, offset, count);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="RegionStream" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        base.Dispose(disposing);
    }
}