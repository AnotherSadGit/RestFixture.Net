using System.Collections.Generic;
using Moq;
using restFixture.Net;
using restFixture.Net.Handlers;
using restFixture.Net.Support;
using restFixture.Net.Variables;
using RestClient.Data;

namespace RestFixtureUnitTests.LetHandlersTests
{
    public class LetHandlersTestBase
    {
        public IRunnerVariablesProvider VariablesProvider { get; set; }

        public void SetupVariablesProvider()
        {
            VariablesProvider = Mock.Of<IRunnerVariablesProvider>(varProv =>
                                    varProv.CreateRunnerVariables() == new FitVariables());
        }

        public string EvaluateExpressionAgainstResponse<T>(RestResponse response,
            string expression) where T : ILetHandler, new() 
        {
            // Arrange.
            T handler = new T();
            IDictionary<string, string> namespaceContext = new Dictionary<string, string>();

            // Act.
            string result = handler.handle(this.VariablesProvider, Config.getConfig(),
                response, namespaceContext, expression);

            return result;
        }
    }
}