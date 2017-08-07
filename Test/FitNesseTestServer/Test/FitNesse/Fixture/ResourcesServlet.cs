using System;
using System.Text;

/*  Copyright 2017 Simon Elms
 *
 *  This file is part of RestFixture.Net, a .NET port of the original Java 
 *  RestFixture written by Fabrizio Cannizzo and others.
 *
 *  RestFixture.Net is free software:
 *  You can redistribute it and/or modify it under the terms of the
 *  GNU Lesser General Public License as published by the Free Software Foundation,
 *  either version 3 of the License, or (at your option) any later version.
 *
 *  RestFixture.Net is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with RestFixture.Net.  If not, see <http://www.gnu.org/licenses/>.
 */
namespace FitNesseTestServer.Test.FitNesse.Fixture
{



	using Log = org.apache.commons.logging.Log;
	using FilePart = com.oreilly.servlet.multipart.FilePart;
	using MultipartParser = com.oreilly.servlet.multipart.MultipartParser;
	using ParamPart = com.oreilly.servlet.multipart.ParamPart;
	using Part = com.oreilly.servlet.multipart.Part;

	/// <summary>
	/// The controller.
	/// 
	/// @author fabrizio
	/// 
	/// </summary>
	public class ResourcesServlet : HttpServlet
	{
		private static readonly Log LOG = LogFactory.getLog(typeof(ResourcesServlet));
		public const string CONTEXT_ROOT = "/resources";
		private const long serialVersionUID = -7012866414216034826L;
		private const string DEF_CHARSET = "ISO-8859-1";
		private readonly Resources resources = Resources.Instance;

		public ResourcesServlet()
		{
			LOG.info("Resources: " + resources.ToString());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void doGet(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws javax.servlet.ServletException, java.io.IOException
		protected internal override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			LOG.debug("Resource GET REQUEST ========= " + req.ToString());
			string uri = sanitise(req.RequestURI);
			string id = getId(uri);
			string type = getType(uri);
			string extension = getExtension(uri);
			echoHeader(req, resp);
			echoQString(req, resp);
			try
			{
				if (string.ReferenceEquals(id, null))
				{
					list(resp, type, extension);
					headers(resp, extension, DEF_CHARSET);
				}
				else if (resources.get(type, id) == null)
				{
					notFound(resp);
				}
				else
				{
					if (resources.get(type, id).Deleted)
					{
						notFound(resp);
					}
					else
					{
						found(resp, type, id);
						headers(resp, extension, DEF_CHARSET);
					}
				}
			}
			catch (Exception)
			{
				resp.sendError(HttpServletResponse.SC_BAD_REQUEST);
			}
			finally
			{
				LOG.debug("Resource GET RESPONSE ========= " + resp.ToString());
			}
		}

		private void echoQString(HttpServletRequest req, HttpServletResponse resp)
		{
			string qstring = req.QueryString;
			if (!string.ReferenceEquals(qstring, null))
			{
				resp.setHeader("Query-String", qstring);
			}
		}

		private string sanitise(string rUri)
		{
			string uri = rUri;
			if (uri.EndsWith("/", StringComparison.Ordinal))
			{
				uri = uri.Substring(0, uri.Length - 1);
			}
			return uri;
		}

		private void headers(HttpServletResponse resp, string extension, string optCharset)
		{
			resp.Status = HttpServletResponse.SC_OK;
			string s = "";
			if (!string.ReferenceEquals(optCharset, null))
			{
				s = ";charset=" + optCharset;
			}
			resp.addHeader("Content-Type", "application/" + extension + s);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void list(javax.servlet.http.HttpServletResponse resp, String type, String extension) throws java.io.IOException
		private void list(HttpServletResponse resp, string type, string extension)
		{
			if (type.Contains("root-context"))
			{
				list(resp, extension);
			}
			else
			{
				StringBuilder buffer = new StringBuilder();
				string slashremoved = type.Substring(1);
				if ("json".Equals(extension))
				{
					buffer.Append("{ \"" + slashremoved + "\" : ");
				}
				else
				{
					buffer.Append("<" + slashremoved + ">");
				}
				foreach (Resource r in resources.asCollection(type))
				{
					buffer.Append(r.Payload);
				}
				if ("json".Equals(extension))
				{
					buffer.Append("}");
				}
				else
				{
					buffer.Append("</" + slashremoved + ">");
				}
				resp.OutputStream.write(buffer.ToString().GetBytes());
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void list(javax.servlet.http.HttpServletResponse resp, String extension) throws java.io.IOException
		private void list(HttpServletResponse resp, string extension)
		{
			StringBuilder buffer = new StringBuilder();
			if ("json".Equals(extension))
			{
				buffer.Append("{ \"root-context\" : ");
			}
			else
			{
				buffer.Append("<root-context>");
			}
			resp.OutputStream.write(buffer.ToString().GetBytes());
			foreach (string s in resources.contexts())
			{
				list(resp, s, extension);
			}
			buffer = new StringBuilder();
			if ("json".Equals(extension))
			{
				buffer.Append("}");
			}
			else
			{
				buffer.Append("</root-context>");
			}
			resp.OutputStream.write(buffer.ToString().GetBytes());
		}

		private string getExtension(string uri)
		{
			int extensionPoint = uri.LastIndexOf(".", StringComparison.Ordinal);
			if (extensionPoint != -1)
			{
				return uri.Substring(extensionPoint + 1);
			}
			else
			{
				return "xml";
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void found(javax.servlet.http.HttpServletResponse resp, String type, String id) throws java.io.IOException
		private void found(HttpServletResponse resp, string type, string id)
		{
			StringBuilder buffer = new StringBuilder();
			Resource r = resources.get(type, id);
			buffer.Append(r);
			resp.OutputStream.write(buffer.ToString().GetBytes());
			// resp.setHeader("Content-Lenght",
			// Integer.toString(buffer.toString().getBytes().length));
		}

		private string getType(string uri)
		{
			if (uri.Length <= 1)
			{
				return "/root-context";
			}
			int pos = uri.Substring(1).IndexOf('/');
			string ret = uri;
			if (pos >= 0)
			{
				ret = uri.Substring(0, pos + 1);
			}
			return ret;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void notFound(javax.servlet.http.HttpServletResponse resp) throws java.io.IOException
		private void notFound(HttpServletResponse resp)
		{
			resp.OutputStream.write("".GetBytes());
			resp.Status = HttpServletResponse.SC_NOT_FOUND;
			// resp.setHeader("Content-Lenght", "0");
		}

		private void echoHeader(HttpServletRequest req, HttpServletResponse resp)
		{
			string s = req.getHeader("Echo-Header");
			if (!string.ReferenceEquals(s, null))
			{
				resp.setHeader("Echo-Header", s);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void doDelete(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws javax.servlet.ServletException, java.io.IOException
		protected internal override void doDelete(HttpServletRequest req, HttpServletResponse resp)
		{
			LOG.debug("Resource DELETE REQUEST ========= " + req.ToString());
			string uri = sanitise(req.RequestURI);
			string type = getType(uri);
			echoHeader(req, resp);
			string id = getId(uri);
			Resource resource = resources.get(type, id);
			if (resource != null)
			{
				// resource.setDeleted(true);
				resources.remove(type, id);
				resp.OutputStream.write("".GetBytes());
				resp.Status = HttpServletResponse.SC_OK;
			}
			else
			{
				notFound(resp);
			}
			resp.OutputStream.write("".GetBytes());
			resp.Status = HttpServletResponse.SC_NO_CONTENT;
			LOG.debug("Resource DELETE RESPONSE ========= " + req.ToString());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void doPut(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws javax.servlet.ServletException, java.io.IOException
		protected internal override void doPut(HttpServletRequest req, HttpServletResponse resp)
		{
			LOG.debug("Resource PUT REQUEST ========= " + req.ToString());
			echoHeader(req, resp);
			string uri = sanitise(req.RequestURI);
			string id = getId(uri);
			string type = getType(uri);
			string content = getContent(req.InputStream);
			Resource resource = resources.get(type, id);
			if (resource != null)
			{
				resource.Payload = content;
				resp.OutputStream.write("".GetBytes());
				resp.Status = HttpServletResponse.SC_OK;
			}
			else
			{
				notFound(resp);
			}
			LOG.debug("Resource PUT RESPONSE ========= " + req.ToString());
		}

		private string getId(string uri)
		{
			if (uri.Length <= 1)
			{
				return null;
			}
			int pos = uri.Substring(1).LastIndexOf("/", StringComparison.Ordinal);
			string sId = null;
			if (pos > 0)
			{
				sId = uri.Substring(pos + 2);
			}
			if (!string.ReferenceEquals(sId, null))
			{
				int pos2 = sId.LastIndexOf('.');
				if (pos2 >= 0)
				{
					sId = sId.Substring(0, pos2);
				}
			}
			return sId;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void processMultiPart(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws java.io.IOException
		private void processMultiPart(HttpServletRequest req, HttpServletResponse resp)
		{
			PrintWriter @out = resp.Writer;
			resp.ContentType = "text/plain";
			MultipartParser mp = new MultipartParser(req, 2048);
			Part part = null;
			while ((part = mp.readNextPart()) != null)
			{
				string name = part.Name.Trim();
				if (part.Param)
				{
					// it's a parameter part
					ParamPart paramPart = (ParamPart) part;
					string value = paramPart.StringValue.Trim();
					LOG.info("param; name=" + name + ", value=" + value);
					@out.print("param; name=" + name + ", value=" + value);
				}
				else if (part.File)
				{
					// it's a file part
					FilePart filePart = (FilePart) part;
					string fileName = filePart.FileName;
					if (!string.ReferenceEquals(fileName, null))
					{
						// the part actually contained a file
						// StringWriter sw = new StringWriter();
						// long size = filePart.writeTo(new File(System
						// .getProperty("java.io.tmpdir")));
						System.IO.MemoryStream baos = new System.IO.MemoryStream();
						long size = filePart.writeTo(baos);
						LOG.info("file; name=" + name + "; filename=" + fileName + ", filePath=" + filePart.FilePath + ", content type=" + filePart.ContentType + ", size=" + size);
						@out.print(string.Format("{0}: {1}", name, (StringHelperClass.NewString(baos.toByteArray())).Trim()));
					}
					else
					{
						// the field did not contain a file
						LOG.info("file; name=" + name + "; EMPTY");
					}
					@out.flush();
				}
			}
			resp.Status = HttpServletResponse.SC_OK;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void doPost(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws javax.servlet.ServletException, java.io.IOException
		protected internal override void doPost(HttpServletRequest req, HttpServletResponse resp)
		{
			LOG.debug("Resource POST REQUEST ========= " + req.ToString());
			echoHeader(req, resp);
			string uri = sanitise(req.RequestURI);
			string type = getType(uri);
			try
			{
				string contentType = req.ContentType;
				if (contentType.Equals("application/octet-stream"))
				{
					LOG.debug("Resource POST REQUEST is a file upload");
					processFileUpload(req, resp);
				}
				else if (contentType.StartsWith("multipart", StringComparison.Ordinal))
				{
					LOG.debug("Resource POST REQUEST is a multipart file upload");
					processMultiPart(req, resp);
				}
				else
				{
					string content = getContent(req.InputStream);
					if (contentType.Contains("application/x-www-form-urlencoded"))
					{
						try
						{
							generateResponse(resp, type, noddyKvpToXml(content, "UTF-8"));
						}
						catch (Exception)
						{
							LOG.warn("the content passed in isn't encoded as application/x-www-form-urlencoded: " + content);
							resp.sendError(HttpServletResponse.SC_BAD_REQUEST);
						}
					}
					else if (content.Trim().StartsWith("<", StringComparison.Ordinal) || content.Trim().EndsWith("}", StringComparison.Ordinal))
					{
						generateResponse(resp, type, content);
					}
					else
					{
						resp.sendError(HttpServletResponse.SC_BAD_REQUEST);
					}
				}
			}
			catch (Exception)
			{
				resp.sendError(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
			}
			LOG.debug("Resource POST RESPONSE ========= " + req.ToString());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String noddyKvpToXml(String content, String encoding) throws java.io.UnsupportedEncodingException
		private string noddyKvpToXml(string content, string encoding)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<resource>").Append("\n");
			string[] kvpArray = content.Split("&", true);
			foreach (string e in kvpArray)
			{
				string[] kvp = e.Split("=", true);
				sb.Append("<").Append(kvp[0]).Append(">");
				if (kvp.Length > 1)
				{
					sb.Append(URLDecoder.decode(kvp[1], encoding));
				}
				sb.Append("</").Append(kvp[0]).Append(">").Append("\n");
			}
			sb.Append("</resource>");
			return sb.ToString();
		}

		private void generateResponse(HttpServletResponse resp, string type, string content)
		{
			Resource newResource = new Resource(content);
			resources.add(type, newResource);
			// TODO: should put the ID in
			resp.Status = HttpServletResponse.SC_CREATED;
			resp.addHeader("Location", type + "/" + newResource.Id);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void processFileUpload(javax.servlet.http.HttpServletRequest req, javax.servlet.http.HttpServletResponse resp) throws java.io.IOException
		private void processFileUpload(HttpServletRequest req, HttpServletResponse resp)
		{
			System.IO.Stream @is = req.InputStream;
			PrintWriter @out = resp.Writer;
			resp.ContentType = "text/plain";
			string fileContents = getContent(@is);
			@out.print(fileContents.Trim());
			@out.flush();
			resp.Status = HttpServletResponse.SC_OK;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getContent(java.io.InputStream is) throws java.io.IOException
		private string getContent(System.IO.Stream @is)
		{
			StringBuilder sBuff = new StringBuilder();
			int c;
			while ((c = @is.Read()) != -1)
			{
				sBuff.Append((char) c);
			}
			string content = sBuff.ToString();
			return content;
		}

	}

}