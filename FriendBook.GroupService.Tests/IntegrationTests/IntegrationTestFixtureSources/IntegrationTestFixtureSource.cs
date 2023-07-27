using FriendBook.GroupService.API.Domain.JWT;
using System.Collections;

namespace FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources
{

    internal class IntegrationTestFixtureSource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new DataAccessToken("Ilia", Guid.NewGuid()) };
            yield return new object[] { new DataAccessToken("Dima", Guid.NewGuid()) };
        }
    }
}
