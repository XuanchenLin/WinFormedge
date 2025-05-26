// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

namespace WinFormedge.WebResource;

/// <summary>
/// Provides constants for HTTP status codes and a method to get the standard reason phrase for each code.
/// </summary>
public static class StatusCodes
{
    /// <summary>
    /// Returns the standard reason phrase for a given HTTP status code.
    /// </summary>
    /// <param name="status">The HTTP status code.</param>
    /// <returns>The reason phrase associated with the status code, or "Unknown Status Code" if not recognized.</returns>
    public static string GetStatusPhrase(int status)
    {
        switch (status)
        {
            case Status100Continue:
                return "Continue";
            case Status101SwitchingProtocols:
                return "Switching Protocols";
            case Status102Processing:
                return "Processing";
            case Status200OK:
                return "OK";
            case Status201Created:
                return "Created";
            case Status202Accepted:
                return "Accepted";
            case Status203NonAuthoritative:
                return "Non-Authoritative Information";
            case Status204NoContent:
                return "No Content";
            case Status205ResetContent:
                return "Reset Content";
            case Status206PartialContent:
                return "Partial Content";
            case Status207MultiStatus:
                return "Multi-Status";
            case Status208AlreadyReported:
                return "Already Reported";
            case Status226IMUsed:
                return "IM Used";
            case Status300MultipleChoices:
                return "Multiple Choices";
            case Status301MovedPermanently:
                return "Moved Permanently";
            case Status302Found:
                return "Found";
            case Status303SeeOther:
                return "See Other";
            case Status304NotModified:
                return "Not Modified";
            case Status305UseProxy:
                return "Use Proxy";
            case Status306SwitchProxy:
                return "Switch Proxy"; // RFC 2616, removed
            case Status307TemporaryRedirect:
                return "Temporary Redirect";
            case Status308PermanentRedirect:
                return "Permanent Redirect";
            case Status400BadRequest:
                return "Bad Request";
            case Status401Unauthorized:
                return "Unauthorized";
            case Status402PaymentRequired:
                return "Payment Required";
            case Status403Forbidden:
                return "Forbidden";
            case Status404NotFound:
                return "Not Found";
            case Status405MethodNotAllowed:
                return "Method Not Allowed";
            case Status406NotAcceptable:
                return "Not Acceptable";
            case Status407ProxyAuthenticationRequired:
                return "Proxy Authentication Required";
            case Status408RequestTimeout:
                return "Request Timeout";
            case Status409Conflict:
                return "Conflict";
            case Status410Gone:
                return "Gone";
            case Status411LengthRequired:
                return "Length Required";
            case Status412PreconditionFailed:
                return "Precondition Failed";
            //case Status413RequestEntityTooLarge:
            //    return "Request Entity Too Large"; // RFC 2616, renamed
            case Status413PayloadTooLarge:
                return "Payload Too Large"; // RFC 7231
            //case Status414RequestUriTooLong:
            //    return "Request URI Too Long"; // RFC 2616, renamed
            case Status414UriTooLong:
                return "URI Too Long"; // RFC 7231
            case Status415UnsupportedMediaType:
                return "Unsupported Media Type";
            //case Status416RequestedRangeNotSatisfiable:
            //    return "Requested Range Not Satisfiable"; // RFC 2616, renamed
            case Status416RangeNotSatisfiable:
                return "Range Not Satisfiable"; // RFC 7233
            case Status417ExpectationFailed:
                return "Expectation Failed";
            case Status418ImATeapot:
                return "I'm a teapot";
            case Status419AuthenticationTimeout:
                return "Authentication Timeout"; // Not defined in any RFC
            case Status421MisdirectedRequest:
                return "Misdirected Request";
            case Status422UnprocessableEntity:
                return "Unprocessable Entity";
            case Status423Locked:
                return "Locked";
            case Status424FailedDependency:
                return "Failed Dependency";
            case Status426UpgradeRequired:
                return "Upgrade Required";
            case Status428PreconditionRequired:
                return "Precondition Required";
            case Status429TooManyRequests:
                return "Too Many Requests";
            case Status431RequestHeaderFieldsTooLarge:
                return "Request Header Fields Too Large";
            case Status451UnavailableForLegalReasons:
                return "Unavailable For Legal Reasons";
            case Status499ClientClosedRequest:
                return "Client Closed Request"; // unofficial status code originally defined by Nginx
            case Status500InternalServerError:
                return "Internal Server Error";
            case Status501NotImplemented:
                return "Not Implemented";
            case Status502BadGateway:
                return "Bad Gateway";
            case Status503ServiceUnavailable:
                return "Service Unavailable";
            case Status504GatewayTimeout:
                return "Gateway Timeout";
            case Status505HttpVersionNotsupported:
                return "HTTP Version Not Supported";
            case Status506VariantAlsoNegotiates:
                return "Variant Also Negotiates";
            case Status507InsufficientStorage:
                return "Insufficient Storage";
            case Status508LoopDetected:
                return "Loop Detected";
            case Status510NotExtended:
                return "Not Extended";
            case Status511NetworkAuthenticationRequired:
                return "Network Authentication Required";
            default:
                return "Unknown Status Code";

        }
    }

    /// <summary>
    /// HTTP status code 100.
    /// </summary>
    public const int Status100Continue = 100;

    /// <summary>
    /// HTTP status code 101.
    /// </summary>
    public const int Status101SwitchingProtocols = 101;

    /// <summary>
    /// HTTP status code 102.
    /// </summary>
    public const int Status102Processing = 102;

    /// <summary>
    /// HTTP status code 200.
    /// </summary>
    public const int Status200OK = 200;

    /// <summary>
    /// HTTP status code 201.
    /// </summary>
    public const int Status201Created = 201;

    /// <summary>
    /// HTTP status code 202.
    /// </summary>
    public const int Status202Accepted = 202;

    /// <summary>
    /// HTTP status code 203.
    /// </summary>
    public const int Status203NonAuthoritative = 203;

    /// <summary>
    /// HTTP status code 204.
    /// </summary>
    public const int Status204NoContent = 204;

    /// <summary>
    /// HTTP status code 205.
    /// </summary>
    public const int Status205ResetContent = 205;

    /// <summary>
    /// HTTP status code 206.
    /// </summary>
    public const int Status206PartialContent = 206;

    /// <summary>
    /// HTTP status code 207.
    /// </summary>
    public const int Status207MultiStatus = 207;

    /// <summary>
    /// HTTP status code 208.
    /// </summary>
    public const int Status208AlreadyReported = 208;

    /// <summary>
    /// HTTP status code 226.
    /// </summary>
    public const int Status226IMUsed = 226;

    /// <summary>
    /// HTTP status code 300.
    /// </summary>
    public const int Status300MultipleChoices = 300;

    /// <summary>
    /// HTTP status code 301.
    /// </summary>
    public const int Status301MovedPermanently = 301;

    /// <summary>
    /// HTTP status code 302.
    /// </summary>
    public const int Status302Found = 302;

    /// <summary>
    /// HTTP status code 303.
    /// </summary>
    public const int Status303SeeOther = 303;

    /// <summary>
    /// HTTP status code 304.
    /// </summary>
    public const int Status304NotModified = 304;

    /// <summary>
    /// HTTP status code 305.
    /// </summary>
    public const int Status305UseProxy = 305;

    /// <summary>
    /// HTTP status code 306.
    /// </summary>
    public const int Status306SwitchProxy = 306; // RFC 2616, removed

    /// <summary>
    /// HTTP status code 307.
    /// </summary>
    public const int Status307TemporaryRedirect = 307;

    /// <summary>
    /// HTTP status code 308.
    /// </summary>
    public const int Status308PermanentRedirect = 308;

    /// <summary>
    /// HTTP status code 400.
    /// </summary>
    public const int Status400BadRequest = 400;

    /// <summary>
    /// HTTP status code 401.
    /// </summary>
    public const int Status401Unauthorized = 401;

    /// <summary>
    /// HTTP status code 402.
    /// </summary>
    public const int Status402PaymentRequired = 402;

    /// <summary>
    /// HTTP status code 403.
    /// </summary>
    public const int Status403Forbidden = 403;

    /// <summary>
    /// HTTP status code 404.
    /// </summary>
    public const int Status404NotFound = 404;

    /// <summary>
    /// HTTP status code 405.
    /// </summary>
    public const int Status405MethodNotAllowed = 405;

    /// <summary>
    /// HTTP status code 406.
    /// </summary>
    public const int Status406NotAcceptable = 406;

    /// <summary>
    /// HTTP status code 407.
    /// </summary>
    public const int Status407ProxyAuthenticationRequired = 407;

    /// <summary>
    /// HTTP status code 408.
    /// </summary>
    public const int Status408RequestTimeout = 408;

    /// <summary>
    /// HTTP status code 409.
    /// </summary>
    public const int Status409Conflict = 409;

    /// <summary>
    /// HTTP status code 410.
    /// </summary>
    public const int Status410Gone = 410;

    /// <summary>
    /// HTTP status code 411.
    /// </summary>
    public const int Status411LengthRequired = 411;

    /// <summary>
    /// HTTP status code 412.
    /// </summary>
    public const int Status412PreconditionFailed = 412;

    /// <summary>
    /// HTTP status code 413.
    /// </summary>
    public const int Status413RequestEntityTooLarge = 413; // RFC 2616, renamed

    /// <summary>
    /// HTTP status code 413.
    /// </summary>
    public const int Status413PayloadTooLarge = 413; // RFC 7231

    /// <summary>
    /// HTTP status code 414.
    /// </summary>
    public const int Status414RequestUriTooLong = 414; // RFC 2616, renamed

    /// <summary>
    /// HTTP status code 414.
    /// </summary>
    public const int Status414UriTooLong = 414; // RFC 7231

    /// <summary>
    /// HTTP status code 415.
    /// </summary>
    public const int Status415UnsupportedMediaType = 415;

    /// <summary>
    /// HTTP status code 416.
    /// </summary>
    public const int Status416RequestedRangeNotSatisfiable = 416; // RFC 2616, renamed

    /// <summary>
    /// HTTP status code 416.
    /// </summary>
    public const int Status416RangeNotSatisfiable = 416; // RFC 7233

    /// <summary>
    /// HTTP status code 417.
    /// </summary>
    public const int Status417ExpectationFailed = 417;

    /// <summary>
    /// HTTP status code 418.
    /// </summary>
    public const int Status418ImATeapot = 418;

    /// <summary>
    /// HTTP status code 419.
    /// </summary>
    public const int Status419AuthenticationTimeout = 419; // Not defined in any RFC

    /// <summary>
    /// HTTP status code 421.
    /// </summary>
    public const int Status421MisdirectedRequest = 421;

    /// <summary>
    /// HTTP status code 422.
    /// </summary>
    public const int Status422UnprocessableEntity = 422;

    /// <summary>
    /// HTTP status code 423.
    /// </summary>
    public const int Status423Locked = 423;

    /// <summary>
    /// HTTP status code 424.
    /// </summary>
    public const int Status424FailedDependency = 424;

    /// <summary>
    /// HTTP status code 426.
    /// </summary>
    public const int Status426UpgradeRequired = 426;

    /// <summary>
    /// HTTP status code 428.
    /// </summary>
    public const int Status428PreconditionRequired = 428;

    /// <summary>
    /// HTTP status code 429.
    /// </summary>
    public const int Status429TooManyRequests = 429;

    /// <summary>
    /// HTTP status code 431.
    /// </summary>
    public const int Status431RequestHeaderFieldsTooLarge = 431;

    /// <summary>
    /// HTTP status code 451.
    /// </summary>
    public const int Status451UnavailableForLegalReasons = 451;

    /// <summary>
    /// HTTP status code 499. This is an unofficial status code originally defined by Nginx and is commonly used
    /// in logs when the client has disconnected.
    /// </summary>
    public const int Status499ClientClosedRequest = 499;

    /// <summary>
    /// HTTP status code 500.
    /// </summary>
    public const int Status500InternalServerError = 500;

    /// <summary>
    /// HTTP status code 501.
    /// </summary>
    public const int Status501NotImplemented = 501;

    /// <summary>
    /// HTTP status code 502.
    /// </summary>
    public const int Status502BadGateway = 502;

    /// <summary>
    /// HTTP status code 503.
    /// </summary>
    public const int Status503ServiceUnavailable = 503;

    /// <summary>
    /// HTTP status code 504.
    /// </summary>
    public const int Status504GatewayTimeout = 504;

    /// <summary>
    /// HTTP status code 505.
    /// </summary>
    public const int Status505HttpVersionNotsupported = 505;

    /// <summary>
    /// HTTP status code 506.
    /// </summary>
    public const int Status506VariantAlsoNegotiates = 506;

    /// <summary>
    /// HTTP status code 507.
    /// </summary>
    public const int Status507InsufficientStorage = 507;

    /// <summary>
    /// HTTP status code 508.
    /// </summary>
    public const int Status508LoopDetected = 508;

    /// <summary>
    /// HTTP status code 510.
    /// </summary>
    public const int Status510NotExtended = 510;

    /// <summary>
    /// HTTP status code 511.
    /// </summary>
    public const int Status511NetworkAuthenticationRequired = 511;
}
