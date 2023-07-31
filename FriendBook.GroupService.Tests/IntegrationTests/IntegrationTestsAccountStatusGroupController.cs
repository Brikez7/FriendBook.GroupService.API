using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsAccountStatusGroupController : BaseIntegrationTestsGroup
    {
        internal const string UrlController = $"{UrlAPI}/AccountStatusGroup";
        public IntegrationTestsAccountStatusGroupController(DataAccessToken dataAccessToken) : base(dataAccessToken){}

        [Test]
        public async Task Create() 
        {
            var newAccountStatusGroupDTO = new AccountStatusGroupDTO(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(newAccountStatusGroupDTO.AccountId).Returns(FabricGrpcResponseHelper.CreateTaskResponseUserExists(true,ServiceCode.UserExists));
            var accountStatusGroupDTOContent = JsonContent.Create(newAccountStatusGroupDTO);

            HttpResponseMessage httpResponseAccountStatusGroupDTO = await _httpClient.PostAsync($"{UrlController}/Create", accountStatusGroupDTOContent);
            var responseAccountStatusGroupDTO = await DeserializeHelper.TryDeserializeStandardResponse<AccountStatusGroupDTO>(httpResponseAccountStatusGroupDTO);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccountStatusGroupDTO.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccountStatusGroupDTO.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupCreated));
                Assert.That(responseAccountStatusGroupDTO?.Data.AccountId, Is.EqualTo(newAccountStatusGroupDTO.AccountId));
                Assert.That(responseAccountStatusGroupDTO?.Data.RoleAccount, Is.EqualTo(RoleAccount.Admin));
            });
        }

        [Test]
        public async Task Delete()
        {
            var newAccountStatusGroupDTO = new AccountStatusGroupDTO(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(newAccountStatusGroupDTO.AccountId).Returns(FabricGrpcResponseHelper.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            var accountStatusGroupDTOContent = JsonContent.Create(newAccountStatusGroupDTO);

            await _httpClient.PostAsync($"{UrlController}/Create", accountStatusGroupDTOContent);

            HttpResponseMessage httpResponseAccountDeleted = await _httpClient.DeleteAsync($"{UrlController}/Delete/{_testGroup.GroupId}?userId={newAccountStatusGroupDTO.AccountId}");
            var responseAccountDeleted = await DeserializeHelper.TryDeserializeStandardResponse<bool>(httpResponseAccountDeleted);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseAccountDeleted.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseAccountDeleted.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupDeleted));
                Assert.That(responseAccountDeleted?.Data, Is.True);
            });
        }

        [Test]
        public async Task Update()
        {
            var newAccountStatusGroupDTO = new AccountStatusGroupDTO(_testGroup.GroupId, Guid.NewGuid(), RoleAccount.Admin);
            _webHost.DecoratorGrpcClient.CheckUserExists(newAccountStatusGroupDTO.AccountId).Returns(FabricGrpcResponseHelper.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            var accountStatusGroupDTOContent = JsonContent.Create(newAccountStatusGroupDTO);

            await _httpClient.PostAsync($"{UrlController}/Create", accountStatusGroupDTOContent);

            newAccountStatusGroupDTO.RoleAccount = RoleAccount.Default;
            var updatedAccountStatusGroupDTOContent = JsonContent.Create(newAccountStatusGroupDTO);
            HttpResponseMessage httpUpdatedAccountStatusGroupDTO = await _httpClient.PutAsync($"{UrlController}/Update", updatedAccountStatusGroupDTOContent);
            var responseUpdatedAccountStatusGroupDTO = await DeserializeHelper.TryDeserializeStandardResponse<AccountStatusGroupDTO>(httpUpdatedAccountStatusGroupDTO);

            Assert.Multiple(() =>
            {
                Assert.That(httpUpdatedAccountStatusGroupDTO.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseUpdatedAccountStatusGroupDTO.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusGroupUpdated));
                Assert.That(responseUpdatedAccountStatusGroupDTO?.Data.AccountId, Is.EqualTo(newAccountStatusGroupDTO.AccountId));
                Assert.That(responseUpdatedAccountStatusGroupDTO?.Data.RoleAccount, Is.EqualTo(RoleAccount.Default));
            });
        }

        [Test]
        public async Task GetProfilesByGroupId()
        {
            var newUser = (Guid.NewGuid(), "NewTestUser");
            _webHost.DecoratorGrpcClient.CheckUserExists(newUser.Item1).Returns(FabricGrpcResponseHelper.CreateTaskResponseUserExists(true, ServiceCode.UserExists));
            var searchedLogin = "";
            _webHost.DecoratorGrpcClient.GetProfiles(searchedLogin, Arg.Any<string>()).Returns(
                FabricGrpcResponseHelper.CreateTaskResponseProfiles(ServiceCode.GrpcProfileReadied, newUser, (_mainUserData.Id, _mainUserData.Login))
            );
            var requestAccountStatusGroupDTO = new AccountStatusGroupDTO(_testGroup.GroupId, newUser.Item1, RoleAccount.Admin);
            var accountStatusGroupDTOContent = JsonContent.Create(requestAccountStatusGroupDTO);

            await _httpClient.PostAsync($"{UrlController}/Create", accountStatusGroupDTOContent);

            HttpResponseMessage httpResponseProfiles = await _httpClient.GetAsync($"{UrlController}/GetProfilesByIdGroup?groupId={_testGroup.GroupId}&login={searchedLogin}");
            var responseProfiles = await DeserializeHelper.TryDeserializeStandardResponse<Profile[]>(httpResponseProfiles);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseProfiles.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(responseProfiles.ServiceCode, Is.EqualTo(ServiceCode.AccountStatusWithGroupMapped));
                Assert.That(responseProfiles?.Data, Has.Length.EqualTo(2));
                Assert.That(responseProfiles?.Data.First(x => x.Id == newUser.Item1.ToString()).Login, Is.EqualTo(newUser.Item2));
                Assert.That(responseProfiles?.Data.First(x => x.Id == _mainUserData.Id.ToString()).Login, Is.EqualTo(_mainUserData.Login));
            });
        }
    }
}
