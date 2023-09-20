using FriendBook.GroupService.API;
using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.Settings;
using FriendBook.GroupService.Tests.TestHelpers;
using FriendBook.GroupService.Tests.WebAppFactories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net.Http.Headers;

namespace FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests
{
    internal abstract class BaseIntegrationTests
    {
        internal const string UrlAPI = "GroupService/v1";

        private protected DataAccessToken _mainUserData;

        private protected WebHostFactory<Program, GroupDBContext> _webHost;
        private protected HttpClient _httpClient;

        public BaseIntegrationTests(DataAccessToken dataAccessToken)
        {
            _mainUserData = dataAccessToken;
        }

        [OneTimeSetUp]
        public virtual async Task Initialization()
        {
            _webHost = new WebHostFactory<Program, GroupDBContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }

        [SetUp]
        public virtual Task SetUp()
        {
            var jWTSettings = _webHost.Services.GetRequiredService<IOptions<JWTSettings>>().Value;
            var accessToken = TokenHelper.GenerateAccessToken(_mainUserData, jWTSettings);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _webHost.DecoratorGrpcClient.CheckUserExists(_mainUserData.Id).Returns(FabricGrpcResponse.CreateTaskResponseUserExists(true,ServiceCode.UserExists));
            return Task.CompletedTask;
        }

        [TearDown]
        public virtual async Task Clear()
        {
            await _webHost.ClearData();
        }

        [OneTimeTearDown]
        public virtual async Task Dispose()
        {
            await _webHost.DisposeAsync();
        }
    }
}
