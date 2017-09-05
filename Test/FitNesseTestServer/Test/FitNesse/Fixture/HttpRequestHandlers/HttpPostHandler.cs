using System;
using System.IO;
using System.Net;
using System.Text;
using NLog;

namespace FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    public class HttpPostHandler : HttpMethodHandlerBase
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public override void ProcessHttpRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            LOG.Debug("Processing POST request {0}...", request.Url);

            using (HttpListenerResponse response = context.Response)
            {
                string localUrl = this.Sanitise(request.Url.LocalPath);
                string type = this.GetResourceType(localUrl);

                try
                {
                    string contentType = request.ContentType;
                    if (contentType.ToLower() == "application/octet-stream")
                    {
                        LOG.Debug("POST REQUEST is a file upload");
                        ProcessFileUpload(request, response);
                    }
                    else if (contentType.StartsWith("multipart/form-data", 
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        LOG.Debug("POST REQUEST is a multipart file upload");
                        ProcessMultiPart(request, response);
                    }
                    else
                    {
                        string content = ReadRequestBody(request, DEF_CHARSET);
                        if (contentType.ToLower().Contains("application/x-www-form-urlencoded"))
                        {
                            try
                            {
                                GenerateResponse(response, type, noddyKvpToXml(content));
                            }
                            catch (Exception)
                            {
                                LOG.Warn("the content passed in isn't encoded as application/x-www-form-urlencoded: " + content);
                                response.StatusCode = (int)HttpStatusCode.BadRequest;   // 400
                            }
                        }
                        else if (content.Trim().StartsWith("<", StringComparison.Ordinal) 
                            || content.Trim().EndsWith("}", StringComparison.Ordinal))
                        {
                            GenerateResponse(response, type, content);
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;   // 400
                        }
                    }
                }
                catch (Exception)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;   // 500
                }
            }
        }
        private string noddyKvpToXml(string content)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<resource>").Append("\n");
            string[] kvpArray = content.Split(new char[] {'&'});
            foreach (string e in kvpArray)
            {
                string[] kvp = e.Split(new char[] { '=' });
                sb.Append("<").Append(kvp[0]).Append(">");
                if (kvp.Length > 1)
                {
                    string decodedString = WebUtility.UrlDecode(kvp[1]);
                    sb.Append(decodedString);
                }
                sb.Append("</").Append(kvp[0]).Append(">").Append("\n");
            }
            sb.Append("</resource>");
            return sb.ToString();
        }

        private void ProcessFileUpload(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (Stream requestStream = request.InputStream)
            using (Stream responseStream = response.OutputStream)
            {
                requestStream.CopyTo(responseStream);
            }

            response.ContentType = "text/plain";
            response.StatusCode = (int)HttpStatusCode.OK;  // 200
        }

        /// <summary>
        /// Parses multipart form data in the HTTP request and writes it back, in modified form, 
        /// to the response.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="response">The HTTP response.</param>
        /// <remarks>See RFC 7578 "Returning Values from Forms: multipart/form-data"</remarks>
        private void ProcessMultiPart(HttpListenerRequest request, HttpListenerResponse response)
        {
            return;
            // RFC 7578 states that a part of a request body in a multipart/form-data request takes 
            //  the form:

            //--{boundary parameter from Content-Type header}{CRLF}
            //Content-Disposition: form-data; name="{field name}"[; filename="{file name}"]{CRLF}
            //[Content-Type: {content type[;charset={character set}]}{CRLF}]
            //[Content-Transfer-Encoding: {content transfer encoding}{CRLF}]
            //[CRLF,...n]
            //{field data}{CRLF}
            //--{boundary parameter from Content-Type header}
            //etc...

            // NOTES:
            // 1) The Content-Disposition "filename" parameter is NOT mandatory for uploaded files.
            //      However, it could be impossible, when parsing an arbitrary HTTP request, 
            //      to determine if the field data represents the contents of a file without a 
            //      "filename" parameter; 
            // 2) The filename value MAY be percent-encoded to allow for non-standard filenames, 
            //      such as those which include spaces.  A percent-encoded octet is encoded as a 
            //      character triplet: "%nn", where each "n" is a case-insensitive hex digit;
            // 3) Multiple files for one form field:  Multiple files must be sent in separate 
            //      parts but all of these parts should have the same "name" parameter; 
            // 4) Content-Type: 
            //      a) Defaults to "text/plain".  A charset parameter MAY be included for 
            //          "text/plain".  Usually, however, a default charset is specified for all 
            //          parts.  The default charset is specified via a form field named "_charset_".
            //          This will be sent as a part in the body of the request.  For example:
            //              --AaB03x
            //              content-disposition: form-data; name="_charset_"
            //              
            //              iso-8859-1
            //              --AaB03x--
            //              content-disposition: form-data; name="field1"
            //
            //              ...text encoded in iso-8859-1...
            //              AaB03x--
            //      b) If a file is sent, the Content-Type should be specified, if known.  Clients 
            //          should set the Content-Type to "application/octet-stream" if the exact 
            //          content type of the file is not known at the time the file is uploaded;
            // 5) Content-Transfer-Encoding: This is deprecated but parsers still need to handle 
            //      it in case a client includes a Content-Transfer-Encoding header in a part;
            // 6) Character Encoding in HTML 5: Character encoding is determined by checking the 
            //      following in order:
            //      a) "_charset_" field value;
            //      b) <form> element accept-charset attribute;
            //      c) Character encoding of the HTML document containing the form, if it is 
            //          US-ASCII compatible;
            //      d) UTF-8
            string requestContents = ReadRequestBody(request, DEF_CHARSET);
            string boundary = GetMultiPartBoundary(request.ContentType, requestContents);
            if (boundary == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;   // 400
                LOG.Warn("Unable to process multipart form data: No boundary specified in content type.");
                return;
            }

            string[] parts = requestContents.Split(new string[] { boundary }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;   // 400
                LOG.Warn("Request contents contained no multipart form data.");
                return;
            }

            // RFC 7578, section 4.2: Each part must contain a "Content-Disposition" header field 
            //  set to "form-data", with a "name" parameter set to the name of the field that is 
            //  being supplied in the part.  If the part represents the contents of a file the 
            //  Content-Disposition header SHOULD have a "filename" parameter.  This is not 
            //  mandatory, however.  

            //  The original RestFixture ResourcesServlet.java, which this class is based on, uses 
            //  a com.oreilly.servlet.multipart.MultipartParser to parse the parts of a request.  
            //  MultipartParser.readNextPart() uses the presence or absence of a filename parameter 
            //  to determine whether a request part represents a file or not.  It's not totally 
            //  accurate but we'll just follow that reference implementation.

            //foreach (string part in parts)
            //{
                
            //} 

            //response.ContentType = "text/plain";

            //PrintWriter @out = resp.Writer;
            //MultipartParser mp = new MultipartParser(req, 2048);
            //Part part = null;
            //while ((part = mp.readNextPart()) != null)
            //{
            //    string name = part.Name.Trim();
            //    if (part.Param)
            //    {
            //        // it's a parameter part
            //        ParamPart paramPart = (ParamPart)part;
            //        string value = paramPart.StringValue.Trim();
            //        LOG.info("param; name=" + name + ", value=" + value);
            //        @out.print("param; name=" + name + ", value=" + value);
            //    }
            //    else if (part.File)
            //    {
            //        // it's a file part
            //        FilePart filePart = (FilePart)part;
            //        string fileName = filePart.FileName;
            //        if (!string.ReferenceEquals(fileName, null))
            //        {
            //            // the part actually contained a file
            //            // StringWriter sw = new StringWriter();
            //            // long size = filePart.writeTo(new File(System
            //            // .getProperty("java.io.tmpdir")));
            //            System.IO.MemoryStream baos = new System.IO.MemoryStream();
            //            long size = filePart.writeTo(baos);
            //            LOG.info("file; name=" + name + "; filename=" + fileName + ", filePath=" + filePart.FilePath + ", content type=" + filePart.ContentType + ", size=" + size);
            //            @out.print(string.Format("{0}: {1}", name, (StringHelperClass.NewString(baos.toByteArray())).Trim()));
            //        }
            //        else
            //        {
            //            // the field did not contain a file
            //            LOG.info("file; name=" + name + "; EMPTY");
            //        }
            //        @out.flush();
            //    }
            //}
            //resp.Status = HttpServletResponse.SC_OK;
        }

        /// <summary>
        /// Gets the text used to mark the boundaries between the different parts of a multipart 
        /// request body.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>A string with the text that marks the boundaries between the different parts 
        /// of a multipart request body, or null if the Content-Type header has no boundary 
        /// parameter or the Content-Type boundary parameter is not found in the request body.  
        /// </returns>
        /// <remarks>The Content-Type header will be similar to:
        ///     Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryUOaE2JpZxwqbIieW
        /// or
        ///     Content-Type: multipart/form-data; boundary="----WebKitFormBoundaryUOaE2JpZxwqbIieW"
        /// (note the double quotes around the boundary value in the second example)
        /// The boundary text will be something unusual, that would never occur within any of the 
        /// form field names or data, to avoid accidental collisions between the boundaries and the 
        /// form information.  It must be between 1 and 70 characters long.
        /// 
        /// RFC 7578, "Returning Values from Forms: multipart/form-data", section 4.1, 
        /// "'Boundary' Parameter of multipart/form-data", and RFC 2046, "Multipurpose Internet 
        /// Mail Extensions (MIME) Part Two: Media Types", section 5.1, "Multipart Media Type" 
        /// state that the delimiter between the different parts of the request body is:
        ///  
        /// --{Content-Type header boundary parameter}[whitespace,...n]
        /// 
        /// The initial hyphens MUST occur at the start of the line.  The optional trailing 
        /// whitespace is to be assumed to have been added by a gateway and should be deleted. 
        /// 
        /// The delimiter following the final body part is slightly different:
        /// 
        /// --{Content-Type header boundary parameter}--[whitespace,...n]
        /// (it has two additional hyphens immediately following the boundary text)
        /// 
        /// Any content before the first delimiter line or after the last delimiter line in the 
        /// body should be ignored.
        /// 
        /// This method will return "--{Content-Type header boundary parameter}" 
        /// (without the quotes).</remarks>
        private string GetMultiPartBoundary(string requestContentType, string requestContents)
        {
            string contentType = requestContentType.ToLower();

            string[] contentTypeParts = contentType.Split(new string[] {"boundary"}, 
                StringSplitOptions.None);
            if (contentTypeParts.Length < 2)
            {
                return null;
            }

            // Strip leading and trailing white space, leading "=" and leading and trailing double 
            //  quotes.
            string boundary = contentTypeParts[1].TrimStart().TrimStart(new char[] {'=', '"'})
                .TrimEnd().TrimEnd(new char[] {'"' });
            if (string.IsNullOrWhiteSpace(boundary))
            {
                return null;
            }

            return "--" + boundary;
        }

        private void GenerateResponse(HttpListenerResponse response, string type, string content)
        {
            Resource newResource = new Resource(content);
            Resources.add(type, newResource);
            response.StatusCode = (int)HttpStatusCode.Created;  // 201
            response.AddHeader("Location", type + "/" + newResource.Id);
        }
    }
}