using System.Collections.Generic;

namespace RestFixture.Net.FitNesseTestServer.Test.FitNesse.Fixture.HttpRequestHandlers
{
    /// <summary>
    /// HTTP methods supported by the HttpServer.
    /// </summary>
    internal static class HttpMethod
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";

        public static readonly IList<string> List = new List<string>
        {
            Get,
            Post,
            Put,
            Delete
        };
    }
}