using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using restFixture.Net;
using restFixture.Net.Support;
using restFixture.Net.TypeAdapters;

namespace RestFixtureUnitTests
{
    [TestClass]
    public class BodyTypeAdapterFactory_GetBodyTypeAdapter
    {
        private BodyTypeAdapterFactory _factory = null;
        private readonly string _charset = "UTF-8";

        public BodyTypeAdapterFactory Factory 
        {
            get
            {
                if (_factory == null)
                {
                    IRunnerVariablesProvider variablesProvider =
                        Mock.Of<IRunnerVariablesProvider>(varProv =>
                            varProv.CreateRunnerVariables() == null);

                    _factory = new BodyTypeAdapterFactory(variablesProvider, Config.getConfig());
                }

                return _factory;
            }
        }

        [TestMethod]
        public void Should_Return_JSONBodyTypeAdapter_For_ContentType_Json()
        {
            TestBodyTypeAdapter(ContentType.JSON, typeof (JSONBodyTypeAdapter));
        }

        [TestMethod]
        public void Should_Return_XPathBodyTypeAdapter_For_ContentType_Xml()
        {
            TestBodyTypeAdapter(ContentType.XML, typeof(XPathBodyTypeAdapter));
        }

        [TestMethod]
        public void Should_Return_TextBodyTypeAdapter_For_ContentType_Text()
        {
            TestBodyTypeAdapter(ContentType.TEXT, typeof(TextBodyTypeAdapter));
        }

        [TestMethod]
        public void Should_Return_XPathBodyTypeAdapter_For_ContentType_Unknown()
        {
            TestBodyTypeAdapter(ContentType.typeFor("unknown"), typeof(XPathBodyTypeAdapter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_ArgumentNullException_For_ContentType_Null()
        {
            // Act.
            BodyTypeAdapter bodyTypeAdapter =
                Factory.getBodyTypeAdapter(null, _charset);
        }

        private void TestBodyTypeAdapter(ContentType contentType, Type adapterType)
        {
            // Act.
            BodyTypeAdapter bodyTypeAdapter =
                Factory.getBodyTypeAdapter(contentType, _charset);

            // Assert.
            Assert.IsTrue(bodyTypeAdapter.GetType() == adapterType);
        }
    }
}