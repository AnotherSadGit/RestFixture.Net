using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Tools;

namespace RestFixture.Net.UnitTests.HtmlToolsTests
{
    [TestClass]
    public class HtmlTools_ToHtmlLink
    {
        [TestMethod]
        public void Should_Enclose_Text_In_Anchor_Tags()
        {
            // Arrange.
            string textToEnclose = "x";
            string linkUrl = "http://localhost:1234";
            string expectedResult = "<a href='http://localhost:1234'>x</a>";

            // Act.
            string actualResult = HtmlTools.toHtmlLink(linkUrl, textToEnclose);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}