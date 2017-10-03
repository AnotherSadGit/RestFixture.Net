using System;
using System.IO;
using System.Net;
using System.Text;
using NLog;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public abstract class HttpMethodHandlerBase : IHttpMethodHandler
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public const string CONTEXT_ROOT = "/resources";
        protected const string DEF_CHARSET = "ISO-8859-1";
        private readonly Resources _resources = Resources.Instance;
        
        public abstract void ProcessHttpRequest(HttpListenerContext context);

        protected Resources Resources
        {
            get { return _resources; }
        }

        protected string Sanitise(string localUrl)
        {
            if (localUrl.EndsWith("/", StringComparison.Ordinal))
            {
                localUrl = localUrl.Substring(0, localUrl.Length - 1);
            }
            LOG.Debug("Sanitised local URL: {0}", LogHelper.GetDisplayText(localUrl));
            return localUrl;
        }

        protected string GetRedirectedLocation(string localUrl)
        {
            string urlToLower = localUrl.ToLower();
            string redirectUrlSegment = "/redirect";
            int pos = urlToLower.IndexOf(redirectUrlSegment);
            if (pos <= -1)
            {
                return null;
            }

            int length = redirectUrlSegment.Length;
            string redirectedLocation = localUrl.Substring(0, pos) 
                + localUrl.Substring(pos + length);
            return redirectedLocation;
        }

        protected string GetId(string localUrl)
        {
            if (localUrl.Length <= 1)
            {
                LOG.Debug("ID retrieved from local URL: {0}", LogHelper.GetDisplayText(null));
                return null;
            }
            int pos = localUrl.Substring(1).LastIndexOf("/", StringComparison.Ordinal);
            string sId = null;
            if (pos > 0)
            {
                sId = localUrl.Substring(pos + 2);
            }
            if (sId != null)
            {
                int pos2 = sId.LastIndexOf('.');
                if (pos2 >= 0)
                {
                    sId = sId.Substring(0, pos2);
                }

                if (string.IsNullOrWhiteSpace(sId))
                {
                    return null;
                }
            }
            LOG.Debug("ID retrieved from local URL: {0}", LogHelper.GetDisplayText(sId));
            return sId;
        }

        protected string GetResourceType(string localUrl)
        {
            string ret = localUrl;

            if (localUrl.Length <= 1)
            {
                ret = "/root-context";
                LOG.Debug("Resource Type retrieved from local URL: {0}", LogHelper.GetDisplayText(ret));
                return ret;
            }
            int pos = localUrl.Substring(1).IndexOf('/');
            if (pos >= 0)
            {
                ret = localUrl.Substring(0, pos + 1);
            }
            LOG.Debug("Resource Type retrieved from local URL: {0}", LogHelper.GetDisplayText(ret));
            return ret;
        }

        protected string GetExtension(string localUrl)
        {
            string ret = null;
            int extensionPoint = localUrl.LastIndexOf(".", StringComparison.Ordinal);
            if (extensionPoint != -1)
            {
                ret = localUrl.Substring(extensionPoint + 1);
            }
            else
            {
                ret = "xml";
            }
            LOG.Debug("Extension retrieved from local URL: {0}", LogHelper.GetDisplayText(ret));
            return ret;
        }

        protected void NotFound(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound; // 404
            WriteResponseBody(response, "", DEF_CHARSET);
        }

        protected void EchoHeader(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            string headerName = "Echo-Header";
            string echoHeader = request.Headers.Get(headerName);
            if (!string.IsNullOrWhiteSpace(echoHeader))
            {
                HttpListenerResponse response = context.Response;
                response.Headers[headerName] = echoHeader;
            }
        }

        protected string ReadRequestBody(HttpListenerRequest request, string charset)
        {
            Encoding encoding = null;
            try
            {
                encoding = Encoding.GetEncoding(charset);
            }
            catch
            {
                encoding = Encoding.GetEncoding(DEF_CHARSET);
            }

            using (StreamReader reader = new StreamReader(request.InputStream, encoding))
            {
                string body = reader.ReadToEnd();
                return body;
            }
        }

        /// <summary>
        /// Writes the body text to the response output stream.
        /// </summary>
        /// <param name="response">Object representing the response that will be sent.</param>
        /// <param name="body">Text that will be written to the body of the response.</param>
        /// <param name="charset">Character set for encoding the body text.</param>
        /// <remarks>WARNING: The response will be sent immediately the write to the output 
        /// stream is complete.  Therefore all headers, etc, must be set before calling this 
        /// method to write the response body.</remarks>
        protected void WriteResponseBody(HttpListenerResponse response, string body,
            string charset)
        {
            Encoding encoding = null;
            try
            {
                encoding = Encoding.GetEncoding(charset);
            }
            catch
            {
                encoding = Encoding.GetEncoding(DEF_CHARSET);
            }

            byte[] buffer = encoding.GetBytes(body);
            response.ContentLength64 = buffer.Length;
            using (Stream responseStream = response.OutputStream)
            {
                responseStream.Write(buffer, 0, buffer.Length);
            }
        }

        protected void LogResponseHeaders(HttpListenerResponse response)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in response.Headers.AllKeys)
            {
                string[] values = response.Headers.GetValues(key);
                string headerValuesText = null;
                switch (values.Length)
                {
                    case 0:
                        headerValuesText = "[EMPTY]";
                        break;
                    case 1:
                        headerValuesText = values[0];
                        break;
                    default:
                        headerValuesText = "[MULTIPLE VALUES]";
                        break;
                }

                sb.AppendLine(string.Format("    {0}: {1}", key, headerValuesText));

                if (values.Length > 1)
                {
                    foreach (string value in values)
                    {
                        sb.AppendLine(string.Format("        {0}", value));
                    }
                }
            }
            string headerText = sb.ToString();
            if (string.IsNullOrWhiteSpace(headerText))
            {
                LOG.Debug("GET response headers: [NONE]");
            }
            else
            {
                LOG.Debug("GET response headers:");
                LOG.Debug(headerText);
            }
        }
    }
}