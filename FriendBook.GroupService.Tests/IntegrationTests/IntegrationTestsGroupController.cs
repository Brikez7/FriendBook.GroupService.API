using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGroupController : BaseIntegrationTests
    {
        internal const string UrlController = $"{UrlAPI}/Group";
        public IntegrationTestsGroupController(DataAccessToken dataAccessToken) : base(dataAccessToken){}

        [Test]
        public async Task CreateGroup() 
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseGroupView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseGroupView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupView.StatusCode, Is.EqualTo(ServiceCode.GroupCreated));
                Assert.That(responseGroupView?.Data, Is.Not.Null);
            });
        }

        [Test]
        public async Task DeleteGroup()
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseCreatedGroup = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseCreatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseCreatedGroup);

            HttpResponseMessage httpResponseDeletedGroup = await _httpClient.DeleteAsync($"{UrlController}/Delete/{responseCreatedGroup.Data.GroupId}");
            var responseDeletedGroup = await DeserializeHelper.TryDeserializeStandartResponse<bool>(httpResponseDeletedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseDeletedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseDeletedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupDeleted));
                Assert.That(responseDeletedGroup?.Data, Is.True);
            });
        }

        [Test]
        public async Task UpdateGroup()
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseCreatedGroup = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseCreatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseCreatedGroup);

            var newGroupName = $"{responseCreatedGroup.Data.Name}Updated";
            var requestUpdateGroup = new RequestUpdateGroup(responseCreatedGroup.Data.GroupId, newGroupName);
            var requestUpdateGroupContent = JsonContent.Create(requestUpdateGroup);
            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.PutAsync($"{UrlController}/Update", requestUpdateGroupContent);
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseUpdatedGroup);
            
            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupUpdated));
                Assert.That(responseUpdatedGroup?.Data.Name, Is.EqualTo(newGroupName));
            });
        }

        [Test]
        public async Task GetMyGroups()
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseCreatedGroup = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseCreatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseCreatedGroup);

            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroups");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupReadied));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(responseCreatedGroup.Data.GroupId));
            });
        }

        [Test]
        public async Task GetMyGroupsWithMyStatus()
        {
            string newGroup = "TestGroup";

            HttpResponseMessage httpResponseCreatedGroup = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseCreatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseCreatedGroup);

            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroupsWithMyStatus");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseAccountGroup[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupWithStatusMapped));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(responseCreatedGroup.Data.GroupId));
                Assert.That(responseUpdatedGroup?.Data[0].IsAdmin, Is.True);
            });
        }
    }
}
