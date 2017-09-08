using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.XmlToolsTests
{
    [TestClass]
    public class XmlTools_IsValidJson
    {
        [TestMethod]
        public void Should_Return_True_For_Empty_Braces()
        {
            // Arrange.
            string stringToTest = "{ }";
            bool expectedResult = true;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        [TestMethod]
        public void Should_Return_True_For_Empty_SquareBrackets()
        {
            // Arrange.
            string stringToTest = "[ ]";
            bool expectedResult = true;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        [TestMethod]
        public void Should_Return_True_For_Valid_JsonObject()
        {
            // Arrange.
            string stringToTest = "{ \"name\" : \"Joe Bloggs\", \"age\" : 32 }";
            bool expectedResult = true;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        [TestMethod]
        public void Should_Return_False_For_JsonObject_Missing_Closing_Brace()
        {
            // Arrange.
            string stringToTest = "{ \"name\" : \"Joe Bloggs\", \"age\" : 32 ";
            bool expectedResult = false;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        [TestMethod]
        public void Should_Return_True_For_Valid_JsonArray()
        {
            // Arrange.
            string stringToTest = "[10, 20, 30]";
            bool expectedResult = true;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        [TestMethod]
        public void Should_Return_False_For_JsonArray_Missing_Closing_Bracket()
        {
            // Arrange.
            string stringToTest = "[10, 20, 30";
            bool expectedResult = false;

            // Act & Assert.
            TestStringIsValidJson(stringToTest, expectedResult);
        }

        private void TestStringIsValidJson(string stringToTest, bool expectedResult)
        {
            // Act.
            bool actualResult = XmlTools.isValidJson(stringToTest);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}