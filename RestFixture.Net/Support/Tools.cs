using System;
using System.Collections.Generic;
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
namespace RestFixture.Net.Support
{

	using HierarchicalStreamDriver = com.thoughtworks.xstream.io.HierarchicalStreamDriver;
	using HierarchicalStreamReader = com.thoughtworks.xstream.io.HierarchicalStreamReader;
	using HierarchicalStreamCopier = com.thoughtworks.xstream.io.copy.HierarchicalStreamCopier;
	using JettisonMappedXmlDriver = com.thoughtworks.xstream.io.json.JettisonMappedXmlDriver;
	using PrettyPrintWriter = com.thoughtworks.xstream.io.xml.PrettyPrintWriter;
	using JSONException = org.json.JSONException;
	using JSONObject = org.json.JSONObject;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	/// Misc tool methods for string/xml/xpath manipulation.
	/// 
	/// @author smartrics
	/// </summary>
	public sealed class Tools
	{

		private Tools()
		{

		}

		/// <param name="ns">              the name space </param>
		/// <param name="xpathExpression"> the expression </param>
		/// <param name="content">         the content </param>
		/// <returns> the list of nodes matching the supplied XPath. </returns>
		public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content)
		{
			return (NodeList) extractXPath(ns, xpathExpression, content, XPathConstants.NODESET, null);
		}

		/// <param name="ns"> the namespaces map </param>
		/// <param name="xpathExpression"> the xpath </param>
		/// <param name="content"> the content </param>
		/// <param name="encoding"> the charset </param>
		/// <returns> the list of nodes matching the supplied XPath. </returns>
		public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content, string encoding)
		{
			return (NodeList) extractXPath(ns, xpathExpression, content, XPathConstants.NODESET, encoding);
		}

		/// <param name="xpathExpression"> the xpath </param>
		/// <param name="content"> the content </param>
		/// <param name="returnType"> the result type </param>
		/// <returns> the list of nodes matching the supplied XPath. </returns>
		public static object extractXPath(string xpathExpression, string content, QName returnType)
		{
			return extractXPath(xpathExpression, content, returnType, null);
		}

		/// <param name="xpathExpression"> the xpath </param>
		/// <param name="content"> the content </param>
		/// <param name="returnType"> the result type </param>
		/// <param name="encoding"> the encoding/charset </param>
		/// <returns> the list of nodes mathching the supplied XPath. </returns>
		public static object extractXPath(string xpathExpression, string content, QName returnType, string encoding)
		{
			// Use the java Xpath API to return a NodeList to the caller so they can
			// iterate through
			return extractXPath(new Dictionary<string, string>(), xpathExpression, content, returnType, encoding);
		}

		/// <param name="ns">              the namespaces map </param>
		/// <param name="xpathExpression"> the XPath expression </param>
		/// <param name="content">         the content </param>
		/// <param name="returnType">      the return type </param>
		/// <returns> the list of nodes mathching the supplied XPath. </returns>
		public static object extractXPath(IDictionary<string, string> ns, string xpathExpression, string content, QName returnType)
		{
			return extractXPath(ns, xpathExpression, content, returnType, null);
		}

		/// <summary>
		/// extract the XPath from the content. the return value type is passed in
		/// input using one of the <seealso cref="XPathConstants"/>. See also
		/// <seealso cref="XPathExpression#evaluate(Object item, QName returnType)"/> ;
		/// </summary>
		/// <param name="ns">              the namespaces map </param>
		/// <param name="xpathExpression"> the XPath expression </param>
		/// <param name="content">         the content </param>
		/// <param name="returnType">      the return type </param>
		/// <param name="charset">         the charset </param>
		/// <returns> the result </returns>
		public static object extractXPath(IDictionary<string, string> ns, string xpathExpression, string content, QName returnType, string charset)
		{
			if (null == ns)
			{
				ns = new Dictionary<>();
			}
			string ch = charset;
			if (string.ReferenceEquals(ch, null))
			{
				ch = Charset.defaultCharset().name();
			}
			Document doc = toDocument(content, ch);
			XPathExpression expr = toExpression(ns, xpathExpression);
			try
			{
				object o = expr.evaluate(doc, returnType);
				return o;
			}
			catch (XPathExpressionException)
			{
				throw new System.ArgumentException("xPath expression cannot be executed: " + xpathExpression);
			}
		}

		/// <summary>
		/// If the result is neither a <seealso cref="NodeList"/> or a <seealso cref="Node"/> it will simply return the #toString().
		/// </summary>
		/// <param name="result"> the result of an XPath expression (a <seealso cref="NodeList"/> or a <seealso cref="Node"/>). </param>
		/// <returns> the serialised as xml result of an xpath expression evaluation. </returns>
		public static string xPathResultToXmlString(object result)
		{
			if (result == null)
			{
				return null;
			}
			try
			{
				StringWriter sw = new StringWriter();
				Transformer serializer = TransformerFactory.newInstance().newTransformer();
				serializer.setOutputProperty(OutputKeys.INDENT, "yes");
				serializer.setOutputProperty(OutputKeys.MEDIA_TYPE, "text/xml");
				if (result is NodeList)
				{
					serializer.transform(new DOMSource(((NodeList) result).item(0)), new StreamResult(sw));
				}
				else if (result is Node)
				{
					serializer.transform(new DOMSource((Node) result), new StreamResult(sw));
				}
				else
				{
					return result.ToString();
				}
				return sw.ToString();
			}
			catch (Exception e)
			{
				throw new Exception("Transformation caused an exception", e);
			}
		}

		/// <param name="ns"> the namespaces map </param>
		/// <param name="xpathExpression"> the XPath </param>
		/// <returns> true if the expression is valid </returns>
		public static bool isValidXPath(IDictionary<string, string> ns, string xpathExpression)
		{
			try
			{
				toExpression(ns, xpathExpression);
				return true;
			}
			catch (System.ArgumentException)
			{
				return false;
			}
		}

		/// <param name="ns"> the namespaces map </param>
		/// <param name="xpathExpression"> the XPath </param>
		/// <returns> the parsed string as <seealso cref="XPathExpression"/> </returns>
		public static XPathExpression toExpression(IDictionary<string, string> ns, string xpathExpression)
		{
			try
			{
				XPathFactory xpathFactory = XPathFactory.newInstance();
				XPath xpath = xpathFactory.newXPath();
				if (ns.Count > 0)
				{
					xpath.NamespaceContext = toNsContext(ns);
				}
				XPathExpression expr = xpath.compile(xpathExpression);
				return expr;
			}
			catch (XPathExpressionException e)
			{
				throw new System.ArgumentException("xPath expression can not be compiled: " + xpathExpression, e);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static javax.xml.namespace.NamespaceContext toNsContext(final Map<String, String> ns)
		private static NamespaceContext toNsContext(IDictionary<string, string> ns)
		{
			NamespaceContext ctx = new NamespaceContextAnonymousInnerClass(ns);
			return ctx;
		}

		private class NamespaceContextAnonymousInnerClass : NamespaceContext
		{
			private IDictionary<string, string> ns;

			public NamespaceContextAnonymousInnerClass(IDictionary<string, string> ns)
			{
				this.ns = ns;
			}


			public override string getNamespaceURI(string prefix)
			{
				string u = ns[prefix];
				if (null == u)
				{
					return XMLConstants.NULL_NS_URI;
				}
				return u;
			}

			public override string getPrefix(string namespaceURI)
			{
				foreach (string k in ns.Keys)
				{
					if (ns[k].Equals(namespaceURI))
					{
						return k;
					}
				}
				return null;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override public Iterator<?> getPrefixes(String namespaceURI)
			public override IEnumerator<object> getPrefixes(string namespaceURI)
			{
				return null;
			}

		}

		private static Document toDocument(string content, string charset)
		{
			string ch = charset;
			if (string.ReferenceEquals(ch, null))
			{
				ch = Charset.defaultCharset().name();
			}
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.NamespaceAware = true;
			try
			{
				DocumentBuilder builder = factory.newDocumentBuilder();
				Document doc = builder.parse(getInputStreamFromString(content, ch));
				return doc;
			}
			catch (ParserConfigurationException e)
			{
				throw new System.ArgumentException("parser for last response body caused an error", e);
			}
			catch (SAXException e)
			{
				throw new System.ArgumentException("last response body cannot be parsed", e);
			}
			catch (IOException e)
			{
				throw new System.ArgumentException("IO Exception when reading the document", e);
			}
		}

		/// <summary>
		/// this method uses @link <seealso cref="JSONObject"/> to parse the string and return
		/// true if parse succeeds.
		/// </summary>
		/// <param name="presumeblyJson"> string with some json (possibly). </param>
		/// <returns> true if json is valid </returns>
		public static bool isValidJson(string presumeblyJson)
		{
			object o = null;
			try
			{
				o = new JSONObject(presumeblyJson);
			}
			catch (JSONException)
			{
				return false;
			}
			return o != null;
		}

		/// <param name="json"> the json string </param>
		/// <returns> the string as xml. </returns>
		public static string fromJSONtoXML(string json)
		{
			HierarchicalStreamDriver driver = new JettisonMappedXmlDriver();
			StringReader reader = new StringReader(json);
			HierarchicalStreamReader hsr = driver.createReader(reader);
			StringWriter writer = new StringWriter();
			try
			{
				(new HierarchicalStreamCopier()).copy(hsr, new PrettyPrintWriter(writer));
				return writer.ToString();
			}
			finally
			{
				if (writer != null)
				{
					try
					{
						writer.close();
					}
					catch (IOException)
					{
						// ignore
					}
				}
			}
		}

		/// <summary>
		/// Yet another stream 2 string function.
		/// </summary>
		/// <param name="is"> the stream </param>
		/// <returns> the string. </returns>
		public static string getStringFromInputStream(System.IO.Stream @is)
		{
			return getStringFromInputStream(@is, Charset.defaultCharset().name());
		}

		/// <summary>
		/// Yet another stream 2 string function.
		/// </summary>
		/// <param name="is">       the stream </param>
		/// <param name="encoding"> the encoding of the bytes in the stream </param>
		/// <returns> the string. </returns>
		public static string getStringFromInputStream(System.IO.Stream @is, string encoding)
		{
			string line = null;
			if (@is == null)
			{
				return "";
			}
			System.IO.StreamReader @in = null;
			try
			{
				@in = new System.IO.StreamReader(@is, encoding);
			}
			catch (UnsupportedEncodingException e)
			{
				throw new System.ArgumentException("Unsupported encoding: " + encoding, e);
			}
			StringBuilder sb = new StringBuilder();
			try
			{
				while (!string.ReferenceEquals((line = @in.ReadLine()), null))
				{
					sb.Append(line);
				}
			}
			catch (IOException e)
			{
				throw new System.ArgumentException("Unable to read from stream", e);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Yet another stream 2 string function.
		/// </summary>
		/// <param name="string">   the string </param>
		/// <param name="encoding"> the encoding of the bytes in the stream </param>
		/// <returns> the input stream. </returns>
		public static System.IO.Stream getInputStreamFromString(string @string, string encoding)
		{
			if (string.ReferenceEquals(@string, null))
			{
				throw new System.ArgumentException("null input");
			}
			try
			{
				sbyte[] byteArray = @string.GetBytes(encoding);
				return new System.IO.MemoryStream(byteArray);
			}
			catch (UnsupportedEncodingException)
			{
				throw new System.ArgumentException("Unsupported encoding: " + encoding);
			}
		}

		/// <summary>
		/// converts a map to string
		/// </summary>
		/// <param name="map">      the map to convert </param>
		/// <param name="nvSep">    the nvp separator </param>
		/// <param name="entrySep"> the separator of each entry </param>
		/// <returns> the serialised map. </returns>
		public static string convertMapToString(IDictionary<string, string> map, string nvSep, string entrySep)
		{
			StringBuilder sb = new StringBuilder();
			if (map != null)
			{
				foreach (KeyValuePair<string, string> entry in map.SetOfKeyValuePairs())
				{
					string el = entry.Key;
					sb.Append(convertEntryToString(el, map[el], nvSep)).Append(entrySep);
				}
			}
			string repr = sb.ToString();
			int pos = repr.LastIndexOf(entrySep, StringComparison.Ordinal);
			return repr.Substring(0, pos);
		}

		/// <param name="name">  the name </param>
		/// <param name="value"> the value </param>
		/// <param name="nvSep"> the separator </param>
		/// <returns> the kvp as a string <code>&lt;name&gt;&lt;sep&gt;&lt;value&gt;</code>. </returns>
		public static string convertEntryToString(string name, string value, string nvSep)
		{
			return string.Format("{0}{1}{2}", name, nvSep, value);
		}

		/// <param name="text"> the text </param>
		/// <param name="expr"> the regex </param>
		/// <returns> true if regex matches text. </returns>
		public static bool regex(string text, string expr)
		{
			try
			{
				Pattern p = Pattern.compile(expr);
				bool find = p.matcher(text).find();
				return find;
			}
			catch (PatternSyntaxException)
			{
				throw new System.ArgumentException("Invalid regex " + expr);
			}
		}

		/// <summary>
		/// parses a map from a string.
		/// </summary>
		/// <param name="expStr">    the string with the serialised map. </param>
		/// <param name="nvSep">     the separator for keys and values. </param>
		/// <param name="entrySep">  the separator for entries in the map. </param>
		/// <param name="cleanTags"> if true the value is cleaned from any present html tag. </param>
		/// <returns> the parsed map. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Map<String, String> convertStringToMap(final String expStr, final String nvSep, final String entrySep, boolean cleanTags)
		public static IDictionary<string, string> convertStringToMap(string expStr, string nvSep, string entrySep, bool cleanTags)
		{
			string sanitisedExpStr = expStr.Trim();
			sanitisedExpStr = removeOpenEscape(sanitisedExpStr);
			sanitisedExpStr = removeCloseEscape(sanitisedExpStr);
			sanitisedExpStr = sanitisedExpStr.Trim();
			string[] nvpArray = sanitisedExpStr.Split(entrySep, true);
			IDictionary<string, string> ret = new Dictionary<string, string>();
			foreach (string nvp in nvpArray)
			{
				try
				{
					nvp = nvp.Trim();
					if ("".Equals(nvp))
					{
						continue;
					}
					nvp = removeOpenEscape(nvp).Trim();
					nvp = removeCloseEscape(nvp).Trim();
					string[] nvpArr = nvp.Split(nvSep, true);
					string k, v;
					k = nvpArr[0].Trim();
					v = "";
					if (nvpArr.Length == 2)
					{
						v = nvpArr[1].Trim();
					}
					else if (nvpArr.Length > 2)
					{
						int pos = nvp.IndexOf(nvSep, StringComparison.Ordinal) + nvSep.Length;
						v = nvp.Substring(pos).Trim();
					}
					if (cleanTags)
					{
						ret[k] = fromSimpleTag(v);
					}
					else
					{
						ret[k] = v;
					}
				}
				catch (Exception)
				{
					throw new System.ArgumentException("Each entry in the must be separated by '" + entrySep + "' and each entry must be expressed as a name" + nvSep + "value");
				}
			}
			return ret;
		}

		/// <param name="message"> the message to be included in the collapsable section header. </param>
		/// <param name="content"> the content collapsed. </param>
		/// <returns> a string with the html/js code to implement a collapsable section
		/// in fitnesse. </returns>
		public static string makeToggleCollapseable(string message, string content)
		{
			Random random = new Random();
			string id = Convert.ToString(content.GetHashCode()) + Convert.ToString(random.nextLong());
			StringBuilder sb = new StringBuilder();
			sb.Append("<div class='collapsible closed'>");
			sb.Append("<ul><li><a href='#' class='expandall'>Expand</a></li><li><a href='#' class='collapseall'>Collapse</a></li></ul>");
			sb.Append("<p class='title'>").Append(message).Append("</p>");
			sb.Append("<div>").Append(content).Append("</div>");
			sb.Append("</div>");
			return sb.ToString();
		}

		/// <summary>
		/// <table border="1">
		/// <caption>Substitutions</caption>
		/// <tr>
		/// <td><code>&lt;pre&gt;</code> and <code>&lt;/pre&gt;</code></td>
		/// <td><code>""</code></td>
		/// </tr>
		/// <tr>
		/// <td><code>&lt;</code></td>
		/// <td><code>&amp;lt;</code></td>
		/// </tr>
		/// <tr>
		/// <td><code>\n</code></td>
		/// <td><code>&lt;br /&gt;</code></td>
		/// </tr>
		/// <tr>
		/// <td><code>&nbsp;</code> <i>(space)</i></td>
		/// <td><code>&amp;nbsp;</code></td>
		/// </tr>
		/// <tr>
		/// <td><code>-----</code> <i>(5 hyphens)</i></td>
		/// <td><code>&lt;hr /&gt;</code></td>
		/// </tr>
		/// </table>
		/// </summary>
		/// <param name="text"> some text. </param>
		/// <returns> the html. </returns>
		public static string toHtml(string text)
		{
			return text.replaceAll("<pre>", "").replaceAll("</pre>", "").replaceAll("<", "&lt;").replaceAll(">", "&gt;").replaceAll("\n", "<br/>").replaceAll("\t", "    ").replaceAll(" ", "&nbsp;").replaceAll("-----", "<hr/>");
		}

		/// <param name="c"> some text </param>
		/// <returns> the text within <code>&lt;code&gt;</code> tags. </returns>
		public static string toCode(string c)
		{
			return "<code>" + c + "</code>";
		}

		/// <param name="somethingWithinATag"> some text enclosed in some html tag. </param>
		/// <returns> the text within the tag. </returns>
		public static string fromSimpleTag(string somethingWithinATag)
		{
			return somethingWithinATag.replaceAll("<[^>]+>", "").replace("</[^>]+>", "");
		}

		/// <param name="text"> some html </param>
		/// <returns> the text stripped out of all tags. </returns>
		public static string fromHtml(string text)
		{
			string ls = "\n";
			return text.replaceAll("<br[\\s]*/>", ls).replaceAll("<BR[\\s]*/>", ls).replaceAll("<span[^>]*>", "").replaceAll("</span>", "").replaceAll("<pre>", "").replaceAll("</pre>", "").replaceAll("&nbsp;", " ").replaceAll("&gt;", ">").replaceAll("&amp;", "&").replaceAll("&lt;", "<").replaceAll("&nbsp;", " ");
		}

		/// <param name="string"> a string </param>
		/// <returns> the string htmlified as a fitnesse label. </returns>
		public static string toHtmlLabel(string @string)
		{
			return "<i><span class='fit_label'>" + @string + "</span></i>";
		}

		/// <param name="href"> a string ending up in the anchor href. </param>
		/// <param name="text"> a string within anchors. </param>
		/// <returns> the string htmlified as a html link. </returns>
		public static string toHtmlLink(string href, string text)
		{
			return "<a href='" + href + "'>" + text + "</a>";
		}

		/// <param name="expected">        the expected value </param>
		/// <param name="typeAdapter">     the body adapter for the cell </param>
		/// <param name="formatter">       the formatter </param>
		/// <param name="minLenForToggle"> the value determining whether the content should be rendered
		///                        as a collapseable section. </param>
		/// <returns> the formatted content for a cell with a wrong expectation </returns>
		public static string makeContentForWrongCell<T1>(string expected, RestDataTypeAdapter typeAdapter, ICellFormatter<T1> formatter, int minLenForToggle)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Tools.toHtml(expected));
			if (formatter.DisplayActual)
			{
				sb.Append(toHtml("\n"));
				sb.Append(formatter.label("expected"));
				string actual = typeAdapter.ToString();
				sb.Append(toHtml("-----"));
				sb.Append(toHtml("\n"));
				if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
				{
					sb.Append(makeToggleCollapseable("toggle actual", toHtml(actual)));
				}
				else
				{
					sb.Append(toHtml(actual));
				}
				sb.Append(toHtml("\n"));
				sb.Append(formatter.label("actual"));
			}
			IList<string> errors = typeAdapter.Errors;
			if (errors.Count > 0)
			{
				sb.Append(toHtml("-----"));
				sb.Append(toHtml("\n"));
				foreach (string e in errors)
				{
					sb.Append(toHtml(e + "\n"));
				}
				sb.Append(toHtml("\n"));
				sb.Append(formatter.label("errors"));
			}
			return sb.ToString();
		}

		/// <param name="expected">        the expected value </param>
		/// <param name="typeAdapter">     the body type adaptor </param>
		/// <param name="formatter">       the formatter
		///                        the value determining whether the content should be rendered
		///                        as a collapseable section. </param>
		/// <param name="minLenForToggle"> the value determining whether the content should be rendered
		///                        as a collapseable section. </param>
		/// <returns> the formatted content for a cell with a right expectation </returns>
		public static string makeContentForRightCell<T1>(string expected, RestDataTypeAdapter typeAdapter, ICellFormatter<T1> formatter, int minLenForToggle)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(toHtml(expected));
			string actual = typeAdapter.ToString();
			if (formatter.DisplayActual && !expected.Equals(actual))
			{
				sb.Append(toHtml("\n"));
				sb.Append(formatter.label("expected"));
				sb.Append(toHtml("-----"));
				sb.Append(toHtml("\n"));
				if (minLenForToggle >= 0 && actual.Length > minLenForToggle)
				{
					sb.Append(makeToggleCollapseable("toggle actual", toHtml(actual)));
				}
				else
				{
					sb.Append(toHtml(actual));
				}
				sb.Append(toHtml("\n"));
				sb.Append(formatter.label("actual"));
			}
			return sb.ToString();
		}

		private static string removeCloseEscape(string str)
		{
			return trimStartEnd("-!", str);
		}

		private static string removeOpenEscape(string str)
		{
			return trimStartEnd("!-", str);
		}

		private static string trimStartEnd(string pattern, string str)
		{
			if (str.StartsWith(pattern, StringComparison.Ordinal))
			{
				str = str.Substring(2);
			}
			if (str.EndsWith(pattern, StringComparison.Ordinal))
			{
				str = str.Substring(0, str.Length - 2);
			}
			return str;
		}

		public static string wrapInDiv(string body)
		{
			return string.Format("<div>{0}</div>", body);
		}

	}

}