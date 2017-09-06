using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.HtmlToolsTests
{
    [TestClass]
    public class HtmlTools_FromSimpleTag
    {
        [TestMethod]
        public void Should_Extract_Text_From_Tag()
        {
            // Arrange.
            string textWithTags = "<bob data='1'>stuff</bob>";
            string expectedResult = "stuff";

            // Act.
            string actualResult = HtmlTools.fromSimpleTag(textWithTags);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}