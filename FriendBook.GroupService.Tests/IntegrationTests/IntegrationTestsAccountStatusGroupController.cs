using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NSubstitute;
using System.Net;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsAccountStatusGroupController : BaseIntegrationTestsGroup
    {
        internal const string UrlController = $"{UrlAPI}/AccountStatusGroup";
        public IntegrationTestsAccountStatusGroupController(AccessToken dataAccessToken) : base(dataAccessToken){}

        [Test]
        public async Task Create() 
        {
            var requestNewAccountStatusGroup = new RequestNewAccountStatusGroup(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(requestNewAccountStatusGroup.AccountId).Returns(FabricGrpcResponse.CreateTaskResponseUserExists(true,ServiceCode.UserExists));
            var requestNewAccountStatusGroupContent = JsonContentHelper.Create(requestNewAccountStatusGroup);

            HttpResponseMessage httpResponseAccountStatusGroupView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewAccountStatusGroupContent);
            var responseAccountStatusGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseAccountStatusGroupView>(httpResponseAccountStatusGroupView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccountStatusGroupView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccountStatusGroupView.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupCreated));
                Assert.That(responseAccountStatusGroupView?.Data.RoleAccount, Is.EqualTo(RoleAccount.Admin));
            });
        }

        [Test]
        public async Task Delete()
        {
            var requestNewAccountStatusGroup = new RequestNewAccountStatusGroup(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(requestNewAccountStatusGroup.AccountId).Returns(FabricGrpcResponse.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            var requestNewAccountStatusGroupContent = JsonContentHelper.Create(requestNewAccountStatusGroup);

            HttpResponseMessage httpResponseAccountStatusGroupView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewAccountStatusGroupContent);
            var responseAccountStatusGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseAccountStatusGroupView>(httpResponseAccountStatusGroupView);

            HttpResponseMessage httpResponseAccountStatusGroupDeleted = await _httpClient.DeleteAsync($"{UrlController}/Delete/{responseAccountStatusGroupView.Data.Id}");
            var responseAccountStatusGroupDeleted = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseAccountStatusGroupDeleted);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccountStatusGroupDeleted.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccountStatusGroupDeleted.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupDeleted));
                Assert.That(responseAccountStatusGroupDeleted?.Data, Is.True);
            });
        }

        [Test]
        public async Task Update()
        {
            var requestNewAccountStatusGroup = new RequestNewAccountStatusGroup(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(requestNewAccountStatusGroup.AccountId).Returns(FabricGrpcResponse.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            var requestNewAccountStatusGroupContent = JsonContentHelper.Create(requestNewAccountStatusGroup);

            HttpResponseMessage httpResponseAccountStatusGroupView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewAccountStatusGroupContent);
            var responseAccountStatusGroupView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseAccountStatusGroupView>(httpResponseAccountStatusGroupView);

            var requestUpdateAccountStatusGroup = new RequestUpdateAccountStatusGroup(responseAccountStatusGroupView.Data.Id, RoleAccount.Default);
            var requestUpdateAccountStatusGroupContent = JsonContentHelper.Create(requestUpdateAccountStatusGroup);

            HttpResponseMessage httpResponseAccountStatusGroupView2 = await _httpClient.PutAsync($"{UrlController}/Update", requestUpdateAccountStatusGroupContent);
            var responseAccountStatusGroupView2 = await DeserializeHelper.TryDeserializeStandardResponse<ResponseAccountStatusGroupView>(httpResponseAccountStatusGroupView2);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccountStatusGroupView2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccountStatusGroupView2.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupUpdated));
                Assert.That(responseAccountStatusGroupView2?.Data.RoleAccount, Is.EqualTo(RoleAccount.Default));
            });
        }

        [Test]
        public async Task GetProfilesByGroupId()
        {
            var mainUserProfile = new Profile() { Id = _mainUserData.Id.ToString(), Login = _mainUserData.Login, FullName = $"FN {_mainUserData.Login}" };
            var idNewTestUser = Guid.NewGuid();
            var newUser = new Profile() { Id = idNewTestUser.ToString(), Login = "NewTestUser", FullName = "FN NewTestUser" };

            var searchedLogin = "";
            _webHost.DecoratorGrpcClient.CheckUserExists(Guid.Parse(newUser.Id)).Returns(FabricGrpcResponse.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            _webHost.DecoratorGrpcClient.GetProfiles(searchedLogin, Arg.Any<string>()).Returns(
                FabricGrpcResponse.CreateTaskResponseProfiles(ServiceCode.GrpcProfileReadied, newUser, mainUserProfile)
            );

            var requestAccountStatusGroupDTO = new RequestNewAccountStatusGroup(_testGroup.GroupId, idNewTestUser, RoleAccount.Admin);
            var accountStatusGroupDTOContent = JsonContentHelper.Create(requestAccountStatusGroupDTO);

            await _httpClient.PostAsync($"{UrlController}/Create", accountStatusGroupDTOContent);

            HttpResponseMessage httpResponseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfilesByIdGroup?groupId={_testGroup.GroupId}&login={searchedLogin}");
            var responseProfiles = await DeserializeHelper.TryDeserializeStandardResponse<Profile[]>(httpResponseProfiles);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseProfiles.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusWithGroupMapped));
                Assert.That(responseProfiles?.Data, Has.Length.EqualTo(2));
                Assert.That(responseProfiles?.Data.First(x => x.Id == newUser.Id).Login, Is.EqualTo(newUser.Login));
                Assert.That(responseProfiles?.Data.First(x => x.Id == _mainUserData.Id.ToString()).Login, Is.EqualTo(_mainUserData.Login));
            });
        }
    }
}
