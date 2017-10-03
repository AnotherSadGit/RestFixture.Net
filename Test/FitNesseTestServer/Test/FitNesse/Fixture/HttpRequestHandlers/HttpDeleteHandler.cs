using System.Net;
using NLog;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class HttpDeleteHandler : HttpMethodHandlerBase
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public override void ProcessHttpRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            LOG.Debug("Processing DELETE request {0}...", request.Url);

            string localUrl = this.Sanitise(request.Url.LocalPath);
            string id = this.GetId(localUrl);
            string type = this.GetResourceType(localUrl);
            this.EchoHeader(context);

            Resource resource = this.Resources.get(type, id);

            using (HttpListenerResponse response = context.Response)
            {
                if (resource != null)
                {
                    this.Resources.remove(type, id);
                    response.StatusCode = (int)HttpStatusCode.NoContent;    // 204
                    WriteResponseBody(response, "", DEF_CHARSET);
                }
                else
                {
                    NotFound(response);
                }

                LOG.Debug("DELETE response status code: {0}", response.StatusCode);

                LogResponseHeaders(response);
            }
        }
    }
}