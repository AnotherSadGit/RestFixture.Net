using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
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
    /// Specifies the type of object returned when evaluating an XPath expression that has been 
    /// applied to an XML document.
    /// </summary>
    /// <remarks>Based on the Java XPathConstants class, used in the Java implementation of this 
    /// class.</remarks>
    public enum XPathEvaluationReturnType
    {
        Boolean = 0,
        Node,
        Nodeset,
        Number, 
        String
    }

    /// <summary>
    /// Misc tool methods for  xml and xpath manipulation.
    /// </summary>
    public sealed class XmlTools
    {
        private static IDictionary<XPathEvaluationReturnType, Type> _returnTypeMappings =
                new Dictionary<XPathEvaluationReturnType, Type>();

        private static IDictionary<XPathEvaluationReturnType, Type> ReturnTypeMappings
        {
            get
            {
                if (_returnTypeMappings.Count <= 0)
                {
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Boolean, typeof(bool));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Nodeset, typeof(XPathNodeIterator));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Node, typeof(XPathNodeIterator));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Number, typeof(double));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.String, typeof(string));
                }

                return _returnTypeMappings;
            }
        }

        /// <param name="ns">              the name space </param>
        /// <param name="xpathExpression"> the expression </param>
        /// <param name="content">         the content </param>
        /// <returns> the list of nodes matching the supplied XPath. </returns>
        public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content)
        {
            return (NodeList)extractXPath(ns, xpathExpression, content, XPathEvaluationReturnType.Nodeset);
        }

        /// <param name="ns"> the namespaces map </param>
        /// <param name="xpathExpression"> the xpath </param>
        /// <param name="content"> the content </param>
        /// <param name="encoding"> the charset </param>
        /// <returns> the list of nodes matching the supplied XPath. </returns>
        public static NodeList extractXPath(IDictionary<string, string> ns, string xpathExpression, string content,
            string encoding)
        {
            return (NodeList)extractXPath(ns, xpathExpression, content, XPathEvaluationReturnType.Nodeset);
        }

        /// <param name="xpathExpression"> the xpath </param>
        /// <param name="content"> the content </param>
        /// <param name="returnType"> the result type </param>
        /// <returns> the list of nodes matching the supplied XPath. </returns>
        public static object extractXPath(string xpathExpression, string content, XPathEvaluationReturnType returnType)
        {
            return extractXPath(xpathExpression, content, returnType, null);
        }

        /// <param name="xpathExpression"> the xpath </param>
        /// <param name="content"> the content </param>
        /// <param name="returnType"> the result type </param>
        /// <param name="encoding"> the encoding/charset </param>
        /// <returns> the list of nodes mathching the supplied XPath. </returns>
        public static object extractXPath(string xpathExpression, string content, XPathEvaluationReturnType returnType, string encoding)
        {
            // Use the java Xpath API to return a NodeList to the caller so they can
            // iterate through
            return extractXPath(new Dictionary<string, string>(), xpathExpression, content, returnType);
        }

        /// <summary>
        /// extract the XPath from the content. the return value type is passed in input.
        /// </summary>
        /// <param name="ns">              the namespaces map </param>
        /// <param name="xpathExpression"> the XPath expression </param>
        /// <param name="content">         the content </param>
        /// <param name="returnType">      the return type </param>
        /// <returns> the result </returns>
        public static object extractXPath(IDictionary<string, string> ns, string xpathExpression, 
            string content, XPathEvaluationReturnType returnType)
        {
            // The original Java implementation had a charset parameter for this method.  However, 
            //  in .NET charset or encoding does not need to be specified as the content being 
            //  parsed is a .NET string, which does not have encoding associated with it.  It would 
            //  be different if we had to parse the contents of a stream or a file.

            using (StringReader sr = new StringReader(content))
            using (XmlReader xr = XmlReader.Create(sr))
            {
                XPathDocument document = new XPathDocument(xr);
                XPathNavigator navigator = document.CreateNavigator();

                // There are a couple of overloads of XPathNavigator.Evaluate(xpath):
                //  1) Evaluate(string xpath, IXmlNamespaceResolver resolver), where 
                //      IXmlNamespaceResolver is usually an XmlNamespaceManager;
                //  2) Evaluate(string xpath)
                // Under the hood both compile the arguments into an XPathExpression object, using   
                //  XPathExpression.Compile(string xpath, IXmlNamespaceResolver nsResolver).  When 
                //  XPathNavigator.Evaluate(string xpath) calls XPathExpression.Compile it passes 
                //  null for the nsResolver argument.  So we can use 
                //  XPathNavigator.Evaluate(string xpath, IXmlNamespaceResolver resolver) for all 
                //  scenarios and just pass null into resolver if we have no namespaces.
                XmlNamespaceManager namespaceManager = GetNamespaceManager(ns, xr.NameTable);
                object result = null;
                try
                {
                    result = navigator.Evaluate(xpathExpression, namespaceManager);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(
                        "xPath expression would return a node set: " + xpathExpression,
                        ex);
                }
                catch (XPathException ex)
                {
                    throw new ArgumentException(
                        "xPath expression is not valid: " + xpathExpression);
                }

                CheckExtractionReturnType(result, returnType);

                if (returnType == XPathEvaluationReturnType.Node)
                {
                    XPathNodeIterator iterator = (XPathNodeIterator) result;
                    iterator.MoveNext();
                    XPathNavigator singleIteratorNode = iterator.Current;
                    return singleIteratorNode;
                }

                return result;
            }
        }

        private static XmlNamespaceManager GetNamespaceManager(IDictionary<string, string> ns, 
            XmlNameTable nameTable)
        {
            if (ns == null || ns.Keys.Count <= 0)
            {
                return null;
            }

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            foreach (string prefix in ns.Keys)
            {
                namespaceManager.AddNamespace(prefix, ns[prefix]);
            }

            return namespaceManager;
        }

        private static void CheckExtractionReturnType(object returnValue, 
            XPathEvaluationReturnType expectedReturnType)
        {
            // According to https://docs.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathnavigator.evaluate?view=netframework-4.5
            //  XPathNavigator.Evaluate can only return one of the following data types: 
            //  bool, double, string or XPathNodeIterator.  These correspond to the values of the 
            //  XPathEvaluationReturnType enum, where XPathNodeIterator can represent a Nodeset or 
            //  a Node.

            IDictionary<XPathEvaluationReturnType, Type> returnTypeMappings = ReturnTypeMappings;

            string errorMessage = null;

            Type expectedType = returnTypeMappings.GetValueOrNull(expectedReturnType);
            if (expectedType == null)
            {
                errorMessage = string.Format("Invalid return type.  "
                                            + "Result of XPath evaluation cannot return type {0}.",
                    expectedReturnType);
                throw new ArgumentException(errorMessage);
            }

            Type actualType = returnValue.GetType();
            if (actualType != expectedType)
            {
                errorMessage = string.Format("XPath expression return type {0} does not match the "
                            + "supplied return type {1}.", actualType.FullName, expectedReturnType);
                throw new XPathException(errorMessage);
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
