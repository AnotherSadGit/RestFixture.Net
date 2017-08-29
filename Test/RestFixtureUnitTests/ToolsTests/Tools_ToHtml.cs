using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;

namespace RestFixtureUnitTests.ToolsTests
{
    [TestClass]
    public class Tools_ToHtml
    {
        [TestMethod]
        public void Should_Convert_AngleBrackets()
        {
            // Arrange.
            string originalText = "<a>bb</a>";
            string expectedText = "&lt;a&gt;bb&lt;/a&gt;";

            // Act.
            string actualText = Tools.toHtml(originalText);

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
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_CarriageReturnLinefeed()
        {
            // Arrange.
            string originalText = "aa\r\nbb";
            string expectedText = "aa<br/>bb";

            // Act.
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Linefeed()
        {
            // Arrange.
            string originalText = "aa\nbb";
            string expectedText = "aa<br/>bb";

            // Act.
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Tab()
        {
            // Arrange.
            string originalText = "aa\tbb";
            // Converted to spaces then the spaces are subsequently converted to "&nbsp;".
            string expectedText = "aa&nbsp;&nbsp;&nbsp;&nbsp;bb";

            // Act.
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_Space()
        {
            // Arrange.
            string originalText = "aa bb";
            string expectedText = "aa&nbsp;bb";

            // Act.
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void Should_Convert_HorizontalLine()
        {
            // Arrange.
            string originalText = "aa-----bb";
            string expectedText = "aa<hr/>bb";

            // Act.
            string actualText = Tools.toHtml(originalText);

            // Assert.
            Assert.AreEqual(expectedText, actualText);
        }
    }
}