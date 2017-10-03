using fit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.TableElements;
using RestFixture.Net.UnitTests.Helpers;

namespace RestFixture.Net.UnitTests.FitCellTests
{
    [TestClass]
    public class FitCell_AddToBody
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
        public void Should_Append_To_Existing_Body()
        {
            // Arrange.
            string expectedResult = "justone<span class=\"fit_grey\"> _more</span>";

            // Act.
            _cell.addToBody("_more");
            string actualResult = _cell.body();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}