using FriendBook.GroupService.API.Domain.JWT;
using System.Collections;

namespace FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources
{

    internal class IntegrationTestFixtureSource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { new AccessToken("Ilia", Guid.NewGuid()) };
            yield return new object[] { new AccessToken("Dima", Guid.NewGuid()) };
        }
    }
}
