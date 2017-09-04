using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Handlers;
using restFixture.Net.Support;
using RestClient.Data;

namespace RestFixtureUnitTests.LetHandlersTests
{
    [TestClass]
    public class LetBodyJsHandler_Handle : LetHandlersTestBase
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
            string result = EvaluateExpressionAgainstResponse(response, expression);

            // Assert.
            Assert.IsNull(result);
        }

        private string EvaluateExpressionAgainstResponse(RestResponse response, string jsExpression)
        {
            return EvaluateExpressionAgainstResponse<LetBodyJsHandler>(response, jsExpression);
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