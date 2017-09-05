using restFixture.Net.Support;
using RestClient.Data;

namespace RestFixtureUnitTests.ContentTypeTests
{
    public class ContentTypeTestBase
    {
        public virtual void Reset()
        {
            RestData.DEFAULT_ENCODING = "UTF-8";
            ContentType.resetDefaultMapping();
        }
    }
}