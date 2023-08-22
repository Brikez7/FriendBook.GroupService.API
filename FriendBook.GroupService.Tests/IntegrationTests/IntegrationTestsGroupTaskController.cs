using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NodaTime.Extensions;
using NSubstitute;
using System.Net;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGroupTaskController : BaseIntegrationTestsGroup
    {
        internal const string UrlController = $"{UrlAPI}/GroupTask";
        public IntegrationTestsGroupTaskController(DataAccessToken dataAccessToken) : base(dataAccessToken) { }

        [Test]
        public async Task Create()
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseNewGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var responseNewGroupTaskView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseNewGroupTaskView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseNewGroupTaskView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseNewGroupTaskView.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskCreated));
                Assert.That(responseNewGroupTaskView?.Data.Name, Is.EqualTo(requestNewGroupTask.Name));
                Assert.That(responseNewGroupTaskView?.Data.GroupId, Is.EqualTo(requestNewGroupTask.GroupId));
                Assert.That(responseNewGroupTaskView?.Data.DateStartWork.Date, Is.EqualTo(DateTimeOffset.UtcNow.ToOffsetDateTime().Date));
                Assert.That(responseNewGroupTaskView?.Data.DateEndWork, Is.EqualTo(requestNewGroupTask.DateEndWork));
            });
        }

        [Test]
        public async Task Update()
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseNewGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var newGroupTaskView = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseNewGroupTaskView)).Data;

            var requestUpdateGroupTask = new UpdateGroupTaskDTO(newGroupTaskView.Id, $"{requestNewGroupTask.Name}Update", $"{requestNewGroupTask.Description}Update", requestNewGroupTask.DateEndWork.PlusHours(10), StatusTask.Process);
            var requestUpdateGroupTaskContent = JsonContentHelper.Create(requestUpdateGroupTask);

            HttpResponseMessage httpResponseUpdatedGroupTaskView = await _httpClient.PutAsync($"{UrlController}/Update", requestUpdateGroupTaskContent);
            var responseUpdatedGroupTaskView = await DeserializeHelper.TryDeserializeStandardResponse<UpdateGroupTaskDTO>(httpResponseUpdatedGroupTaskView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroupTaskView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroupTaskView.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskUpdated));
                Assert.That(responseUpdatedGroupTaskView?.Data.Name, Is.EqualTo(requestUpdateGroupTask.Name));
                Assert.That(responseUpdatedGroupTaskView?.Data.Description, Is.EqualTo(requestUpdateGroupTask.Description));
                Assert.That(responseUpdatedGroupTaskView?.Data.Status, Is.EqualTo(requestUpdateGroupTask.Status));
                Assert.That(responseUpdatedGroupTaskView?.Data.DateEndWork, Is.EqualTo(requestUpdateGroupTask.DateEndWork));
            });
        }

        [Test]
        public async Task Delete()
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseNewGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var newGroupTaskView = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseNewGroupTaskView)).Data;

            var requestUpdateGroupTask = new UpdateGroupTaskDTO(newGroupTaskView.Id, $"{requestNewGroupTask.Name}Update", $"{requestNewGroupTask.Description}Update", requestNewGroupTask.DateEndWork.PlusHours(10), StatusTask.Denied);
            var requestUpdateGroupTaskContent = JsonContentHelper.Create(requestUpdateGroupTask);
            await _httpClient.PutAsync($"{UrlController}/Update", requestUpdateGroupTaskContent);

            HttpResponseMessage httpResponseGroupTaskDeleted = await _httpClient.DeleteAsync($"{UrlController}/Delete/{newGroupTaskView.Id}");
            var responseGroupTaskDeleted = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseGroupTaskDeleted);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseGroupTaskDeleted.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupTaskDeleted.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskDeleted));
                Assert.That(responseGroupTaskDeleted?.Data, Is.True);
            });
        }

        [Test]
        public async Task Unsubscribe()
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseNewGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var newGroupTaskView = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseNewGroupTaskView)).Data;

            HttpResponseMessage httpResponseUnsubscribed = await _httpClient.PutAsync($"{UrlController}/UnsubscribeTask/{newGroupTaskView.Id}", null);
            var responseUnsubscribed = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseUnsubscribed);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUnsubscribed.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUnsubscribed.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskUpdated));
                Assert.That(responseUnsubscribed?.Data, Is.True);
            });
        }

        [Test]
        public async Task Subscribe()
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseNewGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var newGroupTaskView = (await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseNewGroupTaskView)).Data;
            await _httpClient.PutAsync($"{UrlController}/UnsubscribeTask/{newGroupTaskView.Id}", null);

            HttpResponseMessage httpResponseSubscribed = await _httpClient.PutAsync($"{UrlController}/SubscribeTask/{newGroupTaskView.Id}", null);
            var responseSubscribed = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseSubscribed);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseSubscribed.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseSubscribed.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskUpdated));
                Assert.That(responseSubscribed?.Data, Is.True);
            });
        }

        [Test]
        public async Task GetMyTasksByNameAndGroupId()
        {
            _webHost.DecoratorGrpcClient.GetUsers(Arg.Is<Guid[]>(mas => mas[0] == _mainUserData.Id)).Returns(
               FabricGrpcResponse.CreateTaskResponseUsers(ServiceCode.GrpcUsersReadied, new User { Id = _mainUserData.Id.ToString(), Login = _mainUserData.Login })
            );
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime(), DateTimeOffset.UtcNow.ToOffsetDateTime());
            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);
            await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);

            HttpResponseMessage httpResponseGroupTaskView = await _httpClient.GetAsync($"{UrlController}/GetMyTasksByNameAndGroupId/{requestNewGroupTask.GroupId}");
            var responseGroupTaskView1 = await DeserializeHelper.TryDeserializeStandardResponse<ResponseTasksPage>(httpResponseGroupTaskView);

            var singleTask = responseGroupTaskView1.Data.TasksDTO.Single();
            Assert.Multiple(() =>
            {
                Assert.That(httpResponseGroupTaskView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupTaskView1.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskReadied));
                Assert.That(singleTask.Name, Is.EqualTo(requestNewGroupTask.Name));
                Assert.That(singleTask.Description, Is.EqualTo(requestNewGroupTask.Description));
                Assert.That(singleTask.DateEndWork.ToString(), Is.EqualTo(requestNewGroupTask.DateEndWork.ToString()));
                Assert.That(singleTask.GroupId, Is.EqualTo(requestNewGroupTask.GroupId));
                Assert.That(singleTask.Users?.Single(), Is.EqualTo(_mainUserData.Login));
                Assert.That(singleTask.Status, Is.EqualTo(StatusTask.Process));
                Assert.That(singleTask.StagesGroupTask, Has.Length.EqualTo(1));
            });
        }
    }
}
