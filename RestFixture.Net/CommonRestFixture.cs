using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using fitSharp.Machine.Engine;
using NLog;
using restFixture.Net.Support;
using RestClient;
using RestClient.Data;

namespace restFixture.Net
{
    /// <summary>
    /// Common functionality shared between Slim and Fit implementations of the Rest Fixture.
    /// </summary>
    /// <remarks>Not named BaseRestFixture as FitRestFixture must inherit ActionFixture, one of the 
    /// standard Fit fixture classes, rather than this class.</remarks>
    public class CommonRestFixture<T> : IRunnerVariablesProvider
    {
         //TODO: Fix this.  Should be reading the FitNesse version, not the FitSharp one.  
        //  See original Java code.
        private static Version _fitSharpVersion = Assembly.GetEntryAssembly().GetName().Version;

        private static NLog.Logger LOG = LogManager.GetCurrentClassLogger();

        public Variables CreateRunnerVariables()
        {
            switch (runner)
            {
                case Runner.SLIM:
                    //return new SlimVariables(config, slimStatementExecutor);
                    return new SlimVariables(config);
                case Runner.FIT:
                    return new FitVariables(config, _symbols);
                default:
                    // Use FitVariables for tests
                    return new FitVariables(config, _symbols);
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

        protected internal LinkedHashMap<string, RestMultipart> multiFileNameByParamName =
            new LinkedHashMap<string, RestMultipart>();

        protected internal string requestBody;

        protected internal bool resourceUrisAreEscaped = false;

        protected internal IDictionary<string, string> requestHeaders;

        private IRestClient restClient;

        private Config config;

        private Runner runner;

        public bool displayActualOnRight;

        public bool displayAbsoluteURLInFull;

        private bool _debugMethodCall = false;

        /// <summary>
        /// the headers passed to each request by default.
        /// </summary>
        private IDictionary<string, string> defaultHeaders = new Dictionary<string, string>();

        private IDictionary<string, string> namespaceContext = new Dictionary<string, string>();

        protected Url _baseUrl;

        protected internal IRowWrapper<T> row;

        private ICellFormatter<T> formatter;

        private PartsFactory partsFactory;

        private string lastEvaluation;

        public int minLenForCollapseToggle;

        private bool followRedirects = true;

        //private StatementExecutorInterface slimStatementExecutor;

        private Symbols _symbols = null;

        #region Constructors ******************************************************************************

        static CommonRestFixture()
        {
            LOG.Info("############ Detected FitNesse version: {} ###########", _fitSharpVersion);
        }

        /// <summary>
        /// Constructor for Fit runner.
        /// </summary>
        public CommonRestFixture(Symbols symbols) : base()
        {
            this._symbols = symbols;
            this.partsFactory = new PartsFactory(this, Net.Support.Config.getConfig(Config.DEFAULT_CONFIG_NAME));
            this.displayActualOnRight = true;
            this.minLenForCollapseToggle = -1;
            this.resourceUrisAreEscaped = false;
            this.displayAbsoluteURLInFull = true;
            this.requestHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructor for Slim runner.
        /// </summary>
        /// <param name="hostName">
        ///            the cells following up the first cell in the first row. </param>
        public CommonRestFixture(string hostName) : this(hostName, Config.DEFAULT_CONFIG_NAME)
        {
        }

        /// <summary>
        /// Constructor for Slim runner.
        /// </summary>
        /// <param name="hostName">
        ///            the cells following up the first cell in the first row. </param>
        /// <param name="configName">
        ///            the value of cell number 3 in first row of the fixture table. </param>
        public CommonRestFixture(string hostName, string configName)
        {
            this.displayActualOnRight = true;
            this.minLenForCollapseToggle = -1;
            this.resourceUrisAreEscaped = false;
            this.displayAbsoluteURLInFull = true;
            this.config = Config.getConfig(configName);
            this.partsFactory = new PartsFactory(this, config);
            this._baseUrl = new Url(stripTag(hostName));
            this.requestHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// VisibleForTesting </summary>
        /// <param name="partsFactory">
        ///            the factory of parts necessary to create the rest fixture </param>
        /// <param name="hostName"> </param>
        /// <param name="configName"> </param>
        internal CommonRestFixture(PartsFactory partsFactory, string hostName, string configName)
        {
            this.displayActualOnRight = true;
            this.minLenForCollapseToggle = -1;
            this.resourceUrisAreEscaped = false;
            this.displayAbsoluteURLInFull = true;
            this.partsFactory = partsFactory;
            this.config = Config.getConfig(configName);
            this._baseUrl = new Url(stripTag(hostName));
            this.requestHeaders = new Dictionary<string, string>();
        }

        #endregion

        /// <returns> the config used for this fixture instance </returns>
        public virtual Config Config
        {
            get { return config; }
            set { this.config = value; }
        }

        /// <returns> the result of the last evaluation performed via evalJs. </returns>
        public virtual string LastEvaluation
        {
            get { return lastEvaluation; }
        }

        public virtual Url BaseUrl
        {
            get { return _baseUrl; }

            set { this._baseUrl = value; }
        }

        public virtual string BaseUrlString
        {
            get
            {
                if (_baseUrl != null)
                {
                    return _baseUrl.ToString();
                }
                return null;
            }

            set { this.BaseUrl = new Url(value); }
        }

        /// <summary>
        /// The default headers as defined in the config used to initialise this
        /// fixture.
        /// </summary>
        /// <returns> the map of default headers. </returns>
        public virtual IDictionary<string, string> DefaultHeaders
        {
            get { return defaultHeaders; }
        }

        /// <summary>
        /// The formatter for this instance of the CommonRestFixture.
        /// </summary>
        /// <returns> the formatter for the cells </returns>
        public virtual ICellFormatter<T> Formatter
        {
            get { return formatter; }
        }

        /// <summary>
        /// Slim Table table hook.
        /// </summary>
        /// <param name="rows"> the rows to process </param>
        /// <returns> the rendered content. </returns>
        //public virtual IList<IList<string>> doTable(IList<IList<string>> rows)
        //{
        //    initialize(CommonRestFixture.Runner.SLIM);
        //    IList<IList<string>> res = new List<IList<string>>();
        //    Formatter.DisplayActual = displayActualOnRight;
        //    Formatter.DisplayAbsoluteURLInFull = displayAbsoluteURLInFull;
        //    Formatter.MinLengthForToggleCollapse = minLenForCollapseToggle;
        //    foreach (IList<string> r in rows)
        //    {
        //        processSlimRow(res, r);
        //    }
        //    return res;
        //}

        /// <summary>
        /// Overrideable method to validate the state of the instance in execution. A
        /// <seealso cref="CommonRestFixture{T}"/> is valid if the baseUrl is not null.
        /// </summary>
        /// <returns> true if the state is valid, false otherwise </returns>
        protected internal virtual bool ValidateState()
        {
            return _baseUrl != null;
        }


        /// <summary>
        /// Method invoked to notify that the state of the CommonRestFixture is invalid. It
        /// throws a <seealso cref="RuntimeException"/> with a message displayed in the
        /// FitNesse page.
        /// </summary>
        /// <param name="state">
        ///            as returned by <seealso cref="CommonRestFixture{T}#ValidateState()"/> </param>
        protected internal virtual void NotifyInvalidState(bool state)
        {
            if (!state)
            {
                throw new Exception("You must specify a base url in the |start|, after the fixture to start");
            }
        }

        #region Multi-part File Upload ********************************************************************

        /// <summary>
        /// Allows setting of the name of the multi-part file to upload.
        /// <para>
        /// <code>| setMultipartFileName | Name of file |</code>
        /// </para>
        /// <para>
        /// body text should be location of file which needs to be sent
        /// </para>
        /// </summary>
        public virtual void SetMultipartFileName()
        {
            ICellWrapper<T> cell = row.getCell(1);
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
        public virtual void AddMultipartFile()
        {
            ICellWrapper<T> cellFileName = row.getCell(1);
            ICellWrapper<T> cellParamName = row.getCell(2);
            ICellWrapper<T> cellContentType = row.getCell(3);
            ICellWrapper<T> cellCharset = row.getCell(4);
            if (cellFileName == null)
            {
                Formatter.exception(row.getCell(0), "You must pass a multipart file name to set");
            }
            else
            {
                RegisterMultipartRow(RestMultipart.RestMultipartType.FILE, cellFileName, cellParamName, cellContentType,
                    cellCharset);
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
        public virtual void AddMultipartString()
        {
            ICellWrapper<T> cellFileName = row.getCell(1);
            ICellWrapper<T> cellParamName = row.getCell(2);
            ICellWrapper<T> cellContentType = row.getCell(3);
            ICellWrapper<T> cellCharset = row.getCell(4);
            if (cellFileName == null)
            {
                Formatter.exception(row.getCell(0), "You must pass a multipart string content to set");
            }
            else
            {
                RegisterMultipartRow(RestMultipart.RestMultipartType.STRING, cellFileName, cellParamName,
                    cellContentType, cellCharset);
            }
        }


        private RestMultipart RegisterMultipartRow(RestMultipart.RestMultipartType type, ICellWrapper<T> cellFileName,
            ICellWrapper<T> cellParamName, ICellWrapper<T> cellContentType, ICellWrapper<T> cellCharset)
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
            RestMultipart restMultipart = new RestMultipart(type, multipartFileName, multipartContentType,
                multipartCharSet);
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
            get { return multipartFileName; }
        }

        /// <summary>
        /// The name of the file to upload.
        /// </summary>
        /// <code>| setFileName | Name of file |</code>
        /// <remarks>body text should be location of file which needs to be sent</remarks>
        public virtual string FileName
        {
            get { return fileName; }
            set
            {
                ICellWrapper<T> cell = row.getCell(1);
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
        public virtual void setMultipartFileParameterName()
        {
            ICellWrapper<T> cell = row.getCell(1);
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
            get { return multipartFileParameterName; }
        }

        #endregion

        /// <summary>
        /// <code>| setBody | body text goes here |</code>
        /// <para>
        /// body text can either be a kvp or a xml. The <code>ClientHelper</code>
        /// will figure it out
        /// </para>
        /// </summary>
        public virtual void setBody()
        {
            ICellWrapper<T> cell = row.getCell(1);
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
        public virtual void setHeader()
        {
            requestHeaders.Clear();
            addHeader();
        }

        public virtual void addHeader()
        {
            ICellWrapper<T> cell = row.getCell(1);
            if (cell == null)
            {
                Formatter.exception(row.getCell(0), "You must pass a header map to set");
            }
            else
            {
                string substitutedHeaders = GLOBALS.substitute(cell.text());
                IDictionary<string, string> parsedHeaders = parseHeaders(substitutedHeaders);
                foreach (string key in parsedHeaders.Keys)
                {
                    requestHeaders[key] = parsedHeaders[key];
                }
                cell.body(Formatter.gray(substitutedHeaders));
            }
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> addHeader(string headers)
        {
            string substitutedHeaders = headers;
            if (GLOBALS != null)
            {
                substitutedHeaders = GLOBALS.substitute(headers);
            }
            IDictionary<string, string> parsedHeaders = parseHeaders(substitutedHeaders);
            foreach (string key in parsedHeaders.Keys)
            {
                requestHeaders[key] = parsedHeaders[key];
            }
            return requestHeaders;
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> setHeader(string headers)
        {
            requestHeaders = new Dictionary<string, string>();
            return addHeader(headers);
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> setHeaders(string headers)
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
        public virtual void let()
        {
            debugMethodCallStart();
            if (row.size() != 5)
            {
                Formatter.exception(row.getCell(row.size() - 1),
                    "Not all cells found: | let | label | type | expr | result |");
                debugMethodCallEnd();
                return;
            }
            string label = row.getCell(1).text().Trim();
            string loc = row.getCell(2).text();
            ICellWrapper<T> exprCell = row.getCell(3);
            try
            {
                exprCell.body(GLOBALS.substitute(exprCell.body()));
                string expr = exprCell.text();
                ICellWrapper<T> valueCell = row.getCell(4);
                string valueCellText = valueCell.body();
                string valueCellTextReplaced = GLOBALS.substitute(valueCellText);
                valueCell.body(valueCellTextReplaced);
                string sValue = null;
                ILetHandler letHandler = LetHandlerFactory.getHandlerFor(loc);
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
                    adapter.Actual = sValue;
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
        public virtual void comment()
        {
            debugMethodCallStart();
            ICellWrapper<T> messageCell = row.getCell(1);
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
        public virtual void evalJs()
        {
            ICellWrapper<T> jsCell = row.getCell(1);
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
            adapter.Actual = lastEvaluation;
            Formatter.right(row.getCell(1), adapter);
        }

        /// <summary>
        /// Process the row in input. Abstracts the test runner via the wrapper
        /// interfaces.
        /// </summary>
        /// <param name="currentRow"> the current row </param>
        public virtual void processRow(IRowWrapper<T> currentRow)
        {
            row = currentRow;
            ICellWrapper<T> cell0 = row.getCell(0);
            if (cell0 == null)
            {
                throw new Exception("Current RestFixture row is not parseable (maybe empty or not existent)");
            }
            string methodName = cell0.text();
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Exception("RestFixture method not specified");
            }
            MethodInfo method1;
            try
            {
                // The method cannot take any parameters.
                method1 = this.GetType().GetMethod(methodName, new Type[0]);
                method1.Invoke(this, new object[0]);
            }
            catch (SecurityException e)
            {
                throw new Exception(
                    "Not enough permissions to access method " + methodName + " for this class " + this.GetType().Name,
                    e);
            }
            catch (TargetException e)
            {
                throw new Exception(
                    "Class " + this.GetType().FullName + " doesn't have a callable method named " + methodName, e);
            }
            catch (System.ArgumentException e)
            {
                throw new Exception("Method named " + methodName + " invoked with the wrong argument.", e);
            }
            catch (MethodAccessException e)
            {
                throw new Exception("Method named " + methodName + " is not public.", e);
            }
            catch (TargetInvocationException e)
            {
                throw new Exception("Method named " + methodName + " threw an exception when executing.", e);
            }
        }

        protected internal virtual void initialize(Runner runner)
        {
            this.runner = runner;
            bool state = ValidateState();
            NotifyInvalidState(state);
            configFormatter();
            configFixture();
            configRestClient();
        }

        protected internal virtual string emptifyBody(string b)
        {
            string body = b;
            if (body == null)
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
                if (requestHeaders != null && requestHeaders.Count > 0)
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
            get { return requestBody; }
            set { requestBody = value; }
        }


        protected internal virtual IDictionary<string, string> NamespaceContext
        {
            get { return namespaceContext; }
        }

        private void doMethod(string m)
        {
            doMethod(null, m);
        }

        protected internal virtual void doMethod(string body, string method)
        {
            ICellWrapper<T> urlCell = row.getCell(1);
            string url = deHtmlify(stripTag(urlCell.text()));
            string resUrl = GLOBALS.substitute(url);
            string rBody = GLOBALS.substitute(body);
            IDictionary<string, string> rHeaders = substitute(Headers);

            try
            {
                doMethod(method, resUrl, rHeaders, rBody);
                string clientBaseUrl = restClient.BaseUrlString;
                string lastRequestUrl = LastResponse.Resource;
                string lastRequestQueryString = LastRequest.Query;
                completeHttpMethodExecution(clientBaseUrl, lastRequestUrl, lastRequestQueryString);
            }
            catch (Exception e)
            {
                Formatter.exception(row.getCell(0), "Execution of " + method + " caused exception '" + e.Message + "'");
                LOG.Error(e, "Exception occurred when processing method={0}", method);
            }
        }

        protected internal virtual void doMethod(string method, string resUrl, string rBody)
        {
            doMethod(method, resUrl, substitute(Headers), rBody);
        }

        protected internal virtual void doMethod(string method, string resUrl, 
            IDictionary<string, string> headers, string rBody)
        {
            LastRequest = partsFactory.buildRestRequest();
            RestRequest.Method parsedMethod;
            bool ignoreCase = false;
            if (!Enum.TryParse(method, ignoreCase, out parsedMethod))
            {
                parsedMethod = RestRequest.Method.Get;
            }
            LastRequest.HttpMethod = parsedMethod;
            LastRequest.addHeaders(headers);
            LastRequest.FollowRedirect = followRedirects;
            LastRequest.IsResourceUriEscaped = resourceUrisAreEscaped;
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
            if (!multiFileNameByParamName.IsEmpty())
            {
                foreach (KeyValuePair<string, RestMultipart> entryMultipart in multiFileNameByParamName)
                {
                    LastRequest.addMultipart(entryMultipart.Key, entryMultipart.Value);
                    LOG.Debug(" addMultipart : paramName = {} , value = {}", entryMultipart.Key, entryMultipart.Value);
                }
            }

            string[] uri = resUrl.Split(new char[] {'?'}, 2);

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
            if ("Post".Equals(method, StringComparison.CurrentCultureIgnoreCase) 
                || "Put".Equals(method, StringComparison.CurrentCultureIgnoreCase))
            {
                LastRequest.Body = rBody;
            }

            //sglebs dirty workaround for #96
            configureCredentials();

            restClient.BaseUrlString = thisRequestUrlParts[0];
            RestResponse response = restClient.Execute(LastRequest);
            LastResponse = response;
        }

        protected internal virtual void completeHttpMethodExecution(string clientBaseUrl, 
            string lastRequestUrl, string lastRequestQueryString)
        {
            //string uri = LastResponse.Resource;
            //string query = LastRequest.Query;
            if (lastRequestQueryString != null && lastRequestQueryString.Trim() != "")
            {
                lastRequestUrl = lastRequestUrl + "?" + lastRequestQueryString;
            }
            //string clientBaseUri = restClient.BaseUrlString;
            string u = clientBaseUrl + lastRequestUrl;
            ICellWrapper<T> uriCell = row.getCell(1);
            Formatter.asLink(uriCell, GLOBALS.substitute(uriCell.body()), u, lastRequestUrl);
            ICellWrapper<T> cellStatusCode = row.getCell(2);
            if (cellStatusCode == null)
            {
                throw new System.InvalidOperationException("You must specify a status code cell");
            }
            int? lastStatusCode = LastResponse.StatusCode;
            process(cellStatusCode, lastStatusCode.ToString(), new StatusCodeTypeAdapter());
            IList<RestData.Header> lastHeaders = LastResponse.Headers;
            process(row.getCell(3), lastHeaders, new HeadersTypeAdapter());
            ICellWrapper<T> bodyCell = row.getCell(4);
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

        private void process(ICellWrapper<T> expected, object actual, RestDataTypeAdapter ta)
        {
            if (expected == null)
            {
                throw new System.InvalidOperationException("You must specify a headers cell");
            }
            ta.Actual = actual;
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
            if (_debugMethodCall)
            {
                StackTrace stackTrace = new StackTrace(false);
                int i = 0;
                StackFrame stackFrame = stackTrace.GetFrame(i);
                MethodBase methodBase = stackFrame.GetMethod();
                bool ignoreCase = true;
                while (i < stackTrace.FrameCount && methodBase.Name.StartsWith("debugMethodCall",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    i++;
                    stackFrame = stackTrace.GetFrame(i);
                    methodBase = stackFrame.GetMethod();
                }

                string methodName = methodBase.Name;
                LOG.Debug(h + methodName);
            }
        }

        private IDictionary<string, string> substitute(IDictionary<string, string> headers)
        {
            IDictionary<string, string> sub = new Dictionary<string, string>();
            foreach (string key in headers.Keys)
            {
                sub[key] = GLOBALS.substitute(headers[key]);
            }
            return sub;
        }

        protected internal virtual RestResponse LastResponse
        {
            get { return lastResponse; }
            set { this.lastResponse = value; }
        }

        protected internal virtual RestRequest LastRequest
        {
            get { return lastRequest; }
            set { this.lastRequest = value; }
        }

        private string[] buildThisRequestUrl(string uri)
        {
            string[] parts = new string[2];
            if (BaseUrlString == null || uri.StartsWith(BaseUrlString,
                StringComparison.OrdinalIgnoreCase))
            {
                Url url = new Url(uri);
                parts[0] = url.BaseUrlString;
                parts[1] = url.Resource;
            }
            else
            {
                try
                {
                    Url attempted = new Url(uri);
                    parts[0] = attempted.BaseUrlString;
                    parts[1] = attempted.Resource;
                }
                catch (Exception)
                {
                    parts[0] = BaseUrlString;
                    parts[1] = uri;

                }
            }
            return parts;
        }



        protected internal virtual IDictionary<string, string> parseHeaders(string str)
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
            formatter = partsFactory.buildCellFormatter<T>(runner);
        }

        /// <summary>
        /// Configure the fixture with data from <seealso cref="RestFixtureConfig"/>.
        /// </summary>
        private void configFixture()
        {

            GLOBALS = CreateRunnerVariables();

            displayActualOnRight = config.getAsBoolean("restfixture.display.actual.on.right", displayActualOnRight);

            displayAbsoluteURLInFull = config.getAsBoolean("restfixture.display.absolute.url.in.full",
                displayAbsoluteURLInFull);

            resourceUrisAreEscaped = config.getAsBoolean("restfixture.resource.uris.are.escaped", resourceUrisAreEscaped);

            followRedirects = config.getAsBoolean("restfixture.requests.follow.redirects", followRedirects);

            minLenForCollapseToggle = config.getAsInteger("restfixture.display.toggle.for.cells.larger.than",
                minLenForCollapseToggle);

            string str = config.get("restfixture.default.headers", "");
            defaultHeaders = parseHeaders(str);

            str = config.get("restfixture.xml.namespace.context", "");
            namespaceContext = parseNamespaceContext(str);

            ContentType.resetDefaultMapping();
            ContentType.config(config);
        }

        /// <summary>
        /// Allows to config the rest client implementation. the method shoudl
        /// configure the instance attribute <seealso cref="CommonRestFixture{T}#restClient"/> created
        /// by the <seealso cref="PartsFactory#buildRestClient(smartrics.rest.fitnesse.fixture.support.Config)"/>.
        /// </summary>
        private void configRestClient()
        {
            restClient = partsFactory.buildRestClient(Config);
        }

        private void renderReplacement(ICellWrapper<T> cell, string actual)
        {
            StringTypeAdapter adapter = new StringTypeAdapter();
            adapter.Actual = actual;
            if (!adapter.Equals(actual, cell.body()))
            {
                // eg - a substitution has occurred
                cell.body(actual);
                Formatter.right(cell, adapter);
            }
        }

        //private void processSlimRow(IList<IList<string>> resultTable, IList<string> row)
        //{
        //    IRowWrapper currentRow = new SlimRow(row);
        //    try
        //    {
        //        processRow(currentRow);
        //    }
        //    catch (Exception e)
        //    {
        //        LOG.Error(e, "Exception raised when processing row {0}", row[0]);
        //        Formatter.exception(currentRow.getCell(0), e);
        //    }
        //    finally
        //    {
        //        IList<string> rowAsList = mapSlimRow(row, currentRow);
        //        resultTable.Add(rowAsList);
        //    }
        //}

        //private IList<string> mapSlimRow(IList<string> resultRow, IRowWrapper currentRow)
        //{
        //    IList<string> rowAsList = ((SlimRow) currentRow).asList();
        //    for (int c = 0; c < rowAsList.Count; c++)
        //    {
        //        // HACK: it seems that even if the content is unchanged,
        //        // Slim renders red cell
        //        string v = rowAsList[c];
        //        if (v.Equals(resultRow[c]))
        //        {
        //            rowAsList[c] = "";
        //        }
        //    }
        //    return rowAsList;
        //}

        private string deHtmlify(string someHtml)
        {
            return Tools.fromHtml(someHtml);
        }

        private void configureCredentials()
        {
            string username = config.get("http.basicauth.username");
            string password = config.get("http.basicauth.password");
            if (username != null && password != null)
            {
                string newUsername = GLOBALS.substitute(username);
                string newPassword = GLOBALS.substitute(password);
                Config newConfig = Config;
                newConfig.add("http.basicauth.username", newUsername);
                newConfig.add("http.basicauth.password", newPassword);
                restClient = partsFactory.buildRestClient(newConfig);
            }
        }
    }
}