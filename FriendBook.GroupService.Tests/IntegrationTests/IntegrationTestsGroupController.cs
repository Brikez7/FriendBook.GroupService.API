using FriendBook.GroupService.API;
using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.Tests.IntegrationTests.WebAppFactories;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    public class IntegrationTestsGroupController
    {
        private WebHostFactory<Program, GroupAppDBContext> _webHost;
        private HttpClient _httpClient;

        //private DataAccessToken _userData;

        internal const string UrlController = "api/v1/Group";

        public IntegrationTestsGroupController()
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _webHost = new WebHostFactory<Program, GroupAppDBContext>();
            await _webHost.InitializeAsync();

            _httpClient = _webHost.CreateClient();
        }

        [SetUp]
        public async Task TestRegistrationTestAccount()
        {

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
        public async Task TestCreate() 
        {
            Assert.IsTrue(true);
        }
    }
}
