﻿using FriendBook.GroupService.API;
using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
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

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    internal abstract class BaseIntegrationTests
    {
        private protected WebHostFactory<Program, GroupDBContext> _webHost;
        private protected HttpClient _httpClient;
        private protected DataAccessToken DataAccessToken;
        internal const string UrlAPI = "GroupService/v1";
        public BaseIntegrationTests(DataAccessToken dataAccessToken)
        {
            DataAccessToken = dataAccessToken;
        }

        [OneTimeSetUp]
        public async Task Initialization()
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
            _webHost.DecoratorGrpcClient.CheckUserExists(DataAccessToken.Id).Returns(Task.FromResult<BaseResponse<ResponseUserExists>>(new StandartResponse<ResponseUserExists>()
            {
                Data = new ResponseUserExists() { Exists = true },
                StatusCode = ServiceCode.UserExists
            }));
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
    }
}
