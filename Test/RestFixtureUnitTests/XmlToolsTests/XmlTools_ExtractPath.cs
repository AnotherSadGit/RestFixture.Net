using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Tools;

namespace RestFixture.Net.UnitTests.XmlToolsTests
{
    [TestClass]
    public class XmlTools_ExtractPath
    {
        [TestMethod]
        public void Should_Extract_Value_From_Xml_As_String()
        {
            // Arrange, act and assert.
            ExtractValueFromXml("count(/)", XPathEvaluationReturnType.String, "1");
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_As_Number()
        {
            // Arrange, act and assert.
            ExtractValueFromXml("count(/)", XPathEvaluationReturnType.Number, 1.0);
            ExtractValueFromXml("count(/a/c)", XPathEvaluationReturnType.Number, 2.0);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_As_Boolean()
        {
            // Arrange, act and assert.
            ExtractValueFromXml("count(/a/c)=2", XPathEvaluationReturnType.Boolean, true);
            ExtractValueFromXml("count(/a/c)=3", XPathEvaluationReturnType.Boolean, false);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_As_Node()
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";
            string expectedValue = "test";

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a/b", xml, XPathEvaluationReturnType.Node);
            XPathNavigator actualValue = rawValue as XPathNavigator;

            // Assert.
            Assert.IsInstanceOfType(rawValue, typeof (XPathNavigator));
            Assert.IsNotNull(actualValue);
            Assert.AreEqual(expectedValue, actualValue.Value);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_As_Nodeset()
        {
            // extractXPath returns a single node for both return types Node and Nodeset.  This is 
            //  because RestFixture.Net and the original RestFixture don't ever need a nodeset 
            //  returned.  Both versions of the application are only interested in the value of 
            //  the first node in a nodeset, or in testing whether any node exists.  So returning 
            //  a single node is sufficient.

            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";
            string expectedValue = "test";

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a/b", xml, XPathEvaluationReturnType.Nodeset);
            XPathNavigator actualValue = rawValue as XPathNavigator;

            // Assert.
            Assert.IsInstanceOfType(rawValue, typeof(XPathNavigator));
            Assert.IsNotNull(actualValue);
            Assert.AreEqual(expectedValue, actualValue.Value);
        }

        [TestMethod]
        public void Should_Extract_TextNode_From_Xml_As_Node()
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";
            string expectedValue = "test";

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a/b/text()", xml, XPathEvaluationReturnType.Node);
            XPathNavigator actualValue = rawValue as XPathNavigator;

            // Assert.
            Assert.IsInstanceOfType(rawValue, typeof(XPathNavigator));
            Assert.IsNotNull(actualValue);
            Assert.AreEqual(expectedValue, actualValue.Value);
        }

        [TestMethod]
        public void Should_Extract_TextNode_From_Xml_As_String()
        {
            // Arrange, act and assert.
            ExtractValueFromXml("/a/b/text()", XPathEvaluationReturnType.String, "test");
        }

        [TestMethod]
        public void Should_Extract_NonExistent_Node_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentNode(XPathEvaluationReturnType.Node);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_TextNode_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentTextNode(XPathEvaluationReturnType.Node);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_Node_Boolean_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentNode(XPathEvaluationReturnType.Boolean);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_TextNode_Boolean_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentTextNode(XPathEvaluationReturnType.Boolean);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_Node_Numeric_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentNode(XPathEvaluationReturnType.Number);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_TextNode_Numeric_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentTextNode(XPathEvaluationReturnType.Number);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_Node_String_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentNode(XPathEvaluationReturnType.String);
        }

        [TestMethod]
        public void Should_Extract_NonExistent_TextNode_String_As_Null()
        {
            // Arrange, act and assert.
            ExtractNonExistentTextNode(XPathEvaluationReturnType.String);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_With_DefaultNamespace()
        {
            // Arrange.
            string xml = "<a xmlns='http://ns.com'><b>test</b><c>1</c></a>";
            Dictionary<string, string> ns = new Dictionary<string, string>();
            ns.Add("def", "http://ns.com");
            string expectedValue = "test";

            // Act.
            object rawValue =
                XmlTools.extractXPath(ns, "/def:a/def:b", xml, XPathEvaluationReturnType.String);
            string actualValue = rawValue.ToString();

            // Assert.
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_With_NonDefaultNamespace()
        {
            // Arrange.
            string xml = "<?xml version='1.0' ?><a xmlns:ns1='http://ns1.com'><b>test</b><ns1:c>tada</ns1:c></a>";
            Dictionary<string, string> ns = new Dictionary<string, string>();
            ns.Add("alias", "http://ns1.com");
            string expectedValue = "tada";

            // Act.
            object rawValue =
                XmlTools.extractXPath(ns, "/a/alias:c", xml, XPathEvaluationReturnType.String);
            string actualValue = rawValue.ToString();

            // Assert.
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void Should_Extract_Value_From_Xml_With_NestedNamespaces()
        {
            // Arrange.
            string xml = "<?xml version='1.0' ?><resource xmlns='http://ns.com'><name>a funky name</name>"
                + "<data>an important message</data>"
                + "<nstag xmlns:ns1='http://smartrics/ns1'><ns1:number>3</ns1:number></nstag></resource>";
            Dictionary<string, string> ns = new Dictionary<string, string>();
            ns.Add("ns", "http://ns.com");
            ns.Add("ns2", "http://smartrics/ns1");
            double expectedValue = 3;

            // Act.
            object rawValue =
                XmlTools.extractXPath(ns, "/ns:resource/ns:nstag/ns2:number", 
                xml, XPathEvaluationReturnType.Number);
            double actualValue = (double)rawValue;

            // Assert.
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_Invalid_XPathExpression()
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a[text=1", xml, XPathEvaluationReturnType.Node);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_IllFormed_Xml()
        {
            // Arrange.
            string xml = "<a>test<a>";

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a/text()", xml, XPathEvaluationReturnType.Node);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_Null_XPathExpression()
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";

            // Act.
            object rawValue =
                XmlTools.extractXPath(null, xml, XPathEvaluationReturnType.Node);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_Null_Xml()
        {
            // Arrange.
            string xml = null;

            // Act.
            object rawValue =
                XmlTools.extractXPath("/a/text()", xml, XPathEvaluationReturnType.Node);
        }

        private void ExtractValueFromXml<T>(string xpathExpression,
            XPathEvaluationReturnType returnType, T expectedValue)
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";

            // Act.
            object rawValue =
                XmlTools.extractXPath(xpathExpression, xml, returnType);
            object actualValue = default(T);
            if (typeof (T) == typeof (string))
            {
                actualValue = rawValue.ToString();
            }
            else
            {
                actualValue = (T)rawValue;
            }

            // Assert.
            Assert.AreEqual(expectedValue, actualValue);
        }

        private void ExtractNonExistentNode(XPathEvaluationReturnType returnType)
        {
            ExtractNonExistentNode("/a/nonExistentNode", returnType);
        }

        private void ExtractNonExistentTextNode(XPathEvaluationReturnType returnType)
        {
            ExtractNonExistentNode("/a/nonExistentNode/text()", returnType);
        }

        private void ExtractNonExistentNode(string xpathExpression,
            XPathEvaluationReturnType returnType)
        {
            // Arrange.
            string xml = "<a><b>test</b><c>1</c><c>2</c></a>";

            // Act.
            object rawValue =
                XmlTools.extractXPath(xpathExpression, xml, returnType);

            // Assert.
            Assert.IsNull(rawValue);
        }
    }
}