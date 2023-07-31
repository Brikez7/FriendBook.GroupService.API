using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.Tests.TestHelpers;

namespace FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests
{
    internal abstract class BaseIntegrationTestsGroup : BaseIntegrationTests
    {
        private protected ResponseGroupView _testGroup;
        protected BaseIntegrationTestsGroup(DataAccessToken dataAccessToken) : base(dataAccessToken){}

        public override async Task SetUp()
        {
            await base.SetUp();

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{IntegrationTestsGroupController.UrlController}/Create/{"TestGroup"}", null);
            _testGroup = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView)).Data;
        }
    }
}
