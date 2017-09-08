using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Tools;

namespace RestFixtureUnitTests.XmlToolsTests
{
    [TestClass]
    public class XmlTools_FromJSONtoXML
    {
        [TestMethod]
        public void Should_Return_Xml_For_Valid_Json()
        {
            // Arrange.
            string jsonString =
                "{\"person\" : {\"address\" : { \"street\" : \"regent st\", \"number\" : \"1\"}, \"name\" : \"joe\", \"surname\" : \"bloggs\"} }";
            string expectedResult =
@"<person>
 <address>
  <street>regent st</street>
  <number>1</number>
 </address>
 <name>joe</name>
 <surname>bloggs</surname>
</person>";

            // Act.
            string actualResult = XmlTools.fromJSONtoXML(jsonString);

            // Assert.
            Assert.AreEqual("regent st", XmlTools.GetNodeValue(null, "/person/address/street", actualResult));
            Assert.AreEqual("1", XmlTools.GetNodeValue(null, "/person/address/number", actualResult));
            Assert.AreEqual("joe", XmlTools.GetNodeValue(null, "/person/name", actualResult));
            Assert.AreEqual("bloggs", XmlTools.GetNodeValue(null, "/person/surname", actualResult));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_Invalid_Json()
        {
            // Arrange.
            // Missing final brace: }
            string jsonString =
                "XX\"person\" : {\"address\" : { \"street\" : \"regent st\", \"number\" : \"1\"}, \"name\" : \"joe\", \"surname\" : \"bloggs\"} }";
            
            // Act.
            string actualResult = XmlTools.fromJSONtoXML(jsonString);
        }
    }
}