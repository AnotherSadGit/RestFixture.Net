using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Data;
using RestFixture.Net.TypeAdapters;

namespace RestFixture.Net.UnitTests.HeadersTypeAdapterTests
{
    [TestClass]
    public class HeadersTypeAdapter_Parse 
    {
        private HeadersTypeAdapter _adapter = new HeadersTypeAdapter();

        [TestMethod]
        public void Should_Parse_Headers_From_Html_String()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = new List<RestData.Header>();

            for (int i = 0; i < 3; i++)
            {
                expectedHeaders.Add(new RestData.Header("n" + i, "v" + i));
            }

            string headersHtmlString = " n0 : v0 <br/> n1: v1 <br  /> n2 :v2 ";
            IList<RestData.Header> parsedHeaders =
                (IList<RestData.Header>)_adapter.parse(headersHtmlString);

            // Act.
            bool result = _adapter.Equals(expectedHeaders, parsedHeaders);

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Should_Parse_Headers_From_Html_String_Containing_Colons()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = new List<RestData.Header>();
            expectedHeaders.Add(new RestData.Header("n", "http://something:port/blah:blah"));
            expectedHeaders.Add(new RestData.Header("n1", "v1"));
            string headersHtmlString = " n : http://something:port/blah:blah <br/> n1: v1 ";
            IList<RestData.Header> parsedHeaders =
                (IList<RestData.Header>)_adapter.parse(headersHtmlString);

            // Act.
            bool result = _adapter.Equals(expectedHeaders, parsedHeaders);

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Should_Parse_Headers_From_Html_String_Containing_QueryString()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = new List<RestData.Header>();
            expectedHeaders.Add(new RestData.Header("n", "http://something:port/blah/1?blah=1&other=2"));
            expectedHeaders.Add(new RestData.Header("n1", "v1"));
            string headersHtmlString = " n : http://something:port/blah/1?blah=1&other=2 <br/> n1: v1 ";
            IList<RestData.Header> parsedHeaders =
                (IList<RestData.Header>)_adapter.parse(headersHtmlString);

            // Act.
            bool result = _adapter.Equals(expectedHeaders, parsedHeaders);

            // Assert.
            Assert.IsTrue(result);
        }
    }
}