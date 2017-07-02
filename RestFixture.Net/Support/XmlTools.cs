using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

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
    /// Misc tool methods for  xml and xpath manipulation.
    /// </summary>
    public sealed class XmlTools
    {

        /// <param name="ns">              the name space </param>
        /// <param name="xpathExpression"> the expression </param>
        /// <param name="content">         the content </param>
        /// <returns> the list of nodes matching the supplied XPath. </returns>
        public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content)
        {
            return (NodeList)extractXPath(ns, xpathExpression, content, XPathConstants.NODESET, null);
        }

        /// <param name="ns"> the namespaces map </param>
        /// <param name="xpathExpression"> the xpath </param>
        /// <param name="content"> the content </param>
        /// <param name="encoding"> the charset </param>
        /// <returns> the list of nodes matching the supplied XPath. </returns>
        public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content,
            string encoding)
        {
            return (NodeList)extractXPath(ns, xpathExpression, content, XPathConstants.NODESET, encoding);
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
        public static object extractXPath(IDictionary<string, string> ns, string xpathExpression, string content,
            QName returnType)
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
        public static object extractXPath(IDictionary<string, string> ns, string xpathExpression, string content,
            QName returnType, string charset)
        {
            if (null == ns)
            {
                ns = new Dictionary<string, string>();
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
                    serializer.transform(new DOMSource(((NodeList)result).item(0)), new StreamResult(sw));
                }
                else if (result is Node)
                {
                    serializer.transform(new DOMSource((Node)result), new StreamResult(sw));
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
    }
}
