using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Support;

namespace RestFixture.Net.UnitTests.ConfigTests
{
    [TestClass]
    public class Config_ToString
    {
        private Config _defaultConfig;
        private Config _namedConfig;
        private readonly string _configName = "configName";

        [TestInitialize]
        public void SetUpConfigs()
        {
            _defaultConfig = Config.getConfig();
            _namedConfig = Config.getConfig(_configName);
        }

        [TestMethod]
        public void Should_Return_Name_For_Named_Config()
        {
            // Arrange.
            Config config = _namedConfig;

            // Act.
            string toStringValue = config.ToString();

            // Assert.
            Assert.IsTrue(toStringValue.Contains(_configName), "Did not return config name.");
        }

        [TestMethod]
        public void Should_Return_DefaultName_For_Default_Config()
        {
            // Arrange.
            Config config = _defaultConfig;

            // Act.
            string toStringValue = config.ToString();

            // Assert.
            Assert.IsTrue(toStringValue.Contains(_defaultConfig.Name), "Did not return config name.");
        }
    }
}