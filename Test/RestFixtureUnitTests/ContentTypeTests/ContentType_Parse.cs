using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Data;
using RestFixture.Net.Support;

namespace RestFixture.Net.UnitTests.ContentTypeTests
{
    [TestClass]
    public class ContentType_Parse : ContentTypeTestBase
    {
        [TestInitialize]
        [TestCleanup]
        public override void Reset()
        {
            base.Reset();
        }

        [TestMethod]
        public void Should_Return_ContentTypeXml_For_ContentTypeHeader_ApplicationXml()
        {
            TestContentType("application/xml", ContentType.XML);
        }

        [TestMethod]
        public void Should_Return_ContentTypeJson_For_ContentTypeHeader_ApplicationJson()
        {
            TestContentType("application/json", ContentType.JSON);
        }

        [TestMethod]
        public void Should_Return_ContentTypeJavaScript_For_ContentTypeHeader_ApplicationXJavaScript()
        {
            TestContentType("application/x-javascript", ContentType.JS);
        }

        [TestMethod]
        public void Should_Return_ContentTypeText_For_ContentTypeHeader_TextPlain()
        {
            TestContentType("text/plain", ContentType.TEXT);
        }

        [TestMethod]
        public void Should_Return_ContentTypeXml_For_ContentTypeHeader_Unrecognised()
        {
            TestContentType("whatever", ContentType.XML);
        }

        [TestMethod]
        public void Should_Return_ContentTypeXml_For_ContentTypeHeader_Missing()
        {
            TestContentType(null, ContentType.XML);
        }

        private void TestContentType(string contentTypeHeaderValue, ContentType expectedContentType)
        {
            // Arrange.
            RestResponse response = new RestResponse();
            if (contentTypeHeaderValue != null)
            {
                response.addHeader("Content-Type", contentTypeHeaderValue);
            }

            // Act.
            ContentType parsedContentType = ContentType.parse(response.ContentType);

            // Assert.
            Assert.AreEqual(expectedContentType, parsedContentType, 
                "Parsed content type is incorrect.");
        }
    }
}