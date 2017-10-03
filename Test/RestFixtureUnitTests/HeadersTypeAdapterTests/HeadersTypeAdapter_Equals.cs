using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Data;
using RestFixture.Net.TypeAdapters;

namespace RestFixture.Net.UnitTests.HeadersTypeAdapterTests
{
    [TestClass]
    public class HeadersTypeAdapter_Equals
    {
        private HeadersTypeAdapter _adapter = new HeadersTypeAdapter();

        [TestMethod]
        public void Should_Return_False_For_Null_Equality()
        {
            // Act.
            bool actualResult = _adapter.Equals(null, null);

            // Assert.
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void Should_Return_True_When_Actual_Headers_Identical_To_Expected()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = GetHeadersList(3);
            IList<RestData.Header> actualHeaders = GetHeadersList(3);

            // Act.
            bool actualResult = _adapter.Equals(expectedHeaders, actualHeaders);

            // Assert.
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void Should_Return_True_When_Actual_Headers_Identical_But_Different_Order()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = 
                GetHeadersList(3).OrderByDescending(h => h.Name).ToList();
            IList<RestData.Header> actualHeaders = GetHeadersList(3);

            // Act.
            bool actualResult = _adapter.Equals(expectedHeaders, actualHeaders);

            // Assert.
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void Should_Return_True_When_Actual_Headers_Superset_Of_Expected()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = GetHeadersList(3);
            IList<RestData.Header> actualHeaders = GetHeadersList(5);

            // Act.
            bool actualResult = _adapter.Equals(expectedHeaders, actualHeaders);

            // Assert.
            Assert.IsTrue(actualResult);
        }

        [TestMethod]
        public void Should_Return_False_When_Actual_Headers_Subset_Of_Expected()
        {
            // Arrange.
            IList<RestData.Header> expectedHeaders = GetHeadersList(3);
            IList<RestData.Header> actualHeaders = GetHeadersList(2);

            // Act.
            bool actualResult = _adapter.Equals(expectedHeaders, actualHeaders);

            // Assert.
            Assert.IsFalse(actualResult);
        }

        [TestMethod]
        public void Should_Match_Expected_Header_Containing_Regex()
        {
            // Arrange.
            RestData.Header expectedContentTypeHeaderWithRegex =
                new RestData.Header("content-type", "application/my-xml;.+");
            RestData.Header actualContentTypeHeader =
                new RestData.Header("content-type", "application/my-xml;charset=UTF-8");

            IList<RestData.Header> expectedHeaders =
                new List<RestData.Header> { expectedContentTypeHeaderWithRegex };
            IList<RestData.Header> actualHeaders =
                new List<RestData.Header> { actualContentTypeHeader };

            // Act.
            bool actualResult = _adapter.Equals(expectedHeaders, actualHeaders);

            // Assert.
            Assert.IsTrue(actualResult);
        }

        private IList<RestData.Header> GetHeadersList(int numberOfHeaders)
        {
            IList<RestData.Header> headerList = new List<RestData.Header>();

            for (int i = 0; i < numberOfHeaders; i++)
            {
                headerList.Add(new RestData.Header("name" + i, "value" + i));
            }

            return headerList;
        }
    }
}