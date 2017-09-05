using System.Net;

namespace FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class RequestProcessorParameter
    {
        public HttpListener Listener { get; set; }
        public IHttpRequestHandler RequestHandler { get; set; }

        public RequestProcessorParameter(HttpListener listener, IHttpRequestHandler requestHandler)
        {
            Listener = listener;
            RequestHandler = requestHandler;
        }
    }
}