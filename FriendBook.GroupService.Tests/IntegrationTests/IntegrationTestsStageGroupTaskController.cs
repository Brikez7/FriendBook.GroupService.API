using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.DTO.StageGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NodaTime.Extensions;
using System.Net;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsStageGroupTaskController : BaseIntegrationTestsGroupTask
    {
        internal const string UrlController = $"{UrlAPI}/StageGroupTask";
        public IntegrationTestsStageGroupTaskController(AccessToken dataAccessToken) : base(dataAccessToken){}

        [Test]
        public async Task Create() 
        {
            var requestNewStageGroupTask = new RequestNewStageGroupTask(_testGroupTask.Id, "Prepare documents", "Create word page", DateTimeOffset.UtcNow);
            var requestStageGroupTaskNewContent = JsonContentHelper.Create(requestNewStageGroupTask);

            HttpResponseMessage httpResponseNewStageGroupTask = await _httpClient.PostAsync($"{UrlController}/Create/{_testGroup.GroupId}", requestStageGroupTaskNewContent);
            var responseNewStageGroupTask = await DeserializeHelper.TryDeserializeStandardResponse<ResponseStageGroupTaskView>(httpResponseNewStageGroupTask);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseNewStageGroupTask.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseNewStageGroupTask.ServiceCode, Is.EqualTo(ServiceCode.StageGroupTaskCreated));
                Assert.That(responseNewStageGroupTask.Data.Name, Is.EqualTo(requestNewStageGroupTask.Name));
                Assert.That(responseNewStageGroupTask.Data.Text, Is.EqualTo(requestNewStageGroupTask.Text));
                Assert.That(responseNewStageGroupTask.Data.DateCreate, Is.EqualTo(requestNewStageGroupTask.DateCreate.DateTime));
            });
        }

        [Test]
        public async Task Update() 
        {
            var requestNewStageGroupTask = new RequestNewStageGroupTask(_testGroupTask.Id, "Prepare documents", "Create word page", DateTimeOffset.UtcNow);
            var requestStageGroupTaskNewContent = JsonContentHelper.Create(requestNewStageGroupTask);

            HttpResponseMessage httpResponseNewStageGroupTask = await _httpClient.PostAsync($"{UrlController}/Create/{_testGroup.GroupId}", requestStageGroupTaskNewContent);
            var responseNewStageGroupTask = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseStageGroupTaskView>(httpResponseNewStageGroupTask)).Data;

            var requestUpdateStageGroupTask = new UpdateStageGroupTaskDTO(responseNewStageGroupTask.StageId, responseNewStageGroupTask.IdGroupTask, "UpdatedStage", "UpdatedText", DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestUpdateStageGroupTaskContent = JsonContentHelper.Create(requestUpdateStageGroupTask);

            HttpResponseMessage httpResponseUpdatedStageGroupTask = await _httpClient.PutAsync($"{UrlController}/Update/{_testGroup.GroupId}", requestUpdateStageGroupTaskContent);
            var responseUpdatedStageGroupTask = await DeserializeHelper.TryDeserializeStandardResponse<UpdateStageGroupTaskDTO>(httpResponseUpdatedStageGroupTask);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedStageGroupTask.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedStageGroupTask.ServiceCode, Is.EqualTo(ServiceCode.StageGroupTaskUpdated));
                Assert.That(responseUpdatedStageGroupTask.Data.Name, Is.EqualTo(requestUpdateStageGroupTask.Name));
                Assert.That(responseUpdatedStageGroupTask.Data.Text, Is.EqualTo(requestUpdateStageGroupTask.Text));
                Assert.That(responseUpdatedStageGroupTask.Data.DateUpdate, Is.EqualTo(requestUpdateStageGroupTask.DateUpdate));
            });
        }

        [Test]
        public async Task Delete() 
        {
            var requestNewStageGroupTask = new RequestNewStageGroupTask(_testGroupTask.Id, "Prepare documents", "Create word page", DateTimeOffset.UtcNow);
            var requestStageGroupTaskNewContent = JsonContentHelper.Create(requestNewStageGroupTask);

            HttpResponseMessage httpResponseStageGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create/{_testGroup.GroupId}", requestStageGroupTaskNewContent);
            var responseStageGroupTaskView = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseStageGroupTaskView>(httpResponseStageGroupTaskView)).Data;

            HttpResponseMessage httpResponseStageDeleted = await _httpClient.DeleteAsync($"{UrlController}/Delete/{_testGroup.GroupId}?stageGroupTaskId={responseStageGroupTaskView.StageId}");
            var responseStageDeleted = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseStageDeleted);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseStageDeleted.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseStageDeleted.ServiceCode, Is.EqualTo(ServiceCode.StageGroupTaskDeleted));
                Assert.That(responseStageDeleted.Data, Is.EqualTo(true));
            });
        }

        [Test]
        public async Task Get() 
        {
            var requestNewStageGroupTask = new RequestNewStageGroupTask(_testGroupTask.Id, "Prepare documents", "Create word page", DateTimeOffset.UtcNow);
            var requestStageGroupTaskNewContent = JsonContentHelper.Create(requestNewStageGroupTask);

            HttpResponseMessage httpResponseNewStageGroupTask = await _httpClient.PostAsync($"{UrlController}/Create/{_testGroup.GroupId}", requestStageGroupTaskNewContent);
            var responseNewStageGroupTask = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseStageGroupTaskView>(httpResponseNewStageGroupTask)).Data;

            HttpResponseMessage httpResponseStageGroupTaskView = await _httpClient.GetAsync($"{UrlController}/Get/{_testGroup.GroupId}?stageGroupTaskId={responseNewStageGroupTask.StageId}");
            var responseStageGroupTaskView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseStageGroupTaskView>(httpResponseStageGroupTaskView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseStageGroupTaskView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseStageGroupTaskView.ServiceCode, Is.EqualTo(ServiceCode.StageGroupTaskReadied));
                Assert.That(responseStageGroupTaskView.Data.Name, Is.EqualTo(responseNewStageGroupTask.Name));
                Assert.That(responseStageGroupTaskView.Data.Text, Is.EqualTo(responseNewStageGroupTask.Text));
                Assert.That((responseStageGroupTaskView.Data.DateCreate - responseNewStageGroupTask.DateCreate).Ticks, Is.LessThan(20));
                Assert.That(responseStageGroupTaskView.Data.DateUpdate, Is.EqualTo(responseNewStageGroupTask.DateUpdate));
            });
        }
    }
}
