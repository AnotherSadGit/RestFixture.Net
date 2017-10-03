using RestClient.Data;
using RestFixture.Net.Support;

namespace RestFixture.Net.UnitTests.ContentTypeTests
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