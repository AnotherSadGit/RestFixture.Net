using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;

namespace UnitTests.ConfigTests
{
    [TestClass]
    public class Config_Add 
    {
        private Config _defaultConfig;
        private Config _namedConfig;

        [TestInitialize]
        public void SetUpConfigs()
        {
            _defaultConfig = Config.getConfig();
            _namedConfig = Config.getConfig("configName");
        }

        [TestMethod]
        public void Should_Add_NonNull_Element_To_DefaultConfig()
        {
            TestNewElementValue(_defaultConfig,
                (key, value, actualValue) => Assert.IsNotNull(actualValue, 
                                            "New element with key '{0}' not created.", key));
        }

        [TestMethod]
        public void Should_Add_Element_With_Specified_Value_To_DefaultConfig()
        {
            TestNewElementValue(_defaultConfig,
                (key, value, actualValue) => Assert.AreEqual(value, actualValue,
                                            "New element with key '{0}' has incorrect value.", key));
        }

        [TestMethod]
        public void Should_Add_NonNull_Element_To_NamedConfig()
        {
            TestNewElementValue(_namedConfig,
                (key, value, actualValue) => Assert.IsNotNull(actualValue,
                                            "New element with key '{0}' not created.", key));
        }

        [TestMethod]
        public void Should_Add_Element_With_Specified_Value_To_NamedConfig()
        {
            TestNewElementValue(_namedConfig,
                (key, value, actualValue) => Assert.AreEqual(value, actualValue,
                                            "New element with key '{0}' has incorrect value.", key));
        }

        [TestMethod]
        public void Should_Add_Element_With_Null_Value_To_Config()
        {
            TestNewElementValue(_namedConfig, "key1", null, 
                (key, value, actualValue) => Assert.IsNull(value,
                                            "New element with key '{0}' does not have null value.", 
                                            key));
        }

        private void TestNewElementValue(Config config, Action<string, string, string> assertion)
        {
            string key = "key1";
            string value = "value1";
            TestNewElementValue(config, key, value, assertion);
        }

        private void TestNewElementValue(Config config, string key, string value, 
            Action<string, string, string> assertion)
        {
            // Act.
            config.add(key, value);
            string actualValue = config.get(key);

            // Assert.
            assertion(key, value, actualValue);
        }
    }
}