using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.StringToolsTests
{
    [TestClass]
    public class Tools_Regex
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
    }
}
