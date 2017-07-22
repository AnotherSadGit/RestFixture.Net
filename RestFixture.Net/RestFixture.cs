using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
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
    /// <td>http.proxy.username</td>
    /// <td><i>http proxy credentials username (RestClient proxy configuration).  
    /// NOTE: This parameter was introduced in RestFixture.Net, it does not exist in the original 
    /// Java RestFixture.</i></td>
    /// </tr>
    /// <tr>
    /// <td>http.proxy.password</td>
    /// <td><i>http proxy credentials password (RestClient proxy configuration).  
    /// NOTE: This parameter was introduced in RestFixture.Net, it does not exist in the original 
    /// Java RestFixture.</i></td>
    /// </tr>
    /// <tr>
    /// <td>http.proxy.domain</td>
    /// <td><i>http proxy credentials domain (RestClient proxy configuration).  
    /// NOTE: This parameter was introduced in RestFixture.Net, it does not exist in the original 
    /// Java RestFixture.</i></td>
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
    /// false.  NOTE: This parameter is supported only in the Java implementation 
    /// of RestFixture.  It is not supported in RestFixture.Net.</i></td>
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
    /// false.  NOTE: This parameter is supported only in the Java implementation 
    /// of RestFixture.  It is not supported in RestFixture.Net because uris are 
    /// escaped automatically, if required, by the .NET framework.</i></td>
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
    public class RestFixture
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private CommonRestFixture _restFixture;

        #region Constructors ******************************************************************************

        /// <summary>
        /// Constructor for Slim runner.
        /// </summary>
        /// <param name="hostName">
        ///            the cells following up the first cell in the first row. </param>
        public RestFixture(string hostName)
        {
            _restFixture = new CommonRestFixture(hostName);
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
            _restFixture = new CommonRestFixture(hostName, configName);
        }

        /// <summary>
        /// VisibleForTesting </summary>
        /// <param name="partsFactory">
        ///            the factory of parts necessary to create the rest fixture </param>
        /// <param name="hostName"> </param>
        /// <param name="configName"> </param>
        internal RestFixture(PartsFactory partsFactory, string hostName, string configName)
        {
            _restFixture = new CommonRestFixture(partsFactory, hostName, configName);
        }

        #endregion

        /// <summary>
        /// Slim Table table hook.
        /// </summary>
        /// <param name="rows"> the rows to process </param>
        /// <returns> the rendered content. </returns>
        public virtual IList<IList<string>> doTable(IList<IList<string>> rows)
        {
            _restFixture.initialize(CommonRestFixture.Runner.SLIM);
            IList<IList<string>> res = new List<IList<string>>();
            _restFixture.Formatter.DisplayActual = _restFixture.displayActualOnRight;
            _restFixture.Formatter.DisplayAbsoluteURLInFull = _restFixture.displayAbsoluteURLInFull;
            _restFixture.Formatter.MinLengthForToggleCollapse = _restFixture.minLenForCollapseToggle;
            foreach (IList<string> r in rows)
            {
                processSlimRow(res, r);
            }
            return res;
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
        public virtual void SetMultipartFileName()
        {
            _restFixture.SetMultipartFileName();
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
            _restFixture.AddMultipartFile();
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
            _restFixture.AddMultipartString();
        }

        /// <returns> the multipart filename </returns>
        public virtual string MultipartFileName
        {
            get { return _restFixture.multipartFileName; }
        }

        /// <summary>
        /// The name of the file to upload.
        /// </summary>
        /// <code>| setFileName | Name of file |</code>
        /// <remarks>body text should be location of file which needs to be sent</remarks>
        public virtual string FileName
        {
            get
            {
                return _restFixture.FileName;
            }

            set
            {
                _restFixture.FileName = value;
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
            _restFixture.setMultipartFileParameterName();
        }

        /// <returns> the multipart file parameter name. </returns>
        public virtual string MultipartFileParameterName
        {
            get { return _restFixture.MultipartFileParameterName;; }
        }

        /// <summary>
        /// <code>| setBody | body text goes here |</code>
        /// <para>
        /// body text can either be a kvp or a xml. The <code>ClientHelper</code>
        /// will figure it out
        /// </para>
        /// </summary>
        public virtual void setBody()
        {
            _restFixture.setBody();
        }

        /// <summary>
        /// \@sglebs - fixes #162. necessary to work with a scenario </summary>
        /// <param name="body"> the body to set </param>
        /// <returns> the body after substitution </returns>
        public virtual string setBody(string body)
        {
            return _restFixture.setBody(body);
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
            _restFixture.setHeader();
        }

        public virtual void addHeader()
        {
            _restFixture.addHeader();
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> addHeader(string headers)
        {
            return _restFixture.addHeader(headers);
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> setHeader(string headers)
        {
            return _restFixture.setHeader(headers);
        }

        /// <summary>
        /// \@sglebs - fixes #161. necessary to work with a scenario </summary>
        /// <param name="headers"> the headers string to set </param>
        /// <returns> the headers map </returns>
        public virtual IDictionary<string, string> setHeaders(string headers)
        {
            return _restFixture.setHeaders(headers);
        }

        /// <summary>
        /// Equivalent to setHeader - syntactic sugar to indicate that you can now.
        /// <para>
        /// set multiple headers in a single call
        /// </para>
        /// </summary>
        public virtual void setHeaders()
        {
            _restFixture.setHeaders();
        }

        public virtual void addHeaders()
        {
            _restFixture.addHeaders();
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
            _restFixture.PUT();
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
            _restFixture.GET();
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
            _restFixture.HEAD();
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
            _restFixture.OPTIONS();
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
            _restFixture.DELETE();
        }

        /// <summary>
        /// <code> | TRACE | uri | ?ret | ?headers | ?body |</code>
        /// </summary>
        public virtual void TRACE()
        {
            _restFixture.TRACE();
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
            _restFixture.POST();
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
            _restFixture.let();
        }

        /// <summary>
        /// allows to add comments to a rest fixture - basically does nothing except ignoring the text.
        /// the text is substituted if variables are found.
        /// </summary>
        public virtual void comment()
        {
            _restFixture.comment();
        }

        /// <summary>
        /// Evaluates a string using the internal JavaScript engine. Result of the
        /// last evaluation is set in the attribute lastEvaluation.
        /// 
        /// </summary>
        public virtual void evalJs()
        {
            _restFixture.evalJs();
        }

        /// <summary>
        /// Process the row in input. Abstracts the test runner via the wrapper
        /// interfaces.
        /// </summary>
        /// <param name="currentRow"> the current row </param>
        public virtual void processRow(IRowWrapper currentRow)
        {
            _restFixture.processRow(currentRow);
        }

        /// <returns> the request headers </returns>
        public virtual IDictionary<string, string> Headers
        {
            get
            {
                return _restFixture.Headers;
            }
        }

        // added for RestScriptFixture
        protected internal virtual string RequestBody
        {
            get { return _restFixture.RequestBody; }
            set { _restFixture.RequestBody = value; }
        }

        private void processSlimRow(IList<IList<string>> resultTable, IList<string> row)
        {
            IRowWrapper currentRow = new SlimRow(row);
            try
            {
                processRow(currentRow);
            }
            catch (Exception e)
            {
                LOG.Error(e, "Exception raised when processing row {0}", row[0]);
                _restFixture.Formatter.exception(currentRow.getCell(0), e);
            }
            finally
            {
                IList<string> rowAsList = mapSlimRow(row, currentRow);
                resultTable.Add(rowAsList);
            }
        }

        private IList<string> mapSlimRow(IList<string> resultRow, IRowWrapper currentRow)
        {
            IList<string> rowAsList = ((SlimRow) currentRow).asList();
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
    }
}