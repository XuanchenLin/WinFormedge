// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;

/// <summary>
/// Represents a web resource request, encapsulating the request URI, headers, and related metadata.
/// </summary>
public sealed class WebResourceRequest
{
    /// <summary>
    /// Gets the URI of the web resource request.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Gets the HTTP request headers associated with the web resource request.
    /// </summary>
    public CoreWebView2HttpRequestHeaders Headers { get; }

    /// <summary>
    /// Gets the request URL without query parameters.
    /// </summary>
    public string RequestUrl
    {
        get
        {
            var original = Uri.OriginalString;
            if (original.Contains('?'))
            {
                return original.Substring(0, original.IndexOf("?"));
            }

            return original;
        }
    }

    /// <summary>
    /// Gets the relative path of the requested resource, excluding the leading slash.
    /// </summary>
    public string RelativePath => $"{Uri?.LocalPath ?? string.Empty}".TrimStart('/');

    /// <summary>
    /// Gets the file name from the relative path of the requested resource.
    /// </summary>
    public string FileName => Path.GetFileName(RelativePath);

    /// <summary>
    /// Gets the file extension from the file name of the requested resource, excluding the leading dot.
    /// </summary>
    public string FileExtension => Path.GetExtension(FileName).TrimStart('.');

    /// <summary>
    /// Gets a value indicating whether the requested resource has a file name.
    /// </summary>
    public bool HasFileName => !string.IsNullOrEmpty(FileName);

    /// <summary>
    /// Gets the underlying <see cref="CoreWebView2WebResourceRequest"/> instance.
    /// </summary>
    public CoreWebView2WebResourceRequest Request { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebResourceRequest"/> class.
    /// </summary>
    /// <param name="request">The underlying web resource request.</param>
    /// <param name="requestSourceKinds">The source kinds of the web resource request.</param>
    /// <param name="webResourceContext">The context of the web resource request.</param>
    internal WebResourceRequest(CoreWebView2WebResourceRequest request, CoreWebView2WebResourceRequestSourceKinds requestSourceKinds, CoreWebView2WebResourceContext webResourceContext)
    {
        Request = request;
        Uri = new Uri(request.Uri);
        Headers = request.Headers;
    }
}
