using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;

namespace RestFixtureUnitTests.ConfigTests
{
    [TestClass]
    public class Config_GetConfig 
    {
        [TestMethod]
        public void WithoutParameter_Should_Create_Config_With_DefaultName()
        {
            // Act.
            Config config = Config.getConfig();

            // Assert.
            Assert.IsNotNull(config);
            Assert.AreEqual(Config.DEFAULT_CONFIG_NAME, config.Name);
        }

        [TestMethod]
        public void WithNullParameter_Should_Create_Config_With_DefaultName()
        {
            // Act.
            Config config = Config.getConfig(null);

            // Assert.
            Assert.IsNotNull(config);
            Assert.AreEqual(Config.DEFAULT_CONFIG_NAME, config.Name);
        }

        [TestMethod]
        public void WithParameter_Should_Create_Config_With_SpecifiedName()
        {
            //Arrange.
            string configName = "configName";

            // Act.
            Config config = Config.getConfig(configName);

            // Assert.
            Assert.IsNotNull(config);
            Assert.AreEqual(configName, config.Name);
        }
    }
}