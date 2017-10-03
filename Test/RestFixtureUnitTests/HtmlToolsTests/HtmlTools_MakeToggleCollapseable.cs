using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Tools;

namespace RestFixture.Net.UnitTests.HtmlToolsTests
{
    [TestClass]
    public class HtmlTools_MakeToggleCollapseable
    {
        [TestMethod]
        public void Should_Return_Div_with_CollapsibleClosed_Class()
        {
            //Arrange.
            string title = "title text";
            string content = "this is the content";
            string expectedStartTag = "<div class='collapsible closed'>";
            string expectedEndTag = "</div>";

            // Act.
            string actualResult = HtmlTools.makeToggleCollapseable(title, content);

            // Assert.
            Assert.IsTrue(actualResult.StartsWith(expectedStartTag));
            Assert.IsTrue(actualResult.EndsWith(expectedEndTag));
        }

        [TestMethod]
        public void Should_Include_Title_In_Paragraph_with_Title_Class()
        {
            //Arrange.
            string title = "title text";
            string content = "this is the content";
            string expectedTitleElement = "<p class='title'>title text</p>";

            // Act.
            string actualResult = HtmlTools.makeToggleCollapseable(title, content);

            // Assert.
            Assert.IsTrue(actualResult.Contains(expectedTitleElement));
        }

        [TestMethod]
        public void Should_Include_Content_Wrapped_In_Div()
        {
            //Arrange.
            string title = "title text";
            string content = "this is the content";
            string expectedContentDiv = "<div>this is the content</div>";

            // Act.
            string actualResult = HtmlTools.makeToggleCollapseable(title, content);

            // Assert.
            Assert.IsTrue(actualResult.Contains(expectedContentDiv));
        }
    }
}