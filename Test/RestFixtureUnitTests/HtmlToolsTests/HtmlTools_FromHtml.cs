using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Tools;

namespace RestFixture.Net.UnitTests.HtmlToolsTests
{
    [TestClass]
    public class HtmlTools_FromHtml
    {
        [TestMethod]
        public void Should_Convert_AngleBrackets()
        {
            // Arrange.
            string originalText = "&lt;a&gt;bb&lt;/a&gt;";
            string expectedText = "<a>bb</a>";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Remove_PreTags()
        {
            // Arrange.
            string originalText = "<pre>bb</pre>";
            string expectedText = "bb";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_LineBreak()
        {
            // Arrange.
            string originalText = "aa<br/>bb<br />cc<BR/>dd<BR />ee";
            string expectedText = "aa\nbb\ncc\ndd\nee";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Ampersand()
        {
            // Arrange.
            string originalText = "--&amp;--";
            string expectedText = "--&--";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Span()
        {
            // Arrange.
            string originalText = "<span>aa</span><span >bb</span><span attr=\"value\">cc</span>";
            string expectedText = "aabbcc";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Space()
        {
            // Arrange.
            string originalText = "aa&nbsp;bb";
            string expectedText = "aa bb";

            // Act.
            string actualText = HtmlTools.fromHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }
    }
}