using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.StringToolsTests
{
    [TestClass]
    public class StringTools_Regex
    {
        [TestMethod]
        public void Should_Return_True_When_Regex_Is_Valid_And_Matches_Text()
        {
            // Arrange.
            string text = "200";
            string regexPattern = "200";

            // Act & Assert.
            Assert.IsTrue(StringTools.regex(text, regexPattern));
        }

        [TestMethod]
        public void Should_Return_False_When_Regex_Is_Valid_But_No_Match()
        {
            // Arrange.
            string text = "200";
            string regexPattern = "404";

            // Act & Assert.
            Assert.IsFalse(StringTools.regex(text, regexPattern));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_When_Regex_Is_Invalid()
        {
            // Arrange.
            string text = "200";
            string regexPattern = "40[]4";

            // Act & Assert.
            StringTools.regex(text, regexPattern);
        }

        [TestMethod]
        public void Should_Match_Simple_Url()
        {
            // Arrange.
            string text = "http://domain.com/resource";
            string regexPattern = "http://domain.com/resource";

            // Act & Assert.
            Assert.IsTrue(StringTools.regex(text, regexPattern));
        }

        [TestMethod]
        public void Should_Match_Url_With_Port()
        {
            // Arrange.
            string text = "http://domain.com:8080/resource";
            string regexPattern = "http://domain.com:8080/resource";

            // Act & Assert.
            Assert.IsTrue(StringTools.regex(text, regexPattern));
        }

        [TestMethod]
        public void Should_Match_Url_With_QueryString()
        {
            // Arrange.
            string text = "http://domain.com:8080/resource/1?blah=1&other=2";
            string regexPattern = @"http://domain.com:8080/resource/1\?blah=1&other=2";

            // Act & Assert.
            Assert.IsTrue(StringTools.regex(text, regexPattern));
        }
    }
}
