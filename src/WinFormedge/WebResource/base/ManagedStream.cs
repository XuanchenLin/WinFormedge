// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;
// https://learn.microsoft.com/zh-cn/microsoft-edge/webview2/concepts/working-with-local-content?tabs=dotnetcsharp
/// <summary>
/// A wrapper stream that manages the lifetime of an underlying stream.
/// Disposes the underlying stream when the end is reached or on read error.
/// </summary>
class ManagedStream : Stream
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedStream"/> class.
    /// </summary>
    /// <param name="s">The underlying stream to wrap.</param>
    public ManagedStream(Stream s)
    {
        _stream = s;
    }

    /// <inheritdoc/>
    public override bool CanRead => _stream.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => _stream.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => _stream.CanWrite;

    /// <inheritdoc/>
    public override long Length => _stream.Length;

    /// <inheritdoc/>
    public override long Position { get => _stream.Position; set => _stream.Position = value; }

    /// <summary>
    /// Not implemented. Throws <see cref="NotImplementedException"/>.
    /// </summary>
    public override void Flush()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    /// <summary>
    /// Not implemented. Throws <see cref="NotImplementedException"/>.
    /// </summary>
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads a sequence of bytes from the underlying stream and advances the position.
    /// Disposes the underlying stream if the end is reached or an exception occurs.
    /// </summary>
    /// <param name="buffer">The buffer to read the data into.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <returns>The total number of bytes read into the buffer.</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = 0;
        try
        {
            read = _stream.Read(buffer, offset, count);
            if (read == 0)
            {
                _stream.Dispose();
            }
        }
        catch
        {
            _stream.Dispose();
            throw;
        }
        return read;
    }

    /// <summary>
    /// Not implemented. Throws <see cref="NotImplementedException"/>.
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The underlying stream being managed.
    /// </summary>
    private Stream _stream;
}
