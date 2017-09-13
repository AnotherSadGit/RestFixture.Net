using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.HtmlToolsTests
{
    [TestClass]
    public class HtmlTools_ToCode
    {
        [TestMethod]
        public void Should_Enclose_Text_In_Code_Tags()
        {
            // Arrange.
            string textToEnclose = "x";
            string expectedResult = "<code>x</code>";

            // Act.
            string actualResult = HtmlTools.toCode(textToEnclose);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}