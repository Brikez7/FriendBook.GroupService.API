using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGroupTaskController : BaseIntegrationTestsGroup
    {
        internal const string UrlController = $"{UrlAPI}/GroupTask";
        public IntegrationTestsGroupTaskController(DataAccessToken dataAccessToken) : base(dataAccessToken){}
    }
}
