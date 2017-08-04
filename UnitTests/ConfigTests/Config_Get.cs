using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;

namespace UnitTests.ConfigTests
{
    [TestClass]
    public class Config_Get 
    {
        private Config _config;

        [TestInitialize]
        public void SetUpConfigs()
        {
            _config = Config.getConfig("configName");
        }

        [TestMethod]
        public void Should_Return_Null_For_NonExistent_Key()
        {
            // Arrange.
            Config config = _config;

            // Act.
            string value = config.get("non.existent.key");

            // Assert.
            Assert.IsNull(value, "Value for non-existent key is non-null.");
        }

        [TestMethod]
        public void Overload_With_Default_Should_Return_Default_For_NonExistent_Key()
        {
            // Arrange.
            Config config = _config;
            string defaultValue = "value";

            // Act.
            string value = config.get("non.existent.key", defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, value, "Did not return default value for non-existent key.");
        }

        [TestMethod]
        public void AsLong_Should_Return_Correct_Long_For_Specified_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "long";
            long value = 100L;
            long defaultValue = 10L;
            config.add(key, value.ToString());

            // Act.
            long retrievedValue = config.getAsLong(key, defaultValue);

            // Assert.
            Assert.AreEqual(value, retrievedValue);
        }

        [TestMethod]
        public void AsLong_Should_Return_Default_For_NonLong_Value()
        {
            // Arrange.
            Config config = _config;
            string key = "non.long";
            long defaultValue = 10L;
            config.add(key, "x");

            // Act.
            long retrievedValue = config.getAsLong(key, defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, retrievedValue, 
                "Did not return default for non-long config value.");
        }

        [TestMethod]
        public void AsLong_Should_Return_Default_For_NonExistent_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "non.existent.long";
            long defaultValue = 10L;

            // Act.
            long retrievedValue = config.getAsLong(key, defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, retrievedValue,
                "Did not return default for non-existent config value.");
        }

        [TestMethod]
        public void AsInteger_Should_Return_Correct_Int_For_Specified_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "int";
            int value = 100;
            int defaultValue = 10;
            config.add(key, value.ToString());

            // Act.
            int retrievedValue = config.getAsInteger(key, defaultValue);

            // Assert.
            Assert.AreEqual(value, retrievedValue);
        }

        [TestMethod]
        public void AsInteger_Should_Return_Default_For_NonLong_Value()
        {
            // Arrange.
            Config config = _config;
            string key = "non.int";
            int defaultValue = 10;
            config.add(key, "x");

            // Act.
            long retrievedValue = config.getAsInteger(key, defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, retrievedValue,
                "Did not return default for non-long config value.");
        }

        [TestMethod]
        public void AsInteger_Should_Return_Default_For_NonExistent_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "non.existent.int";
            int defaultValue = 10;

            // Act.
            long retrievedValue = config.getAsInteger(key, defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, retrievedValue,
                "Did not return default for non-existent config value.");
        }

        [TestMethod]
        public void AsBoolean_Should_Return_Correct_Bool_For_Specified_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "bool";
            bool value = false;
            bool defaultValue = true;
            config.add(key, value.ToString());

            // Act.
            bool retrievedValue = config.getAsBoolean(key, defaultValue);

            // Assert.
            Assert.AreEqual(value, retrievedValue);
        }

        [TestMethod]
        public void AsBoolean_Should_Return_Default_For_NonLong_Value()
        {
            // Arrange.
            Config config = _config;
            string key = "non.bool";
            bool defaultValue = true;
            config.add(key, "x");

            // Act.
            bool retrievedValue = config.getAsBoolean(key, defaultValue);

            // Assert.
            // NOTE: This is not the same result as the Java implementation of getAsBoolean 
            //  returns.  That parses "x" as false.  The .NET implementation checks if the 
            //  value is a valid boolean string.  If not it returns the default value, which 
            //  seems more logical.
            Assert.AreEqual(defaultValue, retrievedValue,
                "Did not return default for non-long config value.");
        }

        [TestMethod]
        public void AsBoolean_Should_Return_Default_For_NonExistent_Key()
        {
            // Arrange.
            Config config = _config;
            string key = "non.existent.bool";
            bool defaultValue = true;

            // Act.
            bool retrievedValue = config.getAsBoolean(key, defaultValue);

            // Assert.
            Assert.AreEqual(defaultValue, retrievedValue,
                "Did not return default for non-existent config value.");
        }

        //[TestMethod]
        //public void AsMap_Should_Return_Correct_Dictionary_For_Specified_Key()
        //{
        //    // Arrange.
        //    Config config = _config;
        //    string key = "map";
        //    string value = "a=1\nb=2\n";
        //    Dictionary<string, string> defaultValue = new Dictionary<string, string>();
        //    defaultValue.Add("c", "3");

        //    // Act.
        //    IDictionary<string, string> retrievedValue = config.getAsMap(key, defaultValue);

        //    // Assert.
        //    Assert.IsNotNull(retrievedValue, "Retrieved dictionary is null.");
        //    Assert.AreEqual(defaultValue, retrievedValue,
        //        "Did not return default for non-existent config value.");
        //}
    }
}