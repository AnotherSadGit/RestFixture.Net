using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NLog;
using RestFixture.Net.Support;
using RestClient;
using RestClient.Data;

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
namespace RestFixture.Net
{
    using RestClient = RestClient.RestClient;

	//using smartrics.rest.fitnesse.fixture.support;


	/// <summary>
	/// A fixture that allows to simply test REST APIs with minimal efforts. The core
	/// principles underpinning this fixture are:
	/// <ul>
	/// <li>allowing documentation of a REST API by showing how the API looks like.
	/// For REST this means
	/// <ul>
	/// <li>show what the resource URI looks like. For example
	/// <code>/resource-a/123/resource-b/234</code>
	/// <li>show what HTTP operation is being executed on that resource. Specifically
	/// which one fo the main HTTP verbs where under test (GET, POST, PUT, DELETE,
	/// HEAD, OPTIONS).
	/// <li>have the ability to set headers and body in the request
	/// <li>check expectations on the return code of the call in order to document
	/// the behaviour of the API
	/// <li>check expectation on the HTTP headers and body in the response. Again, to
	/// document the behaviour
	/// </ul>
	/// <li>should work without the need to write/maintain java code: tests are
	/// written in wiki syntax.
	/// <li>tests should be easy to write and above all read.
	/// </ul>
	/// <para>
	/// <b>Configuring RestFixture</b><br>
	/// RestFixture can be configured by using the <seealso cref="RestFixtureConfig"/>. A
	/// {@code RestFixtureConfig} can define named maps with configuration key/value
	/// pairs. The name of the map is passed as second parameter to the
	/// {@code RestFixture}. Using a named configuration is optional: if no name is
	/// passed, the default configuration map is used. See <seealso cref="RestFixtureConfig"/>
	/// for more details.
	/// </para>
	/// <para>
	/// The following list of configuration parameters can are supported.
	/// <table border="1">
	/// <caption>supported configuration parameters</caption>
	/// <tr>
	/// <td>smartrics.rest.fitnesse.fixture.RestFixtureConfig</td>
	/// <td><i>optional named config</i></td>
	/// </tr>
	/// <tr>
	/// <td>http.proxy.host</td>
	/// <td><i>http proxy host name (RestClient proxy configuration)</i></td>
	/// </tr>
	/// <tr>
	/// <td>http.proxy.port</td>
	/// <td><i>http proxy host port (RestClient proxy configuration)</i></td>
	/// </tr>
	/// <tr>
	/// <td>http.basicauth.username</td>
	/// <td><i>username for basic authentication (RestClient proxy configuration)</i>
	/// </td>
	/// </tr>
	/// <tr>
	/// <td>http.basicauth.password</td>
	/// <td><i>password for basic authentication (RestClient proxy configuration)</i>
	/// </td>
	/// </tr>
	/// <tr>
	/// <td>http.client.connection.timeout</td>
	/// <td><i>client timeout for http connection (default 3s). (RestClient proxy
	/// configuration)</i></td>
	/// </tr>
	/// <tr>
	/// <tr>
	/// <td>http.client.use.new.http.uri.factory</td>
	/// <td><i>If set to true uses a more relaxed validation rule to validate URIs.
	/// It, for example, allows array parameters in the query string. Defaults to
	/// false.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.requests.follow.redirects</td>
	/// <td><i>If set to true the underlying client is instructed to follow redirects
	/// for the requests in the current fixture. This setting is not applied to POST
	/// and PUT (for which redirection is set to false) Defaults to true.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.resource.uris.are.escaped</td>
	/// <td><i>boolean value. if true, RestFixture will assume that the resource uris
	/// are already escaped. If false, resource uri will be escaped. Defaults to
	/// false.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.display.actual.on.right</td>
	/// <td><i>boolean value (default=true). if true, the actual value of the header or body in an
	/// expectation cell is displayed even when the expectation is met.</i></td>
	/// </tr>
	/// <tr>
	/// <tr>
	/// <td>restfixture.display.absolute.url.in.full</td>
	/// <td><i>boolean value (default=true). if true, absolute URLs in the fixture second column
	/// are rendered in their absolute format rather than relative.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.default.headers</td>
	/// <td><i>comma separated list of key value pairs representing the default list
	/// of headers to be passed for each request. key and values are separated by a
	/// colon. Entries are sepatated by \n. <seealso cref="RestFixture#setHeader()"/> will
	/// override this value. </i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.xml.namespaces.context</td>
	/// <td><i>comma separated list of key value pairs representing namespace
	/// declarations. The key is the namespace alias, the value is the namespace URI.
	/// alias and URI are separated by a = sign. Entries are sepatated by
	/// {@code System.getProperty("line.separator")}. These entries will be used to
	/// define the namespace context to be used in xpaths that are evaluated in the
	/// results.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.content.default.charset</td>
	/// <td>The default charset name (e.g. UTF-8) to use when parsing the response
	/// body, when a response doesn't contain a valid value in the Content-Type
	/// header. If a default is not specified with this property, the fixture will
	/// use the default system charset, available via
	/// <code>Charset.defaultCharset().name()</code></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.content.handlers.map</td>
	/// <td><i>a map of content type to type adapters, entries separated by \n, and
	/// kye-value separated by '='. Available type adapters are JS, TEXT, JSON, XML
	/// (see <seealso cref="BodyTypeAdapterFactory"/>
	/// ).</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.null.value.representation</td>
	/// <td><i>This string is used in replacement of the default string substituted
	/// when a null value is set for a symbol. Because now the RestFixture labels
	/// support is implemented on top of the Fitnesse symbols, such default value is
	/// defined in Fitnesse, and that is the string 'null'. Hence, every substitution
	/// that would result in rendering the string 'null' is replaced with the value
	/// set for this config key. This value can also be the empty string to replace
	/// null with empty.</i></td>
	/// </tr>
	/// <tr>
	/// <td>restfixture.javascript.imports.map</td>
	/// <td><i>a map of name to Url/File path. Each entry refers to a url/path to a javascript file that is imported in the
	/// JS context and available for evaluation. Files are checked for existence and access. If not available/not accessible,
	/// an exception is thrown. Urls are tried to be downloaded. A failure in accessing the content causes an error/exception.
	/// The map key is the name of the library - it can be a freeform string that appears in logs for debugging purposes.
	/// </i></td>
	/// </tr>
	/// </table>
	/// 
	/// @author smartrics
	/// </para>
	/// </summary>
	public class RestFixture : RunnerVariablesProvider
	{
        private static Version _fitSharpVersion = Assembly.GetEntryAssembly().GetName().Version;

        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// What runner this table is running on.
		/// 
		/// Note, the OTHER runner is primarily for testing purposes.
		/// 
		/// </summary>
		public enum Runner
		{
			/// <summary>
			/// the slim runner
			/// </summary>
			SLIM,
			/// <summary>
			/// the fit runner
			/// </summary>
			FIT,
			/// <summary>
			/// any other runner
			/// </summary>
			OTHER
		}

		public Variables createRunnerVariables()
		{
			switch (runner)
			{
			case Runner.SLIM:
				//return new SlimVariables(config, slimStatementExecutor);
                return new SlimVariables(config);
			case Runner.FIT:
				return new FitVariables(config);
			default:
				// Use FitVariables for tests
				return new FitVariables(config);
			}
		}

		private const string LINE_SEPARATOR = "\n";

		private const string FILE = "file";

		protected internal Variables GLOBALS;

		private RestResponse lastResponse;

		private RestRequest lastRequest;

		protected internal string fileName = null;

		protected internal string multipartFileName = null;

		protected internal string multipartFileParameterName = FILE;

		protected internal LinkedHashMap<string, RestMultipart> multiFileNameByParamName = new LinkedHashMap<string, RestMultipart>();

		protected internal string requestBody;

		protected internal bool resourceUrisAreEscaped = false;

		protected internal LinkedHashMap<string, string> requestHeaders;

		private RestClient restClient;

		private Config config;

		private Runner runner;

		private bool displayActualOnRight;

		private bool displayAbsoluteURLInFull;

		private bool debugMethodCall = false;

		/// <summary>
		/// the headers passed to each request by default.
		/// </summary>
		private IDictionary<string, string> defaultHeaders = new Dictionary<string, string>();

		private IDictionary<string, string> namespaceContext = new Dictionary<string, string>();

		private Url _baseUrl;

		protected internal RowWrapper<object> row;

		private CellFormatter<object> formatter;

		private PartsFactory partsFactory;

		private string lastEvaluation;

		private int minLenForCollapseToggle;

		private bool followRedirects = true;

		//private StatementExecutorInterface slimStatementExecutor;


		static RestFixture()
		{
			LOG.Info("############ Detected FitSharp version: {} ###########", _fitSharpVersion);
		}

		/// <summary>
		/// Constructor for Fit runner.
		/// </summary>
		public RestFixture() : base()
		{
			this.partsFactory = new PartsFactory(this, Net.Support.Config.getConfig(Config.DEFAULT_CONFIG_NAME));
			this.displayActualOnRight = true;
			this.minLenForCollapseToggle = -1;
			this.resourceUrisAreEscaped = false;
			this.displayAbsoluteURLInFull = true;
			this.requestHeaders = new LinkedHashMap<string, string>();
		}

		/// <summary>
		/// Constructor for Slim runner.
		/// </summary>
		/// <param name="hostName">
		///            the cells following up the first cell in the first row. </param>
		public RestFixture(string hostName) : this(hostName, Config.DEFAULT_CONFIG_NAME)
		{
		}

		/// <summary>
		/// Constructor for Slim runner.
		/// </summary>
		/// <param name="hostName">
		///            the cells following up the first cell in the first row. </param>
		/// <param name="configName">
		///            the value of cell number 3 in first row of the fixture table. </param>
		public RestFixture(string hostName, string configName)
		{
			this.displayActualOnRight = true;
			this.minLenForCollapseToggle = -1;
			this.resourceUrisAreEscaped = false;
			this.displayAbsoluteURLInFull = true;
			this.config = Config.getConfig(configName);
			this.partsFactory = new PartsFactory(this, config);
			this._baseUrl = new Url(stripTag(hostName));
			this.requestHeaders = new LinkedHashMap<string, string>();
		}

		/// <summary>
		/// VisibleForTesting </summary>
		/// <param name="partsFactory">
		///            the factory of parts necessary to create the rest fixture </param>
		/// <param name="hostName"> </param>
		/// <param name="configName"> </param>
		internal RestFixture(PartsFactory partsFactory, string hostName, string configName)
		{
			this.displayActualOnRight = true;
			this.minLenForCollapseToggle = -1;
			this.resourceUrisAreEscaped = false;
			this.displayAbsoluteURLInFull = true;
			this.partsFactory = partsFactory;
			this.config = Config.getConfig(configName);
			this._baseUrl = new Url(stripTag(hostName));
			this.requestHeaders = new LinkedHashMap<string, string>();
		}

		/// <returns> the config used for this fixture instance </returns>
		public virtual Config Config
		{
			get
			{
				return config;
			}
			set
			{
				this.config = value;
			}
		}

		/// <returns> the result of the last evaluation performed via evalJs. </returns>
		public virtual string LastEvaluation
		{
			get
			{
				return lastEvaluation;
			}
		}

		/// <summary>
		/// The base URL as defined by the rest fixture ctor or input args.
		/// </summary>
		/// <returns> the base URL as string </returns>
		public virtual string getBaseUrl()
		{
			if (_baseUrl != null)
			{
				return _baseUrl.ToString();
			}
			return null;
		}

		/// <summary>
		/// sets the base url.
		/// </summary>
		/// <param name="url"> the url </param>
		public virtual void setBaseUrl(Url url)
		{
			this._baseUrl = url;
		}

		public virtual void baseUrl(string url)
		{ //mqm  - it comes as a string in a scenario.
			this.setBaseUrl(new Url(url));
		}


		/// <summary>
		/// The default headers as defined in the config used to initialise this
		/// fixture.
		/// </summary>
		/// <returns> the map of default headers. </returns>
		public virtual IDictionary<string, string> DefaultHeaders
		{
			get
			{
				return defaultHeaders;
			}
		}

		/// <summary>
		/// The formatter for this instance of the RestFixture.
		/// </summary>
		/// <returns> the formatter for the cells </returns>
		public virtual CellFormatter<object> Formatter
		{
			get
			{
				return formatter;
			}
		}

		/// <summary>
		/// Slim Table table hook.
		/// </summary>
		/// <param name="rows"> the rows to process </param>
		/// <returns> the rendered content. </returns>
		public virtual IList<IList<string>> doTable(IList<IList<string>> rows)
		{
			initialize(Runner.SLIM);
			IList<IList<string>> res = new List<IList<string>>();
			Formatter.DisplayActual = displayActualOnRight;
			Formatter.DisplayAbsoluteURLInFull = displayAbsoluteURLInFull;
			Formatter.MinLengthForToggleCollapse = minLenForCollapseToggle;
			foreach (IList<string> r in rows)
			{
				processSlimRow(res, r);
			}
			return res;
		}

		/// <summary>
		/// Overrideable method to validate the state of the instance in execution. A
		/// <seealso cref="RestFixture"/> is valid if the baseUrl is not null.
		/// </summary>
		/// <returns> true if the state is valid, false otherwise </returns>
		protected internal virtual bool validateState()
		{
			return _baseUrl != null;
		}


		/// <summary>
		/// Method invoked to notify that the state of the RestFixture is invalid. It
		/// throws a <seealso cref="RuntimeException"/> with a message displayed in the
		/// FitNesse page.
		/// </summary>
		/// <param name="state">
		///            as returned by <seealso cref="RestFixture#validateState()"/> </param>
		protected internal virtual void notifyInvalidState(bool state)
		{
			if (!state)
			{
				throw new Exception("You must specify a base url in the |start|, after the fixture to start");
			}
		}


		/// <summary>
		/// Allows setting of the name of the multi-part file to upload.
		/// <para>
		/// <code>| setMultipartFileName | Name of file |</code>
		/// </para>
		/// <para>
		/// body text should be location of file which needs to be sent
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void setMultipartFileName()
		public virtual void setMultipartFileName()
		{
			CellWrapper<object> cell = row.getCell(1);
			if (cell == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a multipart file name to set");
			}
			else
			{
				multipartFileName = GLOBALS.substitute(cell.text());
				renderReplacement(cell, multipartFileName);
			}
		}

		/// <summary>
		/// Allows setting of the the multi-part file to upload.
		/// <para>
		/// <code>| addMultipartFile | Name of file | Name of form parameter for the uploaded file | ContentType of file | CharSet of file |</code>
		/// </para>
		/// <para>
		/// body text should be location of file which needs to be sent
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void addMultipartFile()
		public virtual void addMultipartFile()
		{
			CellWrapper<object> cellFileName = row.getCell(1);
			CellWrapper<object> cellParamName = row.getCell(2);
			CellWrapper<object> cellContentType = row.getCell(3);
			CellWrapper<object> cellCharset = row.getCell(4);
			if (cellFileName == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a multipart file name to set");
			}
			else
			{
				registerMultipartRow(RestMultipart.RestMultipartType.FILE, cellFileName, cellParamName, cellContentType, cellCharset);
			}
		}

		/// <summary>
		/// Allows setting of the multi-part String.
		/// <para>
		/// <code>| addMultipartString | String content | Name of form parameter  | ContentType of String | CharSet of String |</code>
		/// </para>
		/// <para>
		/// body text should be location of file which needs to be sent
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void addMultipartString()
		public virtual void addMultipartString()
		{
			CellWrapper<object> cellFileName = row.getCell(1);
			CellWrapper<object> cellParamName = row.getCell(2);
			CellWrapper<object> cellContentType = row.getCell(3);
			CellWrapper<object> cellCharset = row.getCell(4);
			if (cellFileName == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a multipart string content to set");
			}
			else
			{
				registerMultipartRow(RestMultipart.RestMultipartType.STRING, cellFileName, cellParamName, cellContentType, cellCharset);
			}
		}


		private RestMultipart registerMultipartRow(RestMultipart.RestMultipartType type, CellWrapper<object> cellFileName, CellWrapper<object> cellParamName, CellWrapper<object> cellContentType, CellWrapper<object> cellCharset)
		{
	// Param Name
			string multipartParamName = FILE;
			if (cellParamName != null)
			{
				multipartParamName = GLOBALS.substitute(cellParamName.text());
			}
			// FileName
			string multipartFileName = GLOBALS.substitute(cellFileName.text());
			// ContentType
			string multipartContentType = null;
			if (cellContentType != null)
			{
				multipartContentType = GLOBALS.substitute(cellContentType.text());
			}
			// Charset
			string multipartCharSet = null;
			if (cellCharset != null)
			{
				multipartCharSet = GLOBALS.substitute(cellCharset.text());
			}
			// Register Multipart
			RestMultipart restMultipart = new RestMultipart(type, multipartFileName, multipartContentType, multipartCharSet);
			multiFileNameByParamName[multipartParamName] = restMultipart;
			// Display Replacement
			renderReplacement(cellFileName, multipartFileName);
			if (cellParamName != null)
			{
				renderReplacement(cellParamName, multipartParamName);
			}
			if (cellContentType != null)
			{
				renderReplacement(cellContentType, multipartContentType);
			}
			if (cellCharset != null)
			{
				renderReplacement(cellCharset, multipartCharSet);
			}
			return restMultipart;
		}

		/// <returns> the multipart filename </returns>
		public virtual string MultipartFileName
		{
			get
			{
				return multipartFileName;
			}
		}

		/// <summary>
		/// Allows setting of the name of the file to upload.
		/// <para>
		/// <code>| setFileName | Name of file |</code>
		/// </para>
		/// <para>
		/// body text should be location of file which needs to be sent
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void setFileName()
		public virtual void setFileName()
		{
			CellWrapper<object> cell = row.getCell(1);
			if (cell == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a file name to set");
			}
			else
			{
				fileName = GLOBALS.substitute(cell.text());
				renderReplacement(cell, fileName);
			}
		}

		/// <returns> the filename </returns>
		public virtual string FileName
		{
			get
			{
				return fileName;
			}
		}

		/// <summary>
		/// Sets the parameter to send in the request storing the multi-part file to
		/// upload. If not specified the default is <code>file</code>
		/// <para>
		/// <code>| setMultipartFileParameterName | Name of form parameter for the uploaded file |</code>
		/// </para>
		/// <para>
		/// body text should be the name of the form parameter, defaults to 'file'
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void setMultipartFileParameterName()
		public virtual void setMultipartFileParameterName()
		{
			CellWrapper<object> cell = row.getCell(1);
			if (cell == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a parameter name to set");
			}
			else
			{
				multipartFileParameterName = GLOBALS.substitute(cell.text());
				renderReplacement(cell, multipartFileParameterName);
			}
		}

		/// <returns> the multipart file parameter name. </returns>
		public virtual string MultipartFileParameterName
		{
			get
			{
				return multipartFileParameterName;
			}
		}

		/// <summary>
		/// <code>| setBody | body text goes here |</code>
		/// <para>
		/// body text can either be a kvp or a xml. The <code>ClientHelper</code>
		/// will figure it out
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void setBody()
		public virtual void setBody()
		{
			CellWrapper<object> cell = row.getCell(1);
			if (cell == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a body to set");
			}
			else
			{
				string text = Formatter.fromRaw(cell.text());
				requestBody = GLOBALS.substitute(text);
				renderReplacement(cell, requestBody);
			}
		}

		/// <summary>
		/// \@sglebs - fixes #162. necessary to work with a scenario </summary>
		/// <param name="body"> the body to set </param>
		/// <returns> the body after substitution </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public String setBody(String body)
		public virtual string setBody(string body)
		{
			requestBody = body;
			if (GLOBALS != null)
			{
				requestBody = GLOBALS.substitute(body);
			}
			return requestBody;
		}

		/// <summary>
		/// <code>| setHeader | http headers go here as nvp |</code>
		/// <para>
		/// header text must be nvp. name and value must be separated by ':' and each
		/// header is in its own line
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void setHeader()
		public virtual void setHeader()
		{
			requestHeaders.Clear();
			addHeader();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void addHeader()
		public virtual void addHeader()
		{
			CellWrapper<object> cell = row.getCell(1);
			if (cell == null)
			{
				Formatter.exception(row.getCell(0), "You must pass a header map to set");
			}
			else
			{
				string substitutedHeaders = GLOBALS.substitute(cell.text());
				requestHeaders.PutAll(parseHeaders(substitutedHeaders));
				cell.body(Formatter.gray(substitutedHeaders));
			}
		}

		/// <summary>
		/// \@sglebs - fixes #161. necessary to work with a scenario </summary>
		/// <param name="headers"> the headers string to set </param>
		/// <returns> the headers map </returns>
		public virtual LinkedHashMap<string, string> addHeader(string headers)
		{
			string substitutedHeaders = headers;
			if (GLOBALS != null)
			{
				substitutedHeaders = GLOBALS.substitute(headers);
			}
			requestHeaders.PutAll(parseHeaders(substitutedHeaders));
			return requestHeaders;
		}

		/// <summary>
		/// \@sglebs - fixes #161. necessary to work with a scenario </summary>
		/// <param name="headers"> the headers string to set </param>
		/// <returns> the headers map </returns>
		public virtual LinkedHashMap<string, string> setHeader(string headers)
		{
			requestHeaders = new LinkedHashMap<string, string>();
			return addHeader(headers);
		}

		/// <summary>
		/// \@sglebs - fixes #161. necessary to work with a scenario </summary>
		/// <param name="headers"> the headers string to set </param>
		/// <returns> the headers map </returns>
		public virtual LinkedHashMap<string, string> setHeaders(string headers)
		{
			return setHeader(headers);
		}

		/// <summary>
		/// Equivalent to setHeader - syntactic sugar to indicate that you can now.
		/// <para>
		/// set multiple headers in a single call
		/// </para>
		/// </summary>
		public virtual void setHeaders()
		{
			setHeader();
		}

		public virtual void addHeaders()
		{
			addHeader();
		}

		/// <summary>
		/// <code> | PUT | URL | ?ret | ?headers | ?body |</code>
		/// <para>
		/// executes a PUT on the URL and checks the return (a string representation
		/// the operation return code), the HTTP response headers and the HTTP
		/// response body
		/// </para>
		/// <para>
		/// URL is resolved by replacing global variables previously defined with
		/// <code>let()</code>
		/// </para>
		/// <para>
		/// the HTTP request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void PUT()
		{
			debugMethodCallStart();
			doMethod(emptifyBody(requestBody), "Put");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | GET | uri | ?ret | ?headers | ?body |</code>
		/// <para>
		/// executes a GET on the uri and checks the return (a string repr the
		/// operation return code), the http response headers and the http response
		/// body
		/// 
		/// uri is resolved by replacing vars previously defined with
		/// <code>let()</code>
		/// 
		/// the http request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void GET()
		{
			debugMethodCallStart();
			doMethod("Get");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | HEAD | uri | ?ret | ?headers |  |</code>
		/// <para>
		/// executes a HEAD on the uri and checks the return (a string repr the
		/// operation return code) and the http response headers. Head is meant to
		/// return no-body.
		/// 
		/// uri is resolved by replacing vars previously defined with
		/// <code>let()</code>
		/// 
		/// the http request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void HEAD()
		{
			debugMethodCallStart();
			doMethod("Head");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | OPTIONS | uri | ?ret | ?headers | ?body |</code>
		/// <para>
		/// executes a OPTIONS on the uri and checks the return (a string repr the
		/// operation return code), the http response headers, the http response body
		/// 
		/// uri is resolved by replacing vars previously defined with
		/// <code>let()</code>
		/// 
		/// the http request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void OPTIONS()
		{
			debugMethodCallStart();
			doMethod("Options");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | DELETE | uri | ?ret | ?headers | ?body |</code>
		/// <para>
		/// executes a DELETE on the uri and checks the return (a string repr the
		/// operation return code), the http response headers and the http response
		/// body
		/// 
		/// uri is resolved by replacing vars previously defined with
		/// <code>let()</code>
		/// 
		/// the http request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void DELETE()
		{
			debugMethodCallStart();
			doMethod("Delete");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | TRACE | uri | ?ret | ?headers | ?body |</code>
		/// </summary>
		public virtual void TRACE()
		{
			debugMethodCallStart();
			doMethod("Trace");
			debugMethodCallEnd();
		}

		/// <summary>
		/// <code> | POST | uri | ?ret | ?headers | ?body |</code>
		/// <para>
		/// executes a POST on the uri and checks the return (a string repr the
		/// operation return code), the http response headers and the http response
		/// body
		/// 
		/// uri is resolved by replacing vars previously defined with
		/// <code>let()</code>
		/// 
		/// post requires a body that can be set via <code>setBody()</code>.
		/// 
		/// the http request headers can be set via <code>setHeaders()</code>. If not
		/// set, the list of default headers will be set. See
		/// <code>DEF_REQUEST_HEADERS</code>
		/// </para>
		/// </summary>
		public virtual void POST()
		{
			debugMethodCallStart();
			doMethod(emptifyBody(requestBody), "Post");
			debugMethodCallEnd();
		}

        /// <summary>
        /// <code> | let | label | type | loc | expr |</code>
        /// <para>
        /// allows to associate a value to a label. values are extracted from the
        /// body of the last successful http response.
        /// <ul>
        /// <li><code>label</code> is the label identifier
        /// 
        /// <li><code>type</code> is the type of operation to perform on the last
        /// http response. At the moment only XPaths and Regexes are supported. In
        /// case of regular expressions, the expression must contain only one group
        /// match, if multiple groups are matched the label will be assigned to the
        /// first found <code>type</code> only allowed values are <code>xpath</code>
        /// and <code>regex</code>
        /// 
        /// <li><code>loc</code> where to apply the <code>expr</code> of the given
        /// <code>type</code>. Currently only <code>header</code> and
        /// <code>body</code> are supported. If type is <code>xpath</code> by default
        /// the expression is matched against the body and the value in loc is
        /// ignored.
        /// 
        /// <li><code>expr</code> is the expression of type <code>type</code> to be
        /// executed on the last http response to extract the content to be
        /// associated to the label.
        /// </ul>
        /// </para>
        /// <para>
        /// <code>label</code>s can be retrieved after they have been defined and
        /// their scope is the fixture instance under execution. They are stored in a
        /// map so multiple calls to <code>let()</code> with the same label will
        /// override the current value of that label.
        /// </para>
        /// <para>
        /// Labels are resolved in <code>uri</code>s, <code>header</code>s and
        /// <code>body</code>es.
        /// </para>
        /// <para>
        /// In order to be resolved a label must be between <code>%</code>, e.g.
        /// <code>%id%</code>.
        /// </para>
        /// <para>
        /// The test row must have an empy cell at the end that will display the
        /// value extracted and assigned to the label.
        /// </para>
        /// <para>
        /// Example: <br>
        /// <code>| GET | /services | 200 | | |</code><br>
        /// <code>| let | id |  body | /services/id[0]/text() | |</code><br>
        /// <code>| GET | /services/%id% | 200 | | |</code>
        /// </para>
        /// <para>
        /// or
        /// </para>
        /// <para>
        /// <code>| POST | /services | 201 | | |</code><br>
        /// <code>| let  | id | header | /services/([.]+) | |</code><br>
        /// <code>| GET  | /services/%id% | 200 | | |</code>
        /// </para>
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void let()
        public virtual void let()
        {
            debugMethodCallStart();
            if (row.size() != 5)
            {
                Formatter.exception(row.getCell(row.size() - 1), "Not all cells found: | let | label | type | expr | result |");
                debugMethodCallEnd();
                return;
            }
            string label = row.getCell(1).text().Trim();
            string loc = row.getCell(2).text();
            CellWrapper<object> exprCell = row.getCell(3);
            try
            {
                exprCell.body(GLOBALS.substitute(exprCell.body()));
                string expr = exprCell.text();
                CellWrapper<object> valueCell = row.getCell(4);
                string valueCellText = valueCell.body();
                string valueCellTextReplaced = GLOBALS.substitute(valueCellText);
                valueCell.body(valueCellTextReplaced);
                string sValue = null;
                LetHandler letHandler = LetHandlerFactory.getHandlerFor(loc);
                if (letHandler != null)
                {
                    StringTypeAdapter adapter = new StringTypeAdapter();
                    try
                    {
                        sValue = letHandler.handle(this, config, LastResponse, namespaceContext, expr);
                        exprCell.body(Formatter.gray(exprCell.body()));
                    }
                    catch (Exception e)
                    {
                        Formatter.exception(exprCell, e.Message);
                        LOG.Error(e, "Exception occurred when processing cell={0}", exprCell);
                    }
                    GLOBALS.put(label, sValue);
                    adapter.set(sValue);
                    Formatter.check(valueCell, adapter);
                }
                else
                {
                    Formatter.exception(exprCell, "I don't know how to process the expression for '" + loc + "'");
                }
            }
            catch (Exception e)
            {
                Formatter.exception(exprCell, e);
            }
            finally
            {
                debugMethodCallEnd();
            }
        }

        /// <summary>
        /// allows to add comments to a rest fixture - basically does nothing except ignoring the text.
        /// the text is substituted if variables are found.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public void comment()
        public virtual void comment()
        {
            debugMethodCallStart();
            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: @SuppressWarnings("rawtypes") CellWrapper messageCell = row.getCell(1);
            CellWrapper<object> messageCell = row.getCell(1);
            try
            {
                string message = messageCell.text().Trim();
                message = GLOBALS.substitute(message);
                messageCell.body(Formatter.gray(message));
            }
            catch (Exception e)
            {
                Formatter.exception(messageCell, e);
            }
            finally
            {
                debugMethodCallEnd();
            }
        }

        /// <summary>
        /// Evaluates a string using the internal JavaScript engine. Result of the
        /// last evaluation is set in the attribute lastEvaluation.
        /// 
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void evalJs()
        public virtual void evalJs()
        {
            CellWrapper<object> jsCell = row.getCell(1);
            if (jsCell == null)
            {
                Formatter.exception(row.getCell(0), "Missing string to evaluate)");
                return;
            }
            JavascriptWrapper wrapper = new JavascriptWrapper(this);
            object result = null;
            try
            {
                result = wrapper.evaluateExpression(lastResponse, jsCell.body());
            }
            catch (JavascriptException e)
            {
                Formatter.exception(row.getCell(1), e);
                return;
            }
            lastEvaluation = null;
            if (result != null)
            {
                lastEvaluation = result.ToString();
            }
            StringTypeAdapter adapter = new StringTypeAdapter();
            adapter.set(lastEvaluation);
            Formatter.right(row.getCell(1), adapter);
        }

        /// <summary>
        /// Process the row in input. Abstracts the test runner via the wrapper
        /// interfaces.
        /// </summary>
        /// <param name="currentRow"> the current row </param>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("rawtypes") public void processRow(RowWrapper<?> currentRow)
        public virtual void processRow<T1>(RowWrapper<T1> currentRow)
        {
            row = currentRow;
            CellWrapper<object> cell0 = row.getCell(0);
            if (cell0 == null)
            {
                throw new Exception("Current RestFixture row is not parseable (maybe empty or not existent)");
            }
            string methodName = cell0.text();
            if ("".Equals(methodName))
            {
                throw new Exception("RestFixture method not specified");
            }
            Method method1;
            try
            {
                method1 = this.GetType().GetMethod(methodName);
                method1.invoke(this);
            }
            catch (SecurityException e)
            {
                throw new Exception("Not enough permissions to access method " + methodName + " for this class " + this.GetType().Name, e);
            }
            catch (NoSuchMethodException e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw new Exception("Class " + this.GetType().FullName + " doesn't have a callable method named " + methodName, e);
            }
            catch (System.ArgumentException e)
            {
                throw new Exception("Method named " + methodName + " invoked with the wrong argument.", e);
            }
            catch (IllegalAccessException e)
            {
                throw new Exception("Method named " + methodName + " is not public.", e);
            }
            catch (InvocationTargetException e)
            {
                throw new Exception("Method named " + methodName + " threw an exception when executing.", e);
            }
        }

        protected internal virtual void initialize(Runner runner)
        {
            this.runner = runner;
            bool state = validateState();
            notifyInvalidState(state);
            configFormatter();
            configFixture();
            configRestClient();
        }

        protected internal virtual string emptifyBody(string b)
        {
            string body = b;
            if (string.ReferenceEquals(body, null))
            {
                body = "";
            }
            return body;
        }

        /// <returns> the request headers </returns>
        public virtual IDictionary<string, string> Headers
        {
            get
            {
                IDictionary<string, string> headers = null;
                if (requestHeaders != null && !requestHeaders.Empty)
                {
                    headers = requestHeaders;
                }
                else
                {
                    headers = defaultHeaders;
                }
                return headers;
            }
        }

        // added for RestScriptFixture
        protected internal virtual string RequestBody
        {
            get
            {
                return requestBody;
            }
            set
            {
                requestBody = value;
            }
        }


        protected internal virtual IDictionary<string, string> NamespaceContext
        {
            get
            {
                return namespaceContext;
            }
        }

        private void doMethod(string m)
        {
            doMethod(null, m);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void doMethod(String body, String method)
        protected internal virtual void doMethod(string body, string method)
        {
            CellWrapper<object> urlCell = row.getCell(1);
            string url = deHtmlify(stripTag(urlCell.text()));
            string resUrl = GLOBALS.substitute(url);
            string rBody = GLOBALS.substitute(body);
            IDictionary<string, string> rHeaders = substitute(Headers);

            try
            {
                doMethod(method, resUrl, rHeaders, rBody);
                completeHttpMethodExecution();
            }
            catch (Exception e)
            {
                Formatter.exception(row.getCell(0), "Execution of " + method + " caused exception '" + e.Message + "'");
                LOG.error("Exception occurred when processing method=" + method, e);
            }
        }

        protected internal virtual void doMethod(string method, string resUrl, string rBody)
        {
            doMethod(method, resUrl, substitute(Headers), rBody);
        }

        protected internal virtual void doMethod(string method, string resUrl, IDictionary<string, string> headers, string rBody)
        {
            LastRequest = partsFactory.buildRestRequest();
            LastRequest.Method = RestRequest.Method.valueOf(method);
            LastRequest.addHeaders(headers);
            LastRequest.FollowRedirect = followRedirects;
            LastRequest.ResourceUriEscaped = resourceUrisAreEscaped;
            if (fileName != null)
            {
                LastRequest.FileName = fileName;
            }

            // Set multiFileName
            if (multipartFileName != null)
            {
                RestMultipart restMultipart = new RestMultipart(RestMultipart.RestMultipartType.FILE, multipartFileName);
                LastRequest.addMultipart(multipartFileParameterName, restMultipart);
            }

            // Add multiFileName
            if (!multiFileNameByParamName.Empty)
            {
                foreach (KeyValuePair<string, RestMultipart> entryMultipart in multiFileNameByParamName.entrySet())
                {
                    LastRequest.addMultipart(entryMultipart.Key, entryMultipart.Value);
                    LOG.debug(" addMultipart : paramName = {} , value = {}", entryMultipart.Key, entryMultipart.Value);
                }
            }

            string[] uri = resUrl.Split("\\?", 2);

            string[] thisRequestUrlParts = buildThisRequestUrl(uri[0]);
            LastRequest.Resource = thisRequestUrlParts[1];
            if (uri.Length > 1)
            {
                string query = uri[1];
                for (int i = 2; i < uri.Length; i++)
                {
                    query += "?" + uri[i]; //TODO: StringBuilder
                }
                LastRequest.Query = query;
            }
            if ("Post".Equals(method) || "Put".Equals(method))
            {
                LastRequest.Body = rBody;
            }

            //sglebs dirty workaround for #96
            configureCredentials();

            restClient.BaseUrl = thisRequestUrlParts[0];
            RestResponse response = restClient.execute(LastRequest);
            LastResponse = response;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) protected void completeHttpMethodExecution()
        protected internal virtual void completeHttpMethodExecution()
        {
            string uri = LastResponse.Resource;
            string query = LastRequest.Query;
            if (!string.ReferenceEquals(query, null) && !"".Equals(query.Trim()))
            {
                uri = uri + "?" + query;
            }
            string clientBaseUri = restClient.BaseUrl;
            string u = clientBaseUri + uri;
            CellWrapper<object> uriCell = row.getCell(1);
            Formatter.asLink(uriCell, GLOBALS.substitute(uriCell.body()), u, uri);
            CellWrapper<object> cellStatusCode = row.getCell(2);
            if (cellStatusCode == null)
            {
                throw new System.InvalidOperationException("You must specify a status code cell");
            }
            int? lastStatusCode = LastResponse.StatusCode;
            process(cellStatusCode, lastStatusCode.ToString(), new StatusCodeTypeAdapter());
            IList<Header> lastHeaders = LastResponse.Headers;
            process(row.getCell(3), lastHeaders, new HeadersTypeAdapter());
            CellWrapper<object> bodyCell = row.getCell(4);
            if (bodyCell == null)
            {
                throw new System.InvalidOperationException("You must specify a body cell");
            }
            bodyCell.body(GLOBALS.substitute(bodyCell.body()));
            BodyTypeAdapter bodyTypeAdapter = createBodyTypeAdapter();
            process(bodyCell, LastResponse.Body, bodyTypeAdapter);
        }

        // Split out of completeHttpMethodExecution so RestScriptFixture can call
        // this
        protected internal virtual BodyTypeAdapter createBodyTypeAdapter()
        {
            return createBodyTypeAdapter(ContentType.parse(LastResponse.ContentType));
        }

        // Split out of completeHttpMethodExecution so RestScriptFixture can call
        // this
        protected internal virtual BodyTypeAdapter createBodyTypeAdapter(ContentType ct)
        {
            string charset = LastResponse.Charset;
            BodyTypeAdapter bodyTypeAdapter = partsFactory.buildBodyTypeAdapter(ct, charset);
            bodyTypeAdapter.Context = namespaceContext;
            return bodyTypeAdapter;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) private void process(CellWrapper<object> expected, Object actual, RestDataTypeAdapter ta)
        private void process(CellWrapper expected, object actual, RestDataTypeAdapter ta)
        {
            if (expected == null)
            {
                throw new System.InvalidOperationException("You must specify a headers cell");
            }
            ta.set(actual);
            bool ignore = "".Equals(expected.text().Trim());
            if (ignore)
            {
                string actualString = ta.ToString();
                if (!"".Equals(actualString))
                {
                    expected.addToBody(Formatter.gray(actualString));
                }
            }
            else
            {
                bool success = false;
                try
                {
                    string substitute = GLOBALS.substitute(Tools.fromHtml(expected.text()));
                    object parse = ta.parse(substitute);
                    success = ta.Equals(parse, actual);
                }
                catch (Exception e)
                {
                    Formatter.exception(expected, e);
                    return;
                }
                if (success)
                {
                    Formatter.right(expected, ta);
                }
                else
                {
                    Formatter.wrong(expected, ta);
                }
            }
        }

        private void debugMethodCallStart()
        {
            debugMethodCall("=> ");
        }

        private void debugMethodCallEnd()
        {
            debugMethodCall("<= ");
        }

        private void debugMethodCall(string h)
        {
            if (debugMethodCall)
            {
                StackTraceElement el = Thread.CurrentThread.StackTrace[4];
                LOG.debug(h + el.MethodName);
            }
        }

        private IDictionary<string, string> substitute(IDictionary<string, string> headers)
        {
            IDictionary<string, string> sub = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> e in headers.SetOfKeyValuePairs())
            {
                sub[e.Key] = GLOBALS.substitute(e.Value);
            }
            return sub;
        }

        protected internal virtual RestResponse LastResponse
        {
            get
            {
                return lastResponse;
            }
            set
            {
                this.lastResponse = value;
            }
        }

        protected internal virtual RestRequest LastRequest
        {
            get
            {
                return lastRequest;
            }
            set
            {
                this.lastRequest = value;
            }
        }

        private string[] buildThisRequestUrl(string uri)
        {
            string[] parts = new string[2];
            if (baseUrl == null || uri.StartsWith(baseUrl.ToString(), StringComparison.Ordinal))
            {
                Url url = new Url(uri);
                parts[0] = url.BaseUrl;
                parts[1] = url.Resource;
            }
            else
            {
                try
                {
                    Url attempted = new Url(uri);
                    parts[0] = attempted.BaseUrl;
                    parts[1] = attempted.Resource;
                }
                catch (Exception)
                {
                    parts[0] = baseUrl.ToString();
                    parts[1] = uri;

                }
            }
            return parts;
        }



        protected internal virtual LinkedHashMap<string, string> parseHeaders(string str)
        {
            return Tools.convertStringToMap(str, ":", LINE_SEPARATOR, true);
        }

        private IDictionary<string, string> parseNamespaceContext(string str)
        {
            return Tools.convertStringToMap(str, "=", LINE_SEPARATOR, true);
        }

        private string stripTag(string somethingWithinATag)
        {
            return Tools.fromSimpleTag(somethingWithinATag);
        }

        private void configFormatter()
        {
            formatter = partsFactory.buildCellFormatter(runner);
        }

        /// <summary>
        /// Configure the fixture with data from <seealso cref="RestFixtureConfig"/>.
        /// </summary>
        private void configFixture()
        {

            GLOBALS = createRunnerVariables();

            displayActualOnRight = config.getAsBoolean("restfixture.display.actual.on.right", displayActualOnRight);

            displayAbsoluteURLInFull = config.getAsBoolean("restfixture.display.absolute.url.in.full", displayAbsoluteURLInFull);

            resourceUrisAreEscaped = config.getAsBoolean("restfixture.resource.uris.are.escaped", resourceUrisAreEscaped);

            followRedirects = config.getAsBoolean("restfixture.requests.follow.redirects", followRedirects);

            minLenForCollapseToggle = config.getAsInteger("restfixture.display.toggle.for.cells.larger.than", minLenForCollapseToggle);

            string str = config.get("restfixture.default.headers", "");
            defaultHeaders = parseHeaders(str);

            str = config.get("restfixture.xml.namespace.context", "");
            namespaceContext = parseNamespaceContext(str);

            ContentType.resetDefaultMapping();
            ContentType.config(config);
        }

        /// <summary>
        /// Allows to config the rest client implementation. the method shoudl
        /// configure the instance attribute <seealso cref="RestFixture#restClient"/> created
        /// by the <seealso cref="smartrics.rest.fitnesse.fixture.PartsFactory#buildRestClient(smartrics.rest.fitnesse.fixture.support.Config)"/>.
        /// </summary>
        private void configRestClient()
        {
            restClient = partsFactory.buildRestClient(Config);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) private void renderReplacement(CellWrapper<object> cell, String actual)
        private void renderReplacement(CellWrapper<object> cell, string actual)
        {
            StringTypeAdapter adapter = new StringTypeAdapter();
            adapter.set(actual);
            if (!adapter.Equals(actual, cell.body()))
            {
                // eg - a substitution has occurred
                cell.body(actual);
                Formatter.right(cell, adapter);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) private void processSlimRow(List<List<String>> resultTable, List<String> row)
        private void processSlimRow(IList<IList<string>> resultTable, IList<string> row)
        {
            RowWrapper currentRow = new SlimRow(row);
            try
            {
                processRow(currentRow);
            }
            catch (Exception e)
            {
                LOG.error("Exception raised when processing row " + row[0], e);
                Formatter.exception(currentRow.getCell(0), e);
            }
            finally
            {
                IList<string> rowAsList = mapSlimRow(row, currentRow);
                resultTable.Add(rowAsList);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("rawtypes") private List<String> mapSlimRow(List<String> resultRow, RowWrapper currentRow)
        private IList<string> mapSlimRow(IList<string> resultRow, RowWrapper currentRow)
        {
            IList<string> rowAsList = ((SlimRow)currentRow).asList();
            for (int c = 0; c < rowAsList.Count; c++)
            {
                // HACK: it seems that even if the content is unchanged,
                // Slim renders red cell
                string v = rowAsList[c];
                if (v.Equals(resultRow[c]))
                {
                    rowAsList[c] = "";
                }
            }
            return rowAsList;
        }

        private string deHtmlify(string someHtml)
        {
            return Tools.fromHtml(someHtml);
        }

        private void configureCredentials()
        {
            string username = config.get("http.basicauth.username");
            string password = config.get("http.basicauth.password");
            if (!string.ReferenceEquals(username, null) && !string.ReferenceEquals(password, null))
            {
                string newUsername = GLOBALS.substitute(username);
                string newPassword = GLOBALS.substitute(password);
                Config newConfig = Config;
                newConfig.add("http.basicauth.username", newUsername);
                newConfig.add("http.basicauth.password", newPassword);
                restClient = partsFactory.buildRestClient(newConfig);
            }
        }

        //public override StatementExecutorInterface StatementExecutor
        //{
        //    set
        //    {
        //        this.slimStatementExecutor = value;
        //    }
        //}
}