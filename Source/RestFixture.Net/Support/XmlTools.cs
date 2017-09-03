using System;
using System.Collections.Generic;
using System.IO;
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
namespace restFixture.Net.Support
{
    /// <summary>
    /// Specifies the type of object returned when evaluating an XPath expression that has been 
    /// applied to an XML document.
    /// </summary>
    /// <remarks>Based on the Java XPathConstants class, used in the Java implementation of this 
    /// class.</remarks>
    public enum XPathEvaluationReturnType
    {
        Node = 0,
        Nodeset,
        Boolean,
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
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Nodeset, typeof(XPathNavigator));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Node, typeof(XPathNavigator));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.Number, typeof(double));
                    _returnTypeMappings.Add(XPathEvaluationReturnType.String, typeof(string));
                }

                return _returnTypeMappings;
            }
        }

        /// <summary>
        /// Indicates whether any node in the XML content matches the XPath expression.
        /// </summary>
        /// <param name="ns">The namespaces map, which lists the namespaces in the XML and their 
        /// aliases.</param>
        /// <param name="xpathExpression">The XPath expression to apply.</param>
        /// <param name="content">The XML content the Xpath expression will be applied to.</param>
        /// <returns>true if a node in the XML content matches the XPath expression, otherwise 
        /// false.</returns>
        public static bool NodeMatchFound(IDictionary<string, string> ns,
            string xpathExpression, string content)
        {
            object result = extractXPath(ns, xpathExpression, content,
                XPathEvaluationReturnType.Node);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Applies the XPath expression to the XML content and returns the value of the first 
        /// matching node.
        /// </summary>
        /// <param name="ns">The namespaces map, which lists the namespaces in the XML and their 
        /// aliases.</param>
        /// <param name="xpathExpression">The XPath expression to apply.</param>
        /// <param name="content">The XML content the Xpath expression will be applied to.</param>
        /// <returns>The value of the first node matching the XPath expression, or null if there  
        /// is no matching node.</returns>
        public static string GetNodeValue(IDictionary<string, string> ns,
            string xpathExpression, string content)
        {
            object result = extractXPath(ns, xpathExpression, content,
                XPathEvaluationReturnType.Node);
            if (result == null)
            {
                return null;
            }

            if (result is XPathNavigator)
            {
                return ((XPathNavigator)result).Value;
            }

            return null;
        }

        /// <summary>
        /// Applies the XPath expression to the XML content and returns the result.
        /// </summary>
        /// <param name="xpathExpression">The XPath expression to apply.</param>
        /// <param name="content">The XML content the Xpath expression will be applied to.</param>
        /// <param name="returnType">The return type: Boolean, Number, String, Node or Nodeset.</param>
        /// <returns>The result of applying the XPath expression to the XML content.  The type 
        /// returned will depend on the specified return type:  Boolean will return a 
        /// System.Boolean, Number will return a System.Double, String will return a 
        /// System.String, and Node or Nodeset will return an XPathNavigator object representing 
        /// a single node.  Return types Node and Nodeset are equivalent: They will each only 
        /// return a single node.  If the XPath expression matches multiple nodes then only the 
        /// first node or its value will be returned, depending on the return type specified.</returns>
        /// <remarks>This method is only used to read the contents of a node, as either a string 
        /// or a boolean, or to check the existence of a node.  When multiple  nodes match the 
        /// XPath expression only the contents of the first is ever read, and only the first node 
        /// is needed to prove existence.  Hence the return type Nodeset is identical to return 
        /// type Node, returning only a single node.
        /// 
        /// The original Java implementation had a charset parameter for this method.  
        /// However, in .NET charset or encoding does not need to be specified as the content 
        /// being parsed is a .NET string, which does not have encoding associated with it.  It 
        /// would be different if we had to parse the contents of a stream or a file.</remarks>
        public static object extractXPath(string xpathExpression, string content,
            XPathEvaluationReturnType returnType)
        {
            // Use the java Xpath API to return a NodeList to the caller so they can
            // iterate through
            return extractXPath(new Dictionary<string, string>(), xpathExpression, content,
                returnType);
        }

        /// <summary>
        /// Applies the XPath expression to the XML content and returns the result.
        /// </summary>
        /// <param name="ns">The namespaces map, which lists the namespaces in the XML and their 
        /// aliases.</param>
        /// <param name="xpathExpression">The XPath expression to apply.</param>
        /// <param name="content">The XML content the Xpath expression will be applied to.</param>
        /// <returns>The result of applying the XPath expression to the XML content.  If the XPath 
        /// expression matches multiple nodes then only the first node will be returned.</returns>
        /// <remarks>This method is only used to read the contents of a node, as either a string 
        /// or a boolean, or to check the existence of a node.  When multiple  nodes match the 
        /// XPath expression only the contents of the first is ever read, and only the first node 
        /// is needed to prove existence.  Hence the return type Nodeset is identical to return 
        /// type Node, returning only a single node.
        /// 
        /// The original Java implementation had a charset parameter for this method.  
        /// However, in .NET charset or encoding does not need to be specified as the content 
        /// being parsed is a .NET string, which does not have encoding associated with it.  It 
        /// would be different if we had to parse the contents of a stream or a file.</remarks>
        public static XPathNavigator extractXPath(IDictionary<string, string> ns,
            string xpathExpression, string content)
        {
            // The original Java implementation returned a Java NodeList object.
            return (XPathNavigator)extractXPath(ns, xpathExpression, content,
                XPathEvaluationReturnType.Nodeset);
        }

        /// <summary>
        /// Applies the XPath expression to the XML content and returns the result.
        /// </summary>
        /// <param name="ns">The namespaces map, which lists the namespaces in the XML and their 
        /// aliases.</param>
        /// <param name="xpathExpression">The XPath expression to apply.</param>
        /// <param name="content">The XML content the Xpath expression will be applied to.</param>
        /// <param name="returnType">The return type: Boolean, Number, String, Node or Nodeset.</param>
        /// <returns>The result of applying the XPath expression to the XML content.  The type 
        /// returned will depend on the specified return type:  Boolean will return a 
        /// System.Boolean, Number will return a System.Double, String will return a 
        /// System.String, and Node or Nodeset will return an XPathNavigator object representing 
        /// a single node.  Return types Node and Nodeset are equivalent: They will each only 
        /// return a single node.  If the XPath expression matches multiple nodes then only the 
        /// first node or its value will be returned, depending on the return type specified.</returns>
        /// <remarks>This method is only used to read the contents of a node, as either a string 
        /// or a boolean, or to check the existence of a node.  When multiple  nodes match the 
        /// XPath expression only the contents of the first is ever read, and only the first node 
        /// is needed to prove existence.  Hence the return type Nodeset is identical to return 
        /// type Node, returning only a single node.
        /// 
        /// The original Java implementation had a charset parameter for this method.  
        /// However, in .NET charset or encoding does not need to be specified as the content 
        /// being parsed is a .NET string, which does not have encoding associated with it.  It 
        /// would be different if we had to parse the contents of a stream or a file.</remarks>
        public static object extractXPath(IDictionary<string, string> ns, string xpathExpression,
            string content, XPathEvaluationReturnType returnType)
        {
            if (string.IsNullOrWhiteSpace(xpathExpression))
            {
                throw new ArgumentException("XPath expression is null");
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("XML is null");
            }

            XPathDocument document = null;
            XmlNamespaceManager namespaceManager = null;

            using (StringReader sr = new StringReader(content))
            using (XmlReader xr = XmlReader.Create(sr))
            {
                try
                {
                    document = new XPathDocument(xr);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("XML is ill-formed", ex);
                }

                // Handling namespaces:
                //  XPathNavigator.Evaluate(xpath) and XPathNavigator.SelectSingleNode(xpath) both 
                //  compile the xpath expression into an XPathExpression object, using 
                //  XPathExpression.Compile(string xpath, IXmlNamespaceResolver nsResolver).  They 
                //  pass null in as the IXmlNamespaceResolver.  This is equivalent to calling 
                //  XPathNavigator.Evaluate(string xpath, IXmlNamespaceResolver resolver) or 
                //  XPathNavigator.SelectSingleNode(string xpath, IXmlNamespaceResolver resolver), 
                //  respectively, with the resolver set to null.  So we'll just pass null in as 
                //  the resolver if we have no namespaces.
                namespaceManager = GetNamespaceManager(ns, xr.NameTable);
            }

            object result = null;
            XPathNavigator navigator = document.CreateNavigator();

            try
            {
                result = navigator.Evaluate(xpathExpression, namespaceManager);
                result = GetNodeValue(result, returnType);
                if (result == null)
                {
                    return null;
                }
                return result;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    "xPath expression would return a node set: " + xpathExpression,
                    ex);
            }
            catch (XPathException)
            {
                throw new ArgumentException(
                    "xPath expression is not valid: " + xpathExpression);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    "Error parsing XML with xPath expression " + xpathExpression, ex);
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

        private static object GetNodeValue(object rawResult, XPathEvaluationReturnType returnType)
        {
            if (rawResult == null)
            {
                return null;
            }

            XPathNodeIterator iterator = rawResult as XPathNodeIterator;
            if (iterator == null)
            {
                return rawResult;
            }

            // If there is a match the Count will be a positive integer, if there is no match 
            //  the Count will be 0.  Can't depend on the node the iterator moves to as if 
            //  there is no match it will just move to the root node.  So we have to check the 
            //  Count to determine if there is a match or not.
            if (iterator.Count == 0)
            {
                return null;
            }

            iterator.MoveNext();
            XPathNavigator node = iterator.Current;
            if (node == null)
            {
                return rawResult;
            }

            switch (returnType)
            {
                case XPathEvaluationReturnType.Boolean:
                    return node.ValueAsBoolean;
                case XPathEvaluationReturnType.Number:
                    return node.ValueAsDouble;
                case XPathEvaluationReturnType.String:
                    return node.Value;
                case XPathEvaluationReturnType.Node:
                case XPathEvaluationReturnType.Nodeset:
                    return node;
            }
            return rawResult;
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
            // Actual type could be derived from the expected type, for example the actual type 
            //  could be XPathSelectionIterator which is derived from the expected type 
            //  XPathNodeIterator.  So use IsAssignableFrom instead of equality comparison.
            // Assume any return type could be converted to a string.
            if (!expectedType.IsAssignableFrom(actualType) && expectedType != typeof(string))
            {
                errorMessage = string.Format("XPath expression return type {0} is not compatible "
                            + "with the specified return type {1}.",
                            actualType.FullName, expectedReturnType);
                throw new XPathException(errorMessage);
            }
        }

        private static object GetReturnTypeDefault(XPathEvaluationReturnType expectedReturnType)
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

            if (expectedType.IsValueType)
            {
                return Activator.CreateInstance(expectedType);
            }

            // Can return null for all reference types.
            return null;
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
                XPathNavigator node = result as XPathNavigator;
                if (node != null)
                {
                    // Original Java implementation only appears to display the first node of the 
                    //  nodeset.
                    return node.OuterXml;
                }

                return result.ToString();
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
