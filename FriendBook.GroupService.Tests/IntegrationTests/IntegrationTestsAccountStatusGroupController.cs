using FriendBook.GroupService.API.Domain.JWT;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    internal class IntegrationTestsAccountStatusGroupController : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/AccountStatusGroup";
        public IntegrationTestsAccountStatusGroupController(DataAccessToken dataAccessToken) : base(dataAccessToken)
        {
        }

        [Test]
        public async Task Create() 
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{IntegrationTestsGroupController.UrlController}/Create/{newGroup}", null);

        }
    }
}
