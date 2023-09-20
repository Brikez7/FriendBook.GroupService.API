using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.Tests.TestHelpers;
using NodaTime.Extensions;

namespace FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests
{
    internal abstract class BaseIntegrationTestsGroupTask : BaseIntegrationTestsGroup
    {
        private protected ResponseGroupTaskView _testGroupTask;
        public BaseIntegrationTestsGroupTask(AccessToken dataAccessToken) : base(dataAccessToken){}

        public override async Task SetUp()
        {
            await base.SetUp();

            var requestGroupTaskNew = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestGroupTaskNewContent = JsonContentHelper.Create(requestGroupTaskNew);

            HttpResponseMessage httpResponseGroupTaskView = await _httpClient.PostAsync($"{IntegrationTestsGroupTaskController.UrlController}/Create", requestGroupTaskNewContent);
            _testGroupTask = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseGroupTaskView)).Data;
        }
    }
}
