using System.Net;
using System.Net.Http;

namespace RustMapsApi.Tests.Unit;

public sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public TestHttpMessageHandler(HttpStatusCode statusCode, string json)
        : this(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json)
        })
    {
    }

    public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public HttpRequestMessage? LastRequest { get; private set; }

    /// <summary>The raw body of the last request, read before the content is disposed.</summary>
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        if (request.Content is not null)
        {
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        return _responder(request);
    }
}
