using FriendBook.GroupService.API;
using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.Settings;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using FriendBook.GroupService.Tests.WebAppFactories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Net.Http.Headers;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    public class IntegrationTestsGroupController1
    {
        private WebHostFactory<Program, GroupDBContext> _webHost;
        private HttpClient _httpClient;
        private DataAccessToken DataAccessToken;
        internal const string UrlController = "api/v1/Group";

        public IntegrationTestsGroupController1(DataAccessToken dataAccessToken)
        {
            DataAccessToken = dataAccessToken;
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _webHost = new WebHostFactory<Program, GroupDBContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }

        [SetUp]
        public void SetUp()
        {
            var jWTSettings = _webHost.Services.GetRequiredService<IOptions<JWTSettings>>().Value;
            var accessToken = TokenHelpers.GenerateAccessToken(DataAccessToken, jWTSettings);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        [TearDown]
        public async Task Clear()
        {
            await _webHost.ClearData();
        }

        [OneTimeTearDown]
        public async Task Dispose()
        {
            await _webHost.DisposeAsync();
        }
        [Test]
        public async Task CreateGroup() 
        {
            string newGroup = "TestGroup";
            TestGrpcClient.MockGrpcService.CheckUserExists(DataAccessToken.Id).Returns(Task.FromResult<BaseResponse<ResponseUserExists>>(new StandartResponse<ResponseUserExists>()
            {
                Data = new ResponseUserExists() { Exists = true },
                StatusCode = ServiceCode.UserExists
            }));

            HttpResponseMessage responseCreatedGroup = await _httpClient.PostAsync($"{UrlController}/Create/{newGroup}", null);
            var responseGroupView = JsonConvert.DeserializeObject<StandartResponse<ResponseGroupView>>(await responseCreatedGroup.Content.ReadAsStringAsync())
                ?? throw new JsonException("Error parsing JSON: responseGroupView");

            Assert.Multiple(() =>
            {
                Assert.That(responseCreatedGroup.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupView.StatusCode, Is.EqualTo(ServiceCode.GroupCreated));
                Assert.IsNotNull(responseGroupView?.Data);
            });
        }
    }
}
