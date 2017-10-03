using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestFixture.Net.UnitTests.VariablesTests
{
    [TestClass]
    public class Variables_VariablesRegex
    {
        [TestMethod]
        public void Should_Match_Single_Hex_Letter_LowerCase()
        {
            CheckMatch("xxx %a% xxx");
        }

        [TestMethod]
        public void Should_Match_Single_Hex_Letter_UpperCase()
        {
            CheckMatch("xxx %A% xxx");
        }

        [TestMethod]
        public void Should_Match_Single_NonHex_Letter_LowerCase()
        {
            CheckMatch("xxx %g% xxx");
        }

        [TestMethod]
        public void Should_Match_Single_NonHex_Letter_UpperCase()
        {
            CheckMatch("xxx %G% xxx");
        }

        [TestMethod]
        public void Should_Match_Single_Underscore()
        {
            CheckMatch("xxx %_% xxx");
        }

        [TestMethod]
        public void Should_Match_LetterPair_One_Of_Which_Not_HexDigit()
        {
            CheckMatch("xxx %ag% xxx");
            CheckMatch("xxx %aG% xxx");
            CheckMatch("xxx %AG% xxx");
            CheckMatch("xxx %Ag% xxx");
            CheckMatch("xxx %ga% xxx");
            CheckMatch("xxx %Ga% xxx");
            CheckMatch("xxx %GA% xxx");
            CheckMatch("xxx %gA% xxx");
            CheckMatch("xxx %gg% xxx");
            CheckMatch("xxx %GG% xxx");
        }

        [TestMethod]
        public void Should_Match_NonHex_Letter_Followed_By_Digit()
        {
            CheckMatch("xxx %g1% xxx");
            CheckMatch("xxx %G1% xxx");
        }

        [TestMethod]
        public void Should_Match_Underscore_Followed_By_Another_Character()
        {
            CheckMatch("xxx %__% xxx");
            CheckMatch("xxx %_a% xxx");
            CheckMatch("xxx %_A% xxx");
            CheckMatch("xxx %_1% xxx");
        }

        [TestMethod]
        public void Should_Match_Letter_Followed_By_Underscore()
        {
            CheckMatch("xxx %a_% xxx");
            CheckMatch("xxx %A_% xxx");
        }

        [TestMethod]
        public void Should_Match_Three_HexDigits_With_First_NonNumeric()
        {
            CheckMatch("xxx %aaa% xxx");
            CheckMatch("xxx %Aaa% xxx");
            CheckMatch("xxx %aAa% xxx");
            CheckMatch("xxx %aaA% xxx");
            CheckMatch("xxx %a1a% xxx");
            CheckMatch("xxx %A1a% xxx");
            CheckMatch("xxx %a1A% xxx");
            CheckMatch("xxx %A1A% xxx");
            CheckMatch("xxx %aa1% xxx");
            CheckMatch("xxx %Aa1% xxx");
            CheckMatch("xxx %aA1% xxx");
            CheckMatch("xxx %AA1% xxx");
        }

        [TestMethod]
        public void Should_Match_Three_Characters_Including_Underscore()
        {
            CheckMatch("xxx %_aa% xxx");
            CheckMatch("xxx %_Aa% xxx");
            CheckMatch("xxx %_aA% xxx");
            CheckMatch("xxx %_AA% xxx");
            CheckMatch("xxx %_1a% xxx");
            CheckMatch("xxx %_1A% xxx");
            CheckMatch("xxx %_a1% xxx");
            CheckMatch("xxx %_A1% xxx");
            CheckMatch("xxx %a_1% xxx");
            CheckMatch("xxx %A_1% xxx");
            CheckMatch("xxx %a_a% xxx");
            CheckMatch("xxx %A_a% xxx");
            CheckMatch("xxx %a_A% xxx");
            CheckMatch("xxx %A_A% xxx");
        }

        [TestMethod]
        public void Should_Match_More_Than_Three_Characters()
        {
            CheckMatch("xxx %LongerText% xxx");
            CheckMatch("xxx %longer_text% xxx");
        }

        [TestMethod]
        public void Should_Not_Match_Single_Digit()
        {
            CheckNonMatch("xxx %1% xxx");
        }

        [TestMethod]
        public void Should_Not_Match_Pair_Of_HexDigits()
        {
            CheckNonMatch("xxx %12% xxx");
            CheckNonMatch("xxx %1A% xxx");
            CheckNonMatch("xxx %1a% xxx");
            CheckNonMatch("xxx %AF% xxx");
            CheckNonMatch("xxx %Af% xxx");
            CheckNonMatch("xxx %aF% xxx");
            CheckNonMatch("xxx %af% xxx");
            CheckNonMatch("xxx %A1% xxx");
            CheckNonMatch("xxx %a1% xxx");
        }

        [TestMethod]
        public void Should_Not_Match_Digit_Followed_By_One_Or_More_Characters()
        {
            CheckNonMatch("xxx %1G% xxx");
            CheckNonMatch("xxx %1g% xxx");
            CheckNonMatch("xxx %1_% xxx");
            CheckNonMatch("xxx %1aa% xxx");
            CheckNonMatch("xxx %1Aa% xxx");
            CheckNonMatch("xxx %1aA% xxx");
            CheckNonMatch("xxx %1AA% xxx");
            CheckNonMatch("xxx %1_a% xxx");
            CheckNonMatch("xxx %1a_% xxx");
        }

        private void CheckMatch(string textToTest)
        {
            // Arrange.
            string expectedFullMatch = textToTest.Replace("xxx", "").Trim();
            string expectedVariableName = expectedFullMatch.Replace("%", "");

            // Act.
            MatchCollection matches = Variables.Variables.VariablesRegex.Matches(textToTest);

            // Assert.
            Assert.IsTrue(matches.Count > 0, "No match found.");
            Match match = matches[0];
            GroupCollection groups = match.Groups;
            // First group is always the entire match so a match will always have at least one 
            //  group.
            Assert.IsTrue(groups.Count > 1, "Expected at least 2 match groups.");
            string textToSubstitute = groups[0].Value;
            Assert.AreEqual(expectedFullMatch, textToSubstitute, 
                "No match found on text to substitute.");
            string variableName = groups[1].Value;
            Assert.AreEqual(expectedVariableName, variableName, "No match found on variable name.");
        }

        private void CheckNonMatch(string textToTest)
        {
            // Act.
            MatchCollection matches = Variables.Variables.VariablesRegex.Matches(textToTest);

            // Assert.
            Assert.IsTrue(matches.Count == 0, "Unexpected match found.");
        }
    }
}