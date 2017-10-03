using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Data;
using RestFixture.Net.Handlers;

namespace RestFixture.Net.UnitTests.LetHandlersTests
{
    [TestClass]
    public class LetBodyHandler_Handle : LetHandlersTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            this.SetupVariablesProvider();
        }

        [TestMethod]
        public void Should_Return_Null_For_Null_String()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            string expression = "null";

            // Act.
            string result = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Return_String_From_XmlNode_Existing()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/xml");
            response.Body = this.GetXmlString();
            string expression = "/root/dispersionRef/text()";
            string expectedResult = "http://localhost:8111";

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_XmlNode_Empty()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/xml");
            response.Body = this.GetXmlString();
            string expression = "/root/emptyNode/text()";
            // null, not empty string, since an empty node has no child text node.
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_XmlNode_Empty_NoEndTag()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/xml");
            response.Body = this.GetXmlString();
            string expression = "/root/emptyNodeNoEndTag/text()";
            // null, not empty string, since an empty node has no child text node.
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_XmlNode_NonExistent()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/xml");
            response.Body = this.GetXmlString();
            string expression = "/root/nonExistentNode/text()";
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_String_From_JsonNode_Existing()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/json");
            response.Body = this.GetJsonString();
            // Have to use XPath expression to query JSON.  For JavaScript parsing see 
            //  LetBodyJsHandler.
            string expression = "/root/dispersionRef/text()";
            string expectedResult = "http://localhost:8111";

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_JsonNode_Empty()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/json");
            response.Body = this.GetJsonString();
            // Have to use XPath expression to query JSON.  For JavaScript parsing see 
            //  LetBodyJsHandler.
            string expression = "/root/emptyProperty/text()";
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_JsonNode_Null()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/json");
            response.Body = this.GetJsonString();
            // Have to use XPath expression to query JSON.  For JavaScript parsing see 
            //  LetBodyJsHandler.
            string expression = "/root/nullProperty/text()";
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_From_JsonNode_NonExistent()
        {
            // Arrange.
            RestResponse response = new RestResponse();
            response.addHeader("Content-Type", "application/json");
            response.Body = this.GetJsonString();
            // Have to use XPath expression to query JSON.  For JavaScript parsing see 
            //  LetBodyJsHandler.
            string expression = "/root/nonExistentProperty/text()";
            string expectedResult = null;

            // Act.
            string actualResult = ExtractValueFromResponseBody(response, expression);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        private string ExtractValueFromResponseBody(RestResponse response, string xpathExpression)
        {
            return EvaluateExpressionAgainstResponse<LetBodyHandler>(response, xpathExpression);
        }

        private string GetXmlString()
        {
            return
@"<root>
    <accountRef>http://something:8111</accountRef>
    <label>default</label>
    <dispersionRef>http://localhost:8111</dispersionRef>
    <emptyNode></emptyNode>
    <emptyNodeNoEndTag/>
</root>";
        }

        private string GetJsonString()
        {
            // JSON uses empty braces to indicate an empty property, and the keyword "null", 
            //  without the quotes, to indicate a null property.
            return
@"{
	""root"": {
		""accountRef"": ""http://something:8111"",
		""label"": ""default"",
		""websiteRef"": ""ws1"",
		""dispersionRef"": ""http://localhost:8111"", 
        ""emptyProperty"": {}, 
        ""nullProperty"": null
	}
}";
        }
    }
}