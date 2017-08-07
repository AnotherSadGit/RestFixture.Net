using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;
using RestClient.Data;

namespace RestFixtureUnitTests.ContentTypeTests
{
    [TestClass]
    public class ContentType_Config : ContentTypeTestBase
    {
        [TestInitialize]
        [TestCleanup]
        public void Reset()
        {
            base.Reset();
        }

        [TestMethod]
        public void Should_Set_Default_Encoding_Specified_In_Config()
        {
            // Arrange.
            string newDefaultEncoding = "MY-CHARSET";
            Config c = Config.getConfig();
            c.add("restfixture.content.default.charset", newDefaultEncoding);

            // Act.
            ContentType.config(c);

            // Assert.
            Assert.AreEqual(RestData.DEFAULT_ENCODING, newDefaultEncoding, 
                "Default encoding was not set via ContentType.Config.");
        }

        [TestMethod]
        public void Should_Set_Default_Encoding_To_DefaultValue_For_Config_Value_Null()
        {
            // Arrange.
            RestData.DEFAULT_ENCODING = "MY-CHARSET";
            Config c = Config.getConfig();
            c.add("restfixture.content.default.charset", null);

            // Act.
            ContentType.config(c);

            // Assert.
            Assert.AreEqual(RestData.DEFAULT_ENCODING, Encoding.UTF8.HeaderName,
                "Default encoding was not set to ContentType.Config.");
        }

        [TestMethod]
        public void Should_Leave_Unspecified_ContentType_Mappings_Unchanged()
        {
            // Arrange & Act.
            string stringMappings = "application/xhtml=xml";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            IDictionary<string, ContentType> defaultMappings = ContentType.DefaultMappings;
            foreach (string contentType in defaultMappings.Keys)
            {
                Assert.AreEqual(defaultMappings[contentType],
                    ContentType.parse(contentType),
                    "Default content type mapping missing.");
            }
        }

        [TestMethod]
        public void Should_Set_Single_ContentType_Mapping_Specified_In_Config()
        {
            // Arrange & Act.
            string stringMappings = "application/xhtml=xml";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            Assert.AreEqual(ContentType.XML, ContentType.parse("application/xhtml"),
                "Custom content type mapping not added.");
        }

        [TestMethod]
        public void Should_Set_Multiple_ContentType_Mappings_Specified_In_Config()
        {
            // Arrange & Act.
            string stringMappings = "application/xhtml=xml\n"
                + "application/my-type=json";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            Assert.AreEqual(ContentType.XML, ContentType.parse("application/xhtml"),
                "Custom content type mapping not added.");
            Assert.AreEqual(ContentType.JSON, ContentType.parse("application/my-type"),
                "Custom content type mapping not added.");
        }

        [TestMethod]
        public void Should_Ignore_LeadingAndTrailingSpaces_In_ContentType_Mappings_Specified_In_Config()
        {
            // Arrange & Act.
            string stringMappings = " application/xhtml = xml \n "
                + " application/my-type = json ";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            Assert.AreEqual(ContentType.XML, ContentType.parse("application/xhtml"),
                "Custom content type mapping not added.");
            Assert.AreEqual(ContentType.JSON, ContentType.parse("application/my-type"),
                "Custom content type mapping not added.");
        }

        [TestMethod]
        public void Should_Override_Default_ContentType_Mapping_With_Value_Specified_In_Config()
        {
            // Arrange & Act.
            string stringMappings = "default = text";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            Assert.AreEqual(ContentType.TEXT, ContentType.parse("default"),
                "Invalid default content type mapping.");
            Assert.AreEqual(ContentType.TEXT, ContentType.parse("whatever"),
                "Invalid default content type mapping.");
        }

        [TestMethod]
        public void Should_Override_Multiple_ContentType_Mappings_With_Values_Specified_In_Config()
        {
            // Arrange & Act.
            string stringMappings = "default = text\n"
                + "text/plain=json";
            SetUpContentHandlerMappings(stringMappings);

            // Assert.
            Assert.AreEqual(ContentType.TEXT, ContentType.parse("whatever"),
                "Invalid default content type mapping.");
            Assert.AreEqual(ContentType.JSON, ContentType.parse("text/plain"),
                "Invalid text/plain content type mapping.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException_For_Invalid_ContentType_In_Config_Mappings()
        {
            // Arrange & Act.
            string stringMappings = "default = text\n"
                + "text/plain=invalid";
            SetUpContentHandlerMappings(stringMappings);
        }

        private void SetUpContentHandlerMappings(string stringMappings)
        {
            // Arrange.
            Config c = Config.getConfig();
            c.add("restfixture.content.handlers.map", stringMappings);

            // Act.
            ContentType.config(c);
        }
    }
}