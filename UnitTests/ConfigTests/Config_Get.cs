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

        [TestMethod]
        public void AsMap_Should_Return_Dictionary_For_Specified_Key()
        {
            // Arrange.
            string keyToRetrieve = "map";

            // Act.
            IDictionary<string, string> retrievedValue = GetMapFromConfig(keyToRetrieve);

            // Assert.
            Assert.IsNotNull(retrievedValue, "Retrieved dictionary is null.");
            Assert.AreEqual(5, retrievedValue.Keys.Count, 
                "Retrieved dictionary has the wrong number of elements.");
        }

        [TestMethod]
        public void AsMap_Should_Include_Elements_Converted_From_String()
        {
            // Arrange.
            string keyToRetrieve = "map";

            // Act.
            IDictionary<string, string> retrievedValue = GetMapFromConfig(keyToRetrieve);

            // Assert.
            Assert.IsTrue(retrievedValue.ContainsKey("a"),
                "Retrieved dictionary is missing key 'a'.");
            Assert.IsTrue(retrievedValue.ContainsKey("b"),
                "Retrieved dictionary is missing key 'b'.");
            Assert.AreEqual("1", retrievedValue["a"],
                "The value of element 'a' of the retrieved dictionary is incorrect.");
            Assert.AreEqual("2", retrievedValue["b"],
                "The value of element 'b' of the retrieved dictionary is incorrect.");
        }

        [TestMethod]
        public void AsMap_Should_Include_Elements_From_Default_Dictionary()
        {
            // Arrange.
            string keyToRetrieve = "map";

            // Act.
            IDictionary<string, string> retrievedValue = GetMapFromConfig(keyToRetrieve);

            // Assert.
            DefaultMapAssertions(retrievedValue);
        }

        [TestMethod]
        public void AsMap_Should_Return_Default_Dictionary_For_NonExistent_Key()
        {
            // Arrange.
            string keyToRetrieve = "non.existent.map";

            // Act.
            IDictionary<string, string> retrievedValue = GetMapFromConfig(keyToRetrieve);

            // Assert.
            Assert.IsNotNull(retrievedValue, "Retrieved dictionary is null.");
            Assert.AreEqual(3, retrievedValue.Keys.Count,
                "Retrieved dictionary has the wrong number of elements.");
            DefaultMapAssertions(retrievedValue);
        }

        private IDictionary<string, string> GetMapFromConfig(string keyToRetrieve)
        {
            // Arrange.
            Config config = _config;
            string key = "map";
            string value = "a=1\nb=2\n";
            config.add(key, value);
            Dictionary<string, string> defaultValue = new Dictionary<string, string>();
            defaultValue.Add("x", "10");
            defaultValue.Add("y", "11");
            defaultValue.Add("z", "12");

            // Act.
            IDictionary<string, string> retrievedValue = config.getAsMap(keyToRetrieve, defaultValue);
            return retrievedValue;
        }

        private void DefaultMapAssertions(IDictionary<string, string> retrievedValue)
        {
            Assert.IsTrue(retrievedValue.ContainsKey("x"),
                "Retrieved dictionary is missing key 'x'.");
            Assert.IsTrue(retrievedValue.ContainsKey("y"),
                "Retrieved dictionary is missing key 'y'.");
            Assert.IsTrue(retrievedValue.ContainsKey("z"),
                "Retrieved dictionary is missing key 'z'.");
            Assert.AreEqual("10", retrievedValue["x"],
                "The value of element 'x' of the retrieved dictionary is incorrect.");
            Assert.AreEqual("11", retrievedValue["y"],
                "The value of element 'y' of the retrieved dictionary is incorrect.");
            Assert.AreEqual("12", retrievedValue["z"],
                "The value of element 'z' of the retrieved dictionary is incorrect.");
        }
    }
}