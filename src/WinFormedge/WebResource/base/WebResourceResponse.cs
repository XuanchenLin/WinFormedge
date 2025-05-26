// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace WinFormedge.WebResource;

/// <summary>
/// Represents an HTTP web resource response, including status, headers, content type, and body.
/// </summary>
public sealed class WebResourceResponse : IDisposable
{
    /// <summary>
    /// The default content type used when none is specified.
    /// </summary>
    const string DEFAULT_CONTENT_TYPE = "text/plain";

    /// <summary>
    /// Gets or sets the HTTP status code for the response.
    /// </summary>
    public int HttpStatus { get; set; } = StatusCodes.Status200OK;

    /// <summary>
    /// Gets or sets the response content body as a stream.
    /// </summary>
    public Stream? ContentBody { get; set; }

    /// <summary>
    /// Gets or sets the content type of the response.
    /// </summary>
    public string ContentType
    {
        get
        {
            return Headers["Content-Type"]?.ToString() ?? DEFAULT_CONTENT_TYPE;
        }
        set
        {
            Headers["Content-Type"] = value;
        }
    }

    /// <summary>
    /// Gets the collection of HTTP headers for the response.
    /// </summary>
    public NameValueCollection Headers { get; } = new NameValueCollection();

    /// <summary>
    /// Initializes a new instance of the <see cref="WebResourceResponse"/> class.
    /// </summary>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="buff">The content body as a byte array.</param>
    public WebResourceResponse(string? contentType = null, byte[]? buff = null)
    {
        if (!string.IsNullOrEmpty(contentType))
        {
            ContentType = contentType;
        }

        if (buff != null)
        {
            ContentBody = new MemoryStream(buff);
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="WebResourceResponse"/>.
    /// </summary>
    public void Dispose()
    {
        ContentBody?.Close();
        ContentBody?.Dispose();
        ContentBody = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sets the response content using a byte array and optional content type.
    /// </summary>
    /// <param name="buff">The content as a byte array.</param>
    /// <param name="contentType">The content type of the response.</param>
    internal void Content(byte[] buff, string? contentType = null)
    {
        if (!string.IsNullOrEmpty(contentType))
        {
            ContentType = contentType;
        }

        Headers.Set("Content-Type", ContentType);

        if (ContentBody != null)
        {
            ContentBody.Dispose();
            ContentBody = null;
        }

        ContentBody = new MemoryStream(buff);

        HttpStatus = StatusCodes.Status200OK;
    }

    /// <summary>
    /// Sets the response content as JSON using the specified object and serializer options.
    /// </summary>
    /// <param name="data">The object to serialize as JSON.</param>
    /// <param name="jsonSerializerOptions">Optional JSON serializer options.</param>
    internal void JsonContent(object data, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, jsonSerializerOptions));

        Content(bytes, "application/json");
    }

    /// <summary>
    /// Sets the response content as JSON using the specified generic object and serializer options.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="data">The object to serialize as JSON.</param>
    /// <param name="jsonSerializerOptions">Optional JSON serializer options.</param>
    internal void JsonContent<T>(T data, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, jsonSerializerOptions));

        Content(bytes, "application/json");
    }

    /// <summary>
    /// Sets the response content as plain text using UTF-8 encoding.
    /// </summary>
    /// <param name="text">The text content.</param>
    internal void TextContent(string text)
    {
        TextContent(text, Encoding.UTF8);
    }

    /// <summary>
    /// Sets the response content as plain text using the specified encoding.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="encoding">The encoding to use.</param>
    internal void TextContent(string text, Encoding encoding)
    {
        Content(text, "text/plain", encoding);
    }

    /// <summary>
    /// Sets the response content as plain text using UTF-8 encoding.
    /// </summary>
    /// <param name="content">The text content.</param>
    internal void Content(string content)
    {
        Content(Encoding.UTF8.GetBytes(content), "text/plain");
    }

    /// <summary>
    /// Sets the response content using the specified content and content type, with UTF-8 encoding.
    /// </summary>
    /// <param name="content">The content as a string.</param>
    /// <param name="contentType">The content type of the response.</param>
    internal void Content(string content, string contentType)
    {
        Content(Encoding.UTF8.GetBytes(content), contentType);
    }

    /// <summary>
    /// Sets the response content using the specified content, content type, and encoding.
    /// </summary>
    /// <param name="content">The content as a string.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="encoding">The encoding to use.</param>
    internal void Content(string content, string contentType, Encoding encoding)
    {
        Content(encoding.GetBytes(content), contentType);
    }
}
