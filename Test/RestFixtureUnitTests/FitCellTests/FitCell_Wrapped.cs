using fit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.TableElements;
using smartrics.rest.fitnesse.fixture;

namespace RestFixtureUnitTests.FitCellTests
{
    [TestClass]
    public class FitCell_Wrapped
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
        public void Body_Should_Return_Contents_Of_SingleRow_SingleCell()
        {
            // Arrange.
            string expectedResult = "justone";

            // Act.
            string actualResult = _cell.Wrapped.Body;

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}