using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using restFixture.Net;
using restFixture.Net.Javascript;
using restFixture.Net.Support;
using restFixture.Net.Variables;
using RestClient.Data;

namespace RestFixtureUnitTests
{
    [TestClass]
    public class JavascriptWrapper_EvaluateExpression
    {
        private Mock<IRunnerVariablesProvider> _mock = new Mock<IRunnerVariablesProvider>();

        public FitVariables Variables { get; set; }

        public IRunnerVariablesProvider VariablesProvider
        {
            get
            {
                return Mock.Of<IRunnerVariablesProvider>(varProv =>
                    varProv.CreateRunnerVariables() == this.Variables);
            }
        }

        [TestInitialize]
        public void SetupVariablesProvider()
        {
            this.Variables = new FitVariables();
            this.Variables.clearAll();
        }

        [TestMethod]
        public void Should_Read_Symbols_From_JavaScript()
        {
            // Arrange.
            FitVariables initialVariables = new FitVariables();
            initialVariables.put("my_sym", "98");
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider(initialVariables);
            RestResponse response = new RestResponse();
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);
            string expectedResult = "my sym is: 98";

            // Act.
            string javascriptText = "'my sym is: ' + symbols['my_sym']";
            object rawResult = jswrapper.evaluateExpression(response, javascriptText);
            string actualResult = rawResult.ToString();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Read_ResponseBody_From_JavaScript_When_ContentType_Xml()
        {
            string javascriptText = "'my last response body is: ' + response.body";
            string expectedResult = "my last response body is: <xml />";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseJsonBody_From_JavaScript_When_ContentType_Json()
        {
            string javascriptText = 
                "'My friend ' + response.jsonbody.person.name + ' is ' + response.jsonbody.person.age + ' years old.'";
            string expectedResult = "My friend Bruce is 30 years old.";
            ReadJsonResponsePropertyInJavaScript(javascriptText, expectedResult, ContentType.JSON);
        }

        [TestMethod]
        public void Should_Read_ResponseJsonBody_From_JavaScript_For_Content_Appears_ToBe_Json()
        {
            string javascriptText =
                "'My friend ' + response.jsonbody.person.name + ' is ' + response.jsonbody.person.age + ' years old.'";
            string expectedResult = "My friend Bruce is 30 years old.";
            ReadJsonResponsePropertyInJavaScript(javascriptText, expectedResult, ContentType.TEXT);
        }

        [TestMethod]
        public void Should_Read_ResponseResource_From_JavaScript()
        {
            string javascriptText = "'my last response resource is: ' + response.resource";
            string expectedResult = "my last response resource is: /resources";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseStatusCode_From_JavaScript()
        {
            string javascriptText = "'my last response statusCode is: ' + response.statusCode";
            string expectedResult = "my last response statusCode is: 200";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseStatusText_From_JavaScript()
        {
            string javascriptText = "'my last response statusText is: ' + response.statusText";
            string expectedResult = "my last response statusText is: OK";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseTransactionId_From_JavaScript()
        {
            string javascriptText = "'my last response transactionId is: ' + response.transactionId";
            string expectedResult = "my last response transactionId is: 123456789";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_ContentType_From_JavaScript()
        {
            string javascriptText = "'my last response Content-Type is: ' + response.header('Content-Type')";
            string expectedResult = "my last response Content-Type is: application/xml";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_ContentLength_From_JavaScript()
        {
            string javascriptText = "'my last response Content-Length is: ' + response.header('Content-Length')";
            string expectedResult = "my last response Content-Length is: 7";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_Repeated_FirstValue_From_JavaScript()
        {
            string javascriptText = "'my last response Bespoke-Header[0] is: ' + response.header0('Bespoke-Header')";
            string expectedResult = "my last response Bespoke-Header[0] is: jolly";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_Repeated_SecondValue_From_JavaScript()
        {
            string javascriptText = "'my last response Bespoke-Header[1] is: ' + response.header('Bespoke-Header', 1)";
            string expectedResult = "my last response Bespoke-Header[1] is: good";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_Repeated_AllValues_From_JavaScript()
        {
            // response.headers(...) returns an array of headers with the specified name.
            string javascriptText =
@"var resultString = 'my last response Bespoke-Header values are:';
var arrayLength = response.headerListSize('Bespoke-Header');
var multiHeaders = response.headers('Bespoke-Header');
for (var i = 0; i < arrayLength; i++) {
    resultString += ' [' + i + ']:' + multiHeaders[i];
}
resultString;";
            string expectedResult = "my last response Bespoke-Header values are: [0]:jolly [1]:good";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_Repeated_Text_From_JavaScript()
        {
            // response.headersText(...) returns a string representation of an array of headers 
            //  with the specified name.
            string javascriptText = "'my last response Bespoke-Header: ' + response.headersText('Bespoke-Header')";
            string expectedResult = "my last response Bespoke-Header: [jolly, good]";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_NonRepeated_Text_From_JavaScript()
        {
            string javascriptText = "'my last response Content-Length: ' + response.headersText('Content-Length')";
            string expectedResult = "my last response Content-Length: 7";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_Repeated_List_Size_From_JavaScript()
        {
            string javascriptText = "'my last response Bespoke-Header size is: ' + response.headerListSize('Bespoke-Header')";
            string expectedResult = "my last response Bespoke-Header size is: 2";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_NonRepeated_List_Size_From_JavaScript()
        {
            string javascriptText = "'my last response Content-Length size is: ' + response.headerListSize('Content-Length')";
            string expectedResult = "my last response Content-Length size is: 1";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Evaluate_ResponseHeader_NonExistent_As_Null_In_JavaScript()
        {
            string javascriptText = "'my last response does not have Non-Existent header: ' + response.header0('Non-Existent')";
            string expectedResult = "my last response does not have Non-Existent header: null";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Read_ResponseHeader_NonExistent_Text_As_Null_From_JavaScript()
        {
            string javascriptText = "'my last response Non-Existent: ' + response.headersText('Non-Existent')";
            string expectedResult = "my last response Non-Existent: null";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Evaluate_Large_Json_Text()
        {
            // Arrange.
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider();
            StringBuilder sb = new StringBuilder("{ \"content\" : \"");
            int size = 1024*1024*10;
            for (int i = 0; i < size; i++)
            {
                sb.Append("A");
            }
            sb.Append("\"}");
            string jsonBodyText = sb.ToString();
            RestResponse response = CreateResponse(ContentType.JSON, jsonBodyText);
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);
            int expectedResult = size;

            // Act.
            string javascriptText = "response.jsonbody.content.length";
            object rawResult = jswrapper.evaluateExpression(response, javascriptText);
            int actualResult = Convert.ToInt32(rawResult);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Handle_Null_ResponseObject()
        {
            // Arrange.
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider();
            RestResponse response = null;
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);
            string expectedResult = "response is null: true";

            // Act.
            string javascriptText = "'response is null: ' + (response == null)";
            object rawResult = jswrapper.evaluateExpression(response, javascriptText);
            string actualResult = rawResult.ToString();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_For_Null_JavaScript()
        {
            // Arrange.
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider();
            string json = null;
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);
            string expectedResult = null;

            // Act.
            string javascriptText = "null";
            object rawResult = jswrapper.evaluateExpression(json, javascriptText);
            string actualResult = (string)rawResult;

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Null_For_Null_JavaScript_And_Null_JsonString()
        {
            // Arrange.
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider();
            RestResponse response = null;
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);
            string expectedResult = null;

            // Act.
            string javascriptText = "null";
            object rawResult = jswrapper.evaluateExpression(response, javascriptText);
            string actualResult = (string)rawResult;

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        [ExpectedException(typeof(JavascriptException))]
        public void Should_Throw_JavascriptException_On_Error_In_JavaScript()
        {
            string javascriptText = "some invalid JavaScript";
            string expectedResult = "not important";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        [TestMethod]
        public void Should_Evaluate_Multiple_JavaScript_Statements()
        {
            string javascriptText =
@"var resultString = 'Iterating through the values in a header array: ';
var arrayLength = response.headerListSize('Bespoke-Header');
var multiHeaders = response.headers('Bespoke-Header');
for (var i = 0; i < arrayLength; i++) {
    if (i > 0) resultString += ', ';
    resultString += multiHeaders[i];
}
resultString;";
            string expectedResult = "Iterating through the values in a header array: jolly, good";
            ReadXmlResponsePropertyInJavaScript(javascriptText, expectedResult);
        }

        private void ReadXmlResponsePropertyInJavaScript(string javascriptText, 
            string expectedResult)
        {
            RestResponse response = CreateResponse();
            ReadResponsePropertyInJavaScript(javascriptText, expectedResult, response);
        }

        private void ReadJsonResponsePropertyInJavaScript(string javascriptText, 
            string expectedResult, ContentType contentType)
        {
            RestResponse response = CreateJsonResponse(contentType);
            ReadResponsePropertyInJavaScript(javascriptText, expectedResult, response);
        }

        private void ReadResponsePropertyInJavaScript(string javascriptText,
            string expectedResult, RestResponse response)
        {
            // Arrange.
            IRunnerVariablesProvider variablesProvider = GetVariablesProvider();
            JavascriptWrapper jswrapper = new JavascriptWrapper(variablesProvider);

            // Act.
            object rawResult = jswrapper.evaluateExpression(response, javascriptText);
            string actualResult = rawResult.ToString();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        private RestResponse CreateJsonResponse(ContentType contentType)
        {
            string jsonBodyText = GetJsonString();
            return CreateResponse(contentType, jsonBodyText);
        }

        private RestResponse CreateResponse()
        {
            return CreateResponse(ContentType.XML, "<xml />");
        }

        private RestResponse CreateResponse(ContentType contentType, String body)
        {
            RestResponse response = new RestResponse();
            response.Resource = "/resources";
            response.StatusCode = 200;
            response.StatusText = "OK";
            response.Body = body;
            response.addHeader("Content-Type", contentType.toMime()[0]);
            response.addHeader("Bespoke-Header", "jolly");
            response.addHeader("Bespoke-Header", "good");
            response.addHeader("Content-Length", "7");
            response.TransactionId = 123456789L;
            return response;
        }

        private IRunnerVariablesProvider GetVariablesProvider()
        {
            FitVariables variables = new FitVariables();
            IRunnerVariablesProvider variablesProvider =
                Mock.Of<IRunnerVariablesProvider>(varProv =>
                    varProv.CreateRunnerVariables() == variables);
            return variablesProvider;
        }

        private IRunnerVariablesProvider GetVariablesProvider(FitVariables variables)
        {
            IRunnerVariablesProvider variablesProvider =
                Mock.Of<IRunnerVariablesProvider>(varProv =>
                    varProv.CreateRunnerVariables() == variables);
            return variablesProvider;
        }

        private string GetJsonString()
        {
            return "{ \"person\" : { \"name\" : \"Bruce\", \"age\" : \"30\" } }";
        }
    }
}