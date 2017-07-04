using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public static XPathNodeIterator extractXPath(IDictionary<string, string> ns, 
            string xpathExpression, string content)
        {
            // The original Java implementation returned a Java NodeList object.
            return (XPathNodeIterator)extractXPath(ns, xpathExpression, content, 
                XPathEvaluationReturnType.Nodeset);
        }

        /// <param name="xpathExpression"> the xpath </param>
        /// <param name="content"> the content </param>
        /// <param name="returnType"> the result type </param>
        /// <param name="encoding"> the encoding/charset </param>
        /// <returns> the list of nodes mathching the supplied XPath. </returns>
        public static object extractXPath(string xpathExpression, string content, 
            XPathEvaluationReturnType returnType)
        {
            // Use the java Xpath API to return a NodeList to the caller so they can
            // iterate through
            return extractXPath(new Dictionary<string, string>(), xpathExpression, content, 
                returnType);
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
                    XPathNodeIterator iterator = (XPathNodeIterator)result;
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
                object resultToConvert;
                XPathNodeIterator nodeIterator = result as XPathNodeIterator;
                if (nodeIterator != null)
                {
                    // Original Java implementation only appears to display the first node of the 
                    //  nodeset.
                    nodeIterator.MoveNext();
                    resultToConvert = nodeIterator.Current;
                }
                else
                {
                    resultToConvert = result;
                }

                return resultToConvert.ToString();
            }
            catch (Exception e)
            {
                throw new InvalidCastException(
                    "Attempt to convert XPath evaluation result into a string caused an error.", e);
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
                XmlNamespaceManager namespaceManager = GetNamespaceManager(ns, new NameTable());
                XPathExpression expression = 
                    XPathExpression.Compile(xpathExpression, namespaceManager);
                return expression;
            }
            catch (XPathException e)
            {
                throw new System.ArgumentException(
                    "xPath expression can not be compiled: " + xpathExpression, e);
            }
        }

        /// <summary>
        /// Parses the string and returns  true if parse succeeds.
        /// </summary>
        /// <param name="presumeblyJson"> string with some json (possibly). </param>
        /// <returns> true if json is valid </returns>
        public static bool isValidJson(string presumeblyJson)
        {
            object o = null;
            try
            {
                o = JToken.Parse(presumeblyJson);
            }
            catch (Exception)
            {
                return false;
            }
            return o != null;
        }

        /// <param name="json"> the json string </param>
        /// <returns> the string as xml. </returns>
        public static string fromJSONtoXML(string json)
        {
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);

            var stringBuilder = new StringBuilder();

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                doc.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }
    }
}
