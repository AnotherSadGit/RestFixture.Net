using Microsoft.VisualStudio.TestTools.UnitTesting;
using restFixture.Net.Support;
using RestClient;

namespace RestFixtureUnitTests
{
    [TestClass]
    public class RestClientBuilder_CreateRestClient
    {
        [TestMethod]
        public void Should_Set_ReadWriteTimeout_To_Default_If_Config_Null()
        {
            // Arrange.
            Config config = null;
            int expectedReadWriteTimeout = RestClientBuilder.DEFAULT_READWRITE_TIMEOUT;

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.AreEqual(expectedReadWriteTimeout, restClient.ReadWriteTimeout);
        }

        [TestMethod]
        public void Should_Set_ReadWriteTimeout_To_Value_In_Config()
        {
            // Arrange.
            Config config = Config.getConfig();
            string clientTimeoutKey = "http.client.connection.timeout";
            config.add(clientTimeoutKey, "1500");
            int expectedReadWriteTimeout = config.getAsInteger(clientTimeoutKey,
                RestClientBuilder.DEFAULT_READWRITE_TIMEOUT);

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.AreEqual(expectedReadWriteTimeout, restClient.ReadWriteTimeout);
        }

        [TestMethod]
        public void Should_Not_Set_Proxy_When_ProxyHost_Not_Supplied()
        {
            // Arrange.
            Config config = GetConfigWithProxyInfo();
            config.remove("http.proxy.host");

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNull(restClient.Proxy);
        }

        [TestMethod]
        public void Should_Set_Proxy_When_ProxyHost_Supplied()
        {
            // Arrange.
            Config config = GetConfigWithProxyInfo();
            int expectedPort =
                config.getAsInteger("http.proxy.port", RestClientBuilder.DEFAULT_PROXY_PORT);

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNotNull(restClient.Proxy, "Proxy should not be null.");
            Assert.AreEqual(config.get("http.proxy.host"), restClient.Proxy.Address);
            Assert.AreEqual(expectedPort, restClient.Proxy.Port);
            Assert.AreEqual(config.get("http.proxy.username"), restClient.Proxy.UserName);
            Assert.AreEqual(config.get("http.proxy.password"), restClient.Proxy.Password);
            Assert.AreEqual(config.get("http.proxy.domain"), restClient.Proxy.Domain);
        }

        [TestMethod]
        public void Should_Set_ProxyPort_To_Default_When_No_ProxyPort_Supplied()
        {
            // Arrange.
            Config config = GetConfigWithProxyInfo();
            config.remove("http.proxy.port");
            int expectedPort = RestClientBuilder.DEFAULT_PROXY_PORT;

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNotNull(restClient.Proxy, "Proxy should not be null.");
            Assert.AreEqual(expectedPort, restClient.Proxy.Port);
        }

        [TestMethod]
        public void Should_Set_Credentials_When_UserName_And_Password_Supplied()
        {
            // Arrange.
            Config config = GetConfigWithCredentials();

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNotNull(restClient.Credentials, "Credentials should not be null.");
            Assert.AreEqual(config.get("http.basicauth.username"), restClient.Credentials.UserName);
            Assert.AreEqual(config.get("http.basicauth.password"), restClient.Credentials.Password);
        }

        [TestMethod]
        public void Should_Not_Set_Credentials_When_UserName_Missing()
        {
            // Arrange.
            Config config = GetConfigWithCredentials(); ;
            config.remove("http.basicauth.username");

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNull(restClient.Credentials);
        }

        [TestMethod]
        public void Should_Not_Set_Credentials_When_Password_Missing()
        {
            // Arrange.
            Config config = GetConfigWithCredentials(); ;
            config.remove("http.basicauth.password");

            // Act.
            IRestClient restClient = new RestClientBuilder().createRestClient(config);

            // Assert.
            Assert.IsNotNull(restClient, "RestClient should not be null.");
            Assert.IsNull(restClient.Credentials);
        }

        private Config GetConfigWithProxyInfo()
        {
            Config config = Config.getConfig();
            config.add("http.proxy.host", "proxyHost");
            config.add("http.proxy.port", "1234");
            config.add("http.proxy.username", "proxyUsername");
            config.add("http.proxy.password", "proxyPassword");
            config.add("http.proxy.domain", "proxyDomain");
            return config;
        }

        private Config GetConfigWithCredentials()
        {
            Config config = Config.getConfig();
            config.add("http.basicauth.username", "basicAuthUsername");
            config.add("http.basicauth.password", "basicAuthPassword");
            return config;
        }
    }
}