using System.Net;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public interface IHttpMethodHandler
    {
        void ProcessHttpRequest(HttpListenerContext context);
    }
}