using restFixture.Net.Support;
using RestClient.Data;

namespace UnitTests.ContentTypeTests
{
    public class ContentTypeTestBase
    {
        public void Reset()
        {
            RestData.DEFAULT_ENCODING = "UTF-8";
            ContentType.resetDefaultMapping();
        }
    }
}