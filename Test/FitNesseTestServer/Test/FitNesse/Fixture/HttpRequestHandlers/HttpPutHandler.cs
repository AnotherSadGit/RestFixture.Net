using System.Net;
using NLog;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class HttpPutHandler : HttpMethodHandlerBase
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public override void ProcessHttpRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            LOG.Debug("Processing PUT request {0}...", request.Url);

            string localUrl = this.Sanitise(request.Url.LocalPath);
            string id = this.GetId(localUrl);
            string type = this.GetResourceType(localUrl);
            this.EchoHeader(context);

            string content = ReadRequestBody(request, DEF_CHARSET);
            Resource resource = this.Resources.get(type, id);

            using (HttpListenerResponse response = context.Response)
            {
                if (resource != null)
                {
                    resource.Payload = content;
                    response.StatusCode = (int)HttpStatusCode.OK;    // 200
                    WriteResponseBody(response, "", DEF_CHARSET);
                }
                else
                {
                    NotFound(response);
                }

                LOG.Debug("PUT response status code: {0}", response.StatusCode);

                LogResponseHeaders(response);
            }
        }
    }
}