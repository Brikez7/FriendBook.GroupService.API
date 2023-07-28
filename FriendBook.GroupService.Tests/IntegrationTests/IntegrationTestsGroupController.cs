using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
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

        private ResponseGroupView _testGroupView;

        public override async Task SetUp()
        {
            await base.SetUp();

            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{"TestGroup"}", null);
            _testGroupView = (await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView>(httpResponseGroupView)).Data;
        }

        [Test]
        public async Task CreateGroup() 
        {
            HttpResponseMessage httpResponseGroupView = await _httpClient.PostAsync($"{UrlController}/Create/{$"{_testGroupView.Name}Test"}", null);
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
            HttpResponseMessage httpResponseDeletedGroup = await _httpClient.DeleteAsync($"{UrlController}/Delete/{_testGroupView.GroupId}");
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
            var newGroupName = $"{_testGroupView.Name}Updated";
            var requestUpdateGroup = new RequestUpdateGroup(_testGroupView.GroupId, newGroupName);
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
            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroups");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseGroupView[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupReadied));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(_testGroupView.GroupId));
            });
        }

        [Test]
        public async Task GetMyGroupsWithMyStatus()
        {
            HttpResponseMessage httpResponseUpdatedGroup = await _httpClient.GetAsync($"{UrlController}/GetMyGroupsWithMyStatus");
            var responseUpdatedGroup = await DeserializeHelper.TryDeserializeStandartResponse<ResponseAccountGroup[]>(httpResponseUpdatedGroup);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseUpdatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedGroup.StatusCode, Is.EqualTo(ServiceCode.GroupWithStatusMapped));
                Assert.That(responseUpdatedGroup?.Data, Has.Length.EqualTo(1));
                Assert.That(responseUpdatedGroup?.Data[0].GroupId, Is.EqualTo(_testGroupView.GroupId));
                Assert.That(responseUpdatedGroup?.Data[0].IsAdmin, Is.True);
            });
        }
    }
}
