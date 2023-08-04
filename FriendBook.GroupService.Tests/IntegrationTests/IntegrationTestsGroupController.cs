using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
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
            var nameGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{nameGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseGroupView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupView.ServiceCode, Is.EqualTo(ServiceCode.GroupCreated));
                Assert.That(responseGroupView?.Data.Name, Is.EqualTo(nameGroup));
            });
        }

        [Test]
        public async Task DeleteGroup()
        {
            var nameGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{nameGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView);

            HttpResponseMessage httpResponseDeletedGroup = await _httpClient.DeleteAsync($"{UrlController}/Delete/{responseGroupView.Data.GroupId}");
            var responseDeletedGroup = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseDeletedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseDeletedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseDeletedGroup.ServiceCode, Is.EqualTo(ServiceCode.GroupDeleted));
                Assert.That(responseDeletedGroup?.Data, Is.True);
            });
        }

        [Test]
        public async Task UpdateGroup()
        {
            var nameGroup = "TestGroup";
            var updatedNameGroup = $"{nameGroup}Updated";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{nameGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView);

            var requestUpdateGroup = new RequestUpdateGroup(responseGroupView.Data.GroupId, updatedNameGroup);
            var requestUpdateGroupContent = JsonContent.Create(requestUpdateGroup);

            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.PutAsync($"{UrlController}/Update", requestUpdateGroupContent);
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseUpdatedGroup);
            
            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.ServiceCode, Is.EqualTo(ServiceCode.GroupUpdated));
                Assert.That(responseUpdatedGroup?.Data.Name, Is.EqualTo(updatedNameGroup));
            });
        }

        [Test]
        public async Task GetMyGroups()
        {
            var nameGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{nameGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView);

            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroups");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.ServiceCode, Is.EqualTo(ServiceCode.GroupReadied));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(responseGroupView.Data.GroupId));
            });
        }

        [Test]
        public async Task GetMyGroupsWithMyStatus()
        {
            var nameGroup = "TestGroup";

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{nameGroup}", null);
            var responseGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupView>(httpResponseGroupView);

            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroupsWithMyStatus");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandardResponse<ResponseAccountGroup[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.ServiceCode, Is.EqualTo(ServiceCode.GroupWithStatusMapped));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(responseGroupView.Data.GroupId));
                Assert.That(responseUpdatedGroup?.Data[0].IsAdmin, Is.True);
            });
        }
    }
}
