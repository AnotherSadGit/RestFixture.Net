using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Tools;

namespace RestFixture.Net.UnitTests.StringToolsTests
{
    [TestClass]
    public class StringTools_ConvertStringToMap
    {
        [TestMethod]
        public void Should_Convert_String_Without_EntrySeparator_To_Single_Dictionary_Entry()
        {
            // Arrange.
            string stringToConvert = "k1~v1";
            string kvPairSeparator = "~";
            string entrySeparator = "##";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Convert_String_With_EntrySeparator_To_Multiple_Dictionary_Entries()
        {
            // Arrange.
            string stringToConvert = "k1~v1##k2~v2##k3~v3";
            string kvPairSeparator = "~";
            string entrySeparator = "##";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");
            expectedResult.Add("k3", "v3");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Convert_Missing_Value_To_Empty_String()
        {
            // Arrange.
            string stringToConvert = "k1~";
            string kvPairSeparator = "~";
            string entrySeparator = "##";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Remove_HtmlTag_From_Resultant_Value_When_CleanTags_Set()
        {
            // Arrange.
            string stringToConvert = "k1=v1\nk2=<a href=\"2\">v2</a>";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = true;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Not_Remove_HtmlTag_From_Resultant_Value_When_CleanTags_Cleared()
        {
            // Arrange.
            string stringToConvert = "k1=v1\nk2=<a href=\"2\">v2</a>";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "<a href=\"2\">v2</a>");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Read_Plain_Values_Correctly_When_CleanTags_Set()
        {
            // Arrange.
            string stringToConvert = "k1=v1\nk2=v2";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = true;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Convert_String_MarkedUp_As_Multiline()
        {
            // Arrange.
            string stringToConvert = "!- k1=v1\nk2=v2-!";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Convert_String_With_EntrySeparator_Enclosed_In_Multiline_Markup()
        {
            // Arrange.
            string stringToConvert = "k1=v1!-\n-!k2=v2";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Trim_Leading_And_Trailing_Spaces_From_Keys()
        {
            // Arrange.
            string stringToConvert = " k1=v1\nk2 =v2";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Trim_Leading_And_Trailing_Spaces_From_Values()
        {
            // Arrange.
            string stringToConvert = "k1= v1\nk2=v2 ";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        [TestMethod]
        public void Should_Ignore_Empty_Lines()
        {
            // Arrange.
            string stringToConvert = "k1=v1\n\n\nk2=v2";
            string kvPairSeparator = "=";
            string entrySeparator = "\n";
            bool cleanTags = false;
            IDictionary<string, string> expectedResult = new Dictionary<string, string>();
            expectedResult.Add("k1", "v1");
            expectedResult.Add("k2", "v2");

            // Act & Assert.
            CheckConversion(stringToConvert, kvPairSeparator, entrySeparator, cleanTags,
                expectedResult);
        }

        private void CheckConversion(string stringToConvert, string kvPairSeparator,
            string entrySeparator, bool cleanTags, IDictionary<string, string> expectedResult)
        {
            // Act.
            IDictionary<string, string> actualResult =
                StringTools.convertStringToMap(stringToConvert, kvPairSeparator, entrySeparator, cleanTags);

            // Assert.
            Assert.IsNotNull(actualResult, "Result of conversion is null.");
            Assert.AreEqual(expectedResult.Keys.Count, actualResult.Keys.Count,
                "Incorrect number of entries in resultant dictionary.");
            foreach (string key in expectedResult.Keys)
            {
                Assert.IsTrue(actualResult.ContainsKey(key),
                    "Expected key '{0}' is missing from resultant dictionary.", key);
                Assert.AreEqual(expectedResult[key], actualResult[key],
                    "Value for key '{0}' in resultant dictionary is incorrect.", key);
            }
        }
    }
}