using System;
using System.Net;
using System.Text;
using NLog;

namespace FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class HttpGetHandler : HttpMethodHandlerBase
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

                    response.StatusCode = (int)HttpStatusCode.MovedPermanently;    // 301
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
                        // Must set any headers, etc, before calling any method, like List, 
                        //  which writes to the response output stream (in this case via 
                        //  WriteResponseBody).  Once the output stream has been written to 
                        //  the response is sent so after that it's too late to set the headers.
                        SetResponseContentType(response, extension, DEF_CHARSET);
                        List(response, type, extension);
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
                            SetResponseContentType(response, extension, DEF_CHARSET);
                            Found(response, resource);
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
            response.ContentType = "application/" + extension + s;
        }

        private void List(HttpListenerResponse response, string type, string extension)
        {
            bool serveJson = extension.ToLower() == "json";

            string listBody = BuildList(type, serveJson);

            WriteResponseBody(response, listBody, DEF_CHARSET);
        }

        private string BuildList(string type, bool serveJson)
        {
            StringBuilder sb = new StringBuilder();
            if (type.ToLower().Contains("root-context"))
            {
                BuildRootContextList(sb, serveJson);
            }
            else
            {
                BuildContextList(sb, serveJson, type);
            }
            return sb.ToString();
        }

        private void BuildRootContextList(StringBuilder sb, bool serveJson)
        {
            string tagName = "root-context";
            string openTag = GetOpenTag(tagName, serveJson);
            string closeTag = GetCloseTag(tagName, serveJson);

            sb.Append(openTag);

            bool isFirstPass = true;
            // By default there is only one context: "/resources".
            foreach (string context in this.Resources.Contexts)
            {
                if (serveJson && !isFirstPass)
                {
                    sb.Append(", ");
                }
                BuildContextList(sb, serveJson, context);
                isFirstPass = false;
            }

            sb.Append(closeTag);
        }

        private void BuildContextList(StringBuilder sb, bool serveJson, string context)
        {
            string slashRemoved = context.Substring(1);

            string openTag = GetOpenTag(slashRemoved, serveJson);
            string closeTag = GetCloseTag(slashRemoved, serveJson);

            sb.Append(openTag);

            bool isFirstPass = true;
            foreach (Resource r in this.Resources.asCollection(context))
            {
                if (serveJson && !isFirstPass)
                {
                    sb.Append(", ");
                }
                sb.Append(r.Payload);
                isFirstPass = false;
            }

            sb.Append(closeTag);
        }

        private string GetOpenTag(string tagName, bool serveJson)
        {
            if (serveJson)
            {
                return "{ \"" + tagName + "\" : [ "; ;
            }

            return "<" + tagName + ">";
        }

        private string GetCloseTag(string tagName, bool serveJson)
        {
            if (serveJson)
            {
                return " ] }";
            }

            return "</" + tagName + ">";
        }
    }
}