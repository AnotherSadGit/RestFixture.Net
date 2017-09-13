using fit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.TableElements;
using smartrics.rest.fitnesse.fixture;

namespace RestFixtureUnitTests.FitCellTests
{
    [TestClass]
    public class FitCell_Body
    {
        private FitCell _cell = null;

        [TestInitialize]
        public void SetupCell()
        {
            RestFixtureTestHelper helper = new RestFixtureTestHelper();
            Parse table = helper.createSingleRowFitTable("justone");
            _cell = new FitCell(table.Parts.Parts);
        }

        [TestMethod]
        public void Should_Return_Contents_Of_SingleRow_SingleCell()
        {
            // Arrange.
            string expectedResult = "justone";

            // Act.
            string actualResult = _cell.body();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Should_Return_Same_As_TextMethod()
        {
            // Arrange.
            string expectedResult = _cell.text();

            // Act.
            string actualResult = _cell.body();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void Override_Taking_Argument_Should_Set_Content()
        {
            // Arrange.
            string expectedResult = "another";

            // Act.
            _cell.body(expectedResult);
            string actualResult = _cell.body();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}