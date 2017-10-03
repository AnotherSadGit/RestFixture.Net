using System.Collections.Generic;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    /// <summary>
    /// A handler of an HTTP request which will generate an appropriate HTTP response.
    /// </summary>
    public interface IHttpRequestHandler : IHttpMethodHandler
    {
        IDictionary<string, IHttpMethodHandler> MethodHandlers { get; set; }
    }
}