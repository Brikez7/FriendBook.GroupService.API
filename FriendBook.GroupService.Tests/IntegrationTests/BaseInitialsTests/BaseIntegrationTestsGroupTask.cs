using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.Tests.TestHelpers;
using System.Net.Http.Json;

namespace FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests
{
    internal abstract class BaseIntegrationTestsGroupTask : BaseIntegrationTestsGroup
    {
        private protected ResponseGroupTaskView _testGroupTask;
        public BaseIntegrationTestsGroupTask(DataAccessToken dataAccessToken) : base(dataAccessToken){}

        public override async Task SetUp()
        {
            await base.SetUp();

            var requestGroupTaskNew = new RequestGroupTaskNew(_testGroup.GroupId,"TestTask","Description",DateTime.UtcNow.AddDays(10));
            var requestGroupTaskNewContent = JsonContent.Create(requestGroupTaskNew);

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{IntegrationTestsGroupTaskController.UrlController}/Create", requestGroupTaskNewContent);
            _testGroupTask = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseGroupView)).Data;
        }
    }
}
