using System;
using System.Net;
using System.Text;
using NLog;

namespace FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    class HttpGetHandler : HttpMethodHandlerBase
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public override void ProcessHttpRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            LOG.Debug("Processing GET request {0}...", request.Url);

            using (HttpListenerResponse response = context.Response)
            {
                // The original Java version used HttpServletRequest.getRequestURI, which returns  
                //  only the part of the URL that appears in the HTTP request following the 
                //  protocol, excluding the query string.  
                //  eg HTTP request line: "POST /some/path.html HTTP/1.1"
                //      returned string: "/some/path.html"
                //  See http://grepcode.com/file/repository.springsource.com/javax.servlet/com.springsource.javax.servlet/2.5.0/javax/servlet/http/HttpServletRequest.java#HttpServletRequest.getRequestURI%28%29
                //  for details.  
                //  Note that in both Java HttpServletRequest.getRequestURI and .NET Uri.LocalPath
                //  the returned string starts with a leading forward slash, "/".
                string localUrl = this.Sanitise(request.Url.LocalPath);
                // Redirect code is based on the GET redirect tests for the original Java 
                //  RestFixure. See RestFixtureTests.GetTests.html in the RestFixture livedocs.
                string redirectedLocation = this.GetRedirectedLocation(localUrl);
                if (redirectedLocation != null)
                {
                    LOG.Debug("Redirecting...");

                    response.StatusCode = (int) HttpStatusCode.MovedPermanently;    // 301
                    response.AddHeader("Location", redirectedLocation);
                    response.ContentLength64 = 0;

                    LOG.Debug("GET response status code: {0}", response.StatusCode);
                    LogResponseHeaders(response);
                    return;
                }
                string id = this.GetId(localUrl);
                string type = this.GetResourceType(localUrl);
                string extension = this.GetExtension(localUrl);
                this.EchoHeader(context);
                EchoQString(context);
                try
                {
                    if (id == null)
                    {
                        List(response, type, extension);
                        SetResponseContentType(response, extension, DEF_CHARSET);
                    }
                    else
                    {
                        Resource resource = this.Resources.get(type, id);

                        if (resource == null || resource.Deleted)
                        {
                            NotFound(response);
                        }
                        else
                        {
                            Found(response, resource);
                            SetResponseContentType(response, extension, DEF_CHARSET);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error(ex, "Exception while processing GET request.");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                finally
                {
                    LOG.Debug("GET response status code: {0}", response.StatusCode);

                    LogResponseHeaders(response);
                }
            }
        }

        private void Found(HttpListenerResponse response, Resource resource)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(resource);
            WriteResponseBody(response, sb.ToString(), DEF_CHARSET);
        }

        private void EchoQString(HttpListenerContext context)
        {
            string qstring = context.Request.Url.Query;
            if (!string.IsNullOrWhiteSpace(qstring))
            {
                if (qstring.StartsWith("?"))
                {
                    qstring = qstring.Substring(1);
                }
                context.Response.AddHeader("Query-String", qstring);
            }
        }

        private void SetResponseContentType(HttpListenerResponse response, 
            string extension, string optCharset)
        {
            response.StatusCode = (int)HttpStatusCode.OK;
            string s = "";
            if (!string.IsNullOrWhiteSpace(optCharset))
            {
                s = ";charset=" + optCharset;
            }
            response.AddHeader("Content-Type", "application/" + extension + s);
        }

        private void List(HttpListenerResponse response, string type, string extension)
        {
            if (type.Contains("root-context"))
            {
                List(response, extension);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                string slashremoved = type.Substring(1);
                if ("json".Equals(extension))
                {
                    sb.Append("{ \"" + slashremoved + "\" : ");
                }
                else
                {
                    sb.Append("<" + slashremoved + ">");
                }

                foreach (Resource r in this.Resources.asCollection(type))
                {
                    sb.Append(r.Payload);
                }
                if ("json".Equals(extension))
                {
                    sb.Append("}");
                }
                else
                {
                    sb.Append("</" + slashremoved + ">");
                }

                WriteResponseBody(response, sb.ToString(), DEF_CHARSET);
            }
        }

        private void List(HttpListenerResponse response, string extension)
        {
            StringBuilder sb = new StringBuilder();
            if (extension.ToLower() =="json")
            {
                sb.Append("{ \"root-context\" : ");
            }
            else
            {
                sb.Append("<root-context>");
            }
            
            WriteResponseBody(response, sb.ToString(), DEF_CHARSET);

            foreach (string s in this.Resources.contexts())
            {
                List(response, s, extension);
            }

            sb = new StringBuilder();
            if ("json".Equals(extension))
            {
                sb.Append("}");
            }
            else
            {
                sb.Append("</root-context>");
            }

            WriteResponseBody(response, sb.ToString(), DEF_CHARSET);
        }
    }
}