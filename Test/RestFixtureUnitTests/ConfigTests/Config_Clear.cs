using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestFixture.Net.Support;

namespace RestFixture.Net.UnitTests.ConfigTests
{
    [TestClass]
    public class Config_Clear
    {
        private Config _config;

        [TestInitialize]
        public void SetUpConfigs()
        {
            _config = Config.getConfig("configName");
        }

        [TestMethod]
        public void Should_Clear_Existing_Keys()
        {
            // Arrange.
            Config config = _config;
            // Other unit tests will fail if Add method doesn't work.
            string key1 = "key1";
            string value1 = "value1";
            string key2 = "key2";
            string value2 = "value2";
            config.add(key1, value1);
            config.add(key2, value2);

            // Act.
            config.clear();

            //Assert.
            string actualValue1 = config.get(key1);
            string actualValue2 = config.get(key2);
            Assert.IsNull(actualValue1, "Clear() did not remove existing key '{0}'.", key1);
            Assert.IsNull(actualValue2, "Clear() did not remove existing key '{0}'.", key2);
        }
    }
}