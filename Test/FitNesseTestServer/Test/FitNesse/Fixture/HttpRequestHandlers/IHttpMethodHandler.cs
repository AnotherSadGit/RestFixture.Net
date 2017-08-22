using System.Net;

namespace FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public interface IHttpMethodHandler
    {
        void ProcessHttpRequest(HttpListenerContext context);
    }
}