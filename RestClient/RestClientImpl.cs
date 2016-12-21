using System;
using System.Collections.Generic;

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
namespace RestClient
{
	using Header = org.apache.commons.httpclient.Header;
	using HttpClient = org.apache.commons.httpclient.HttpClient;
	using HttpException = org.apache.commons.httpclient.HttpException;
	using HttpMethod = org.apache.commons.httpclient.HttpMethod;
	using URI = org.apache.commons.httpclient.URI;
	using URIException = org.apache.commons.httpclient.URIException;
	using EntityEnclosingMethod = org.apache.commons.httpclient.methods.EntityEnclosingMethod;
	using FileRequestEntity = org.apache.commons.httpclient.methods.FileRequestEntity;
	using RequestEntity = org.apache.commons.httpclient.methods.RequestEntity;
	using FilePart = org.apache.commons.httpclient.methods.multipart.FilePart;
	using MultipartRequestEntity = org.apache.commons.httpclient.methods.multipart.MultipartRequestEntity;
	using Part = org.apache.commons.httpclient.methods.multipart.Part;
	using StringPart = org.apache.commons.httpclient.methods.multipart.StringPart;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// A generic REST client based on {@code HttpClient}.
	/// </summary>
	public class RestClientImpl : RestClient
	{

		private static Logger LOG = LoggerFactory.getLogger(typeof(RestClientImpl));

		private readonly HttpClient client;

		private string baseUrl;

		/// <summary>
		/// Constructor allowing the injection of an {@code
		/// org.apache.commons.httpclient.HttpClient}.
		/// </summary>
		/// <param name="client"> the client
		///               See <seealso cref="org.apache.commons.httpclient.HttpClient"/> </param>
		public RestClientImpl(HttpClient client)
		{
			if (client == null)
			{
				throw new System.ArgumentException("Null HttpClient instance");
			}
			this.client = client;
		}

		/// <summary>
		/// See <seealso cref="smartrics.rest.client.RestClient#setBaseUrl(java.lang.String)"/>
		/// </summary>
		public virtual string BaseUrl
		{
			set
			{
				this.baseUrl = value;
			}
			get
			{
				return baseUrl;
			}
		}


		/// <summary>
		/// Returns the Http client instance used by this implementation.
		/// </summary>
		/// <returns> the instance of HttpClient
		/// See <seealso cref="org.apache.commons.httpclient.HttpClient"/>
		/// See <seealso cref="smartrics.rest.client.RestClientImpl#RestClientImpl(HttpClient)"/> </returns>
		public virtual HttpClient Client
		{
			get
			{
				return client;
			}
		}

		/// <summary>
		/// See <seealso cref="smartrics.rest.client.RestClient#execute(smartrics.rest.client.RestRequest)"/>
		/// </summary>
		public virtual RestResponse execute(RestRequest request)
		{
			return execute(BaseUrl, request);
		}

		/// <summary>
		/// See <seealso cref="smartrics.rest.client.RestClient#execute(java.lang.String, smartrics.rest.client.RestRequest)"/>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public RestResponse execute(String hostAddr, final RestRequest request)
		public virtual RestResponse execute(string hostAddr, RestRequest request)
		{
			if (request == null || !request.Valid)
			{
				throw new System.ArgumentException("Invalid request " + request);
			}
			if (request.TransactionId == null)
			{
				request.TransactionId = Convert.ToInt64(DateTimeHelper.CurrentUnixTimeMillis());
			}
			LOG.debug("request: {}", request);
			HttpMethod m = createHttpClientMethod(request);
			configureHttpMethod(m, hostAddr, request);
			// Debug Client
			if (LOG.DebugEnabled)
			{
				try
				{
					LOG.info("Http Request URI : {}", m.URI);
				}
				catch (URIException e)
				{
					LOG.error("Error URIException in debug : " + e.Message, e);
				}
				// Request Header
				LOG.debug("Http Request Method Class : {} ", m.GetType());
				LOG.debug("Http Request Header : {} ", Arrays.ToString(m.RequestHeaders));
				// Request Body
				if (m is EntityEnclosingMethod)
				{
					try
					{
						System.IO.MemoryStream requestOut = new System.IO.MemoryStream();
						((EntityEnclosingMethod) m).RequestEntity.writeRequest(requestOut);
						LOG.debug("Http Request Body : {}", requestOut.ToString());
					}
					catch (IOException e)
					{
						LOG.error("Error in reading request body in debug : " + e.Message, e);
					}
				}
			}
			// Prepare Response
			RestResponse resp = new RestResponse();
			resp.TransactionId = request.TransactionId;
			resp.Resource = request.Resource;
			try
			{
				client.executeMethod(m);
				foreach (Header h in m.ResponseHeaders)
				{
					resp.addHeader(h.Name, h.Value);
				}
				resp.StatusCode = m.StatusCode;
				resp.StatusText = m.StatusText;
				resp.RawBody = m.ResponseBody;
				// Debug
				if (LOG.DebugEnabled)
				{
					LOG.debug("Http Request Path : {}", m.Path);
					LOG.debug("Http Request Header : {} ", Arrays.ToString(m.RequestHeaders));
					LOG.debug("Http Response Status : {}", m.StatusLine);
					LOG.debug("Http Response Body : {}", m.ResponseBodyAsString);
				}

			}
			catch (HttpException e)
			{
				string message = "Http call failed for protocol failure";
				throw new System.InvalidOperationException(message, e);
			}
			catch (IOException e)
			{
				string message = "Http call failed for IO failure";
				throw new System.InvalidOperationException(message, e);
			}
			finally
			{
				m.releaseConnection();
			}
			LOG.debug("response: {}", resp);
			return resp;
		}

		/// <summary>
		/// Configures the instance of HttpMethod with the data in the request and
		/// the host address.
		/// </summary>
		/// <param name="m">        the method class to configure </param>
		/// <param name="hostAddr"> the host address </param>
		/// <param name="request">  the rest request </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void configureHttpMethod(org.apache.commons.httpclient.HttpMethod m, String hostAddr, final RestRequest request)
		protected internal virtual void configureHttpMethod(HttpMethod m, string hostAddr, RestRequest request)
		{
			addHeaders(m, request);
			setUri(m, hostAddr, request);
			m.QueryString = request.Query;
			if (m is EntityEnclosingMethod)
			{
				RequestEntity requestEntity = null;
				string fileName = request.FileName;
				if (!string.ReferenceEquals(fileName, null))
				{
					requestEntity = configureFileUpload(fileName);
				}
				else
				{
					// Add Multipart
					IDictionary<string, RestMultipart> multipartFiles = request.MultipartFileNames;
					if ((multipartFiles != null) && (multipartFiles.Count > 0))
					{
						requestEntity = configureMultipartFileUpload(m, request, requestEntity, multipartFiles);
					}
					else
					{
						requestEntity = new RequestEntityAnonymousInnerClass(this, request);
					}
				}
				((EntityEnclosingMethod) m).RequestEntity = requestEntity;
			}
			else
			{
				m.FollowRedirects = request.FollowRedirect;
			}

		}

		private class RequestEntityAnonymousInnerClass : RequestEntity
		{
			private readonly RestClientImpl outerInstance;

			private RestRequest request;

			public RequestEntityAnonymousInnerClass(RestClientImpl outerInstance, RestRequest request)
			{
				this.outerInstance = outerInstance;
				this.request = request;
			}

			public virtual bool Repeatable
			{
				get
				{
					return true;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeRequest(OutputStream out) throws IOException
			public virtual void writeRequest(System.IO.Stream @out)
			{
				PrintWriter printer = new PrintWriter(@out);
				printer.print(request.Body);
				printer.flush();
			}

			public virtual long ContentLength
			{
				get
				{
					return request.Body.Bytes.length;
				}
			}

			public virtual string ContentType
			{
				get
				{
					IList<smartrics.rest.client.RestData.Header> values = request.getHeader("Content-Type");
					string v = "text/xml";
					if (values.Count != 0)
					{
						v = values[0].Value;
					}
					return v;
				}
			}
		}


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.apache.commons.httpclient.methods.RequestEntity configureMultipartFileUpload(org.apache.commons.httpclient.HttpMethod m, final RestRequest request, org.apache.commons.httpclient.methods.RequestEntity requestEntity, java.util.Map<String, RestMultipart> multipartFiles)
		private RequestEntity configureMultipartFileUpload(HttpMethod m, RestRequest request, RequestEntity requestEntity, IDictionary<string, RestMultipart> multipartFiles)
		{
			MultipartRequestEntity multipartRequestEntity = null;
			// Current File Name reading for tracking missing file
			string fileName = null;

			IList<Part> fileParts = new List<Part>(multipartFiles.Count);
			// Read File Part
			foreach (KeyValuePair<string, RestMultipart> multipartFile in multipartFiles.SetOfKeyValuePairs())
			{
				Part filePart = createMultipart(multipartFile.Key, multipartFile.Value);
				fileParts.Add(filePart);
			}
			Part[] parts = fileParts.ToArray();
			multipartRequestEntity = new MultipartRequestEntity(parts, ((EntityEnclosingMethod) m).Params);

			return multipartRequestEntity;
		}

		private Part createMultipart(string fileParamName, RestMultipart restMultipart)
		{
			RestMultipart.RestMultipartType type = restMultipart.Type;
			switch (type)
			{
				case smartrics.rest.client.RestMultipart.RestMultipartType.FILE:
					string fileName = null;
					try
					{
						fileName = restMultipart.Value;
						File file = new File(fileName);
						FilePart filePart = new FilePart(fileParamName, file, restMultipart.ContentType, restMultipart.Charset);
						LOG.info("Configure Multipart file upload paramName={} :  ContentType={} for  file={} ", new string[]{fileParamName, restMultipart.ContentType, fileName});
						return filePart;
					}
					catch (FileNotFoundException e)
					{
						throw new System.ArgumentException("File not found: " + fileName, e);
					}
				case smartrics.rest.client.RestMultipart.RestMultipartType.STRING:
					StringPart stringPart = new StringPart(fileParamName, restMultipart.Value, restMultipart.Charset);
					stringPart.ContentType = restMultipart.ContentType;
					LOG.info("Configure Multipart String upload paramName={} :  ContentType={} ", fileParamName, stringPart.ContentType);
					return stringPart;
				default:
					throw new System.ArgumentException("Unknonw Multipart Type : " + type);
			}

		}



		private RequestEntity configureFileUpload(string fileName)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final File file = new File(fileName);
			File file = new File(fileName);
			if (!file.exists())
			{
				throw new System.ArgumentException("File not found: " + fileName);
			}
			return new FileRequestEntity(file, "application/octet-stream");
		}

		public virtual string getContentType(RestRequest request)
		{
			IList<smartrics.rest.client.RestData.Header> values = request.getHeader("Content-Type");
			string v = "text/xml";
			if (values.Count != 0)
			{
				v = values[0].Value;
			}
			return v;
		}

		private void setUri(HttpMethod m, string hostAddr, RestRequest request)
		{
			string host = string.ReferenceEquals(hostAddr, null) ? client.HostConfiguration.Host : hostAddr;
			if (string.ReferenceEquals(host, null))
			{
				throw new System.InvalidOperationException("hostAddress is null: please config httpClient host configuration or " + "pass a valid host address or config a baseUrl on this client");
			}
			string uriString = host + request.Resource;
			bool escaped = request.ResourceUriEscaped;
			try
			{
				m.URI = createUri(uriString, escaped);
			}
			catch (URIException e)
			{
				throw new System.InvalidOperationException("Problem when building URI: " + uriString, e);
			}
			catch (System.NullReferenceException e)
			{
				throw new System.InvalidOperationException("Building URI with null string", e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.commons.httpclient.URI createUri(String uriString, boolean escaped) throws org.apache.commons.httpclient.URIException
		protected internal virtual URI createUri(string uriString, bool escaped)
		{
			return new URI(uriString, escaped);
		}

		/// <summary>
		/// factory method that maps a string with a HTTP method name to an
		/// implementation class in Apache HttpClient. Currently the name is mapped
		/// to <code>org.apache.commons.httpclient.methods.%sMethod</code> where
		/// <code>%s</code> is the parameter mName.
		/// </summary>
		/// <param name="mName"> the method name </param>
		/// <returns> the method class </returns>
		protected internal virtual string getMethodClassnameFromMethodName(string mName)
		{
			return string.Format("org.apache.commons.httpclient.methods.{0}Method", mName);
		}

		/// <summary>
		/// Utility method that creates an instance of {@code
		/// org.apache.commons.httpclient.HttpMethod}.
		/// </summary>
		/// <param name="request"> the rest request </param>
		/// <returns> the instance of {@code org.apache.commons.httpclient.HttpMethod}
		/// matching the method in RestRequest. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected org.apache.commons.httpclient.HttpMethod createHttpClientMethod(RestRequest request)
		protected internal virtual HttpMethod createHttpClientMethod(RestRequest request)
		{
			string mName = request.Method.ToString();
			string className = getMethodClassnameFromMethodName(mName);
			try
			{
				Type<HttpMethod> clazz = (Type<HttpMethod>) Type.GetType(className);
				if (className.EndsWith("TraceMethod", StringComparison.Ordinal))
				{
					return clazz.GetConstructor(typeof(string)).newInstance("http://dummy.com");
				}
				else
				{
					return clazz.newInstance();
				}
			}
			catch (ClassNotFoundException e)
			{
				throw new System.InvalidOperationException(className + " not found: you may be using a too old or " + "too new version of HttpClient", e);
			}
			catch (InstantiationException e)
			{
				throw new System.InvalidOperationException("An object of type " + className + " cannot be instantiated", e);
			}
			catch (IllegalAccessException e)
			{
				throw new System.InvalidOperationException("The default ctor for type " + className + " cannot be accessed", e);
			}
			catch (Exception e)
			{
				throw new System.InvalidOperationException("Exception when instantiating: " + className, e);
			}
			catch (InvocationTargetException e)
			{
				throw new System.InvalidOperationException("The ctor with String.class arg for type " + className + " cannot be invoked", e);
			}
			catch (NoSuchMethodException e)
			{
				throw new System.InvalidOperationException("The ctor with String.class arg for type " + className + " doesn't exist", e);
			}
		}

		private void addHeaders(HttpMethod m, RestRequest request)
		{
			foreach (RestData.Header h in request.Headers)
			{
				m.addRequestHeader(h.Name, h.Value);
			}
		}
	}

}