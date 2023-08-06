using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.Tests.IntegrationTests.BaseInitialsTests;
using FriendBook.GroupService.Tests.IntegrationTests.IntegrationTestFixtureSources;
using FriendBook.GroupService.Tests.TestHelpers;
using NodaTime.Extensions;
using System.Net;

namespace FriendBook.GroupService.Tests.IntegrationTests
{
    [TestFixtureSource(typeof(IntegrationTestFixtureSource))]
    internal class IntegrationTestsGroupTaskController : BaseIntegrationTestsGroup
    {
        internal const string UrlController = $"{UrlAPI}/GroupTask";
        public IntegrationTestsGroupTaskController(DataAccessToken dataAccessToken) : base(dataAccessToken){}

        [Test]
        public async Task Create() 
        {
            var requestNewGroupTask = new RequestNewGroupTask(_testGroup.GroupId, "TestTask", "Description", DateTimeOffset.UtcNow.AddDays(10).ToOffsetDateTime());

            var requestNewGroupTaskContent = JsonContentHelper.Create(requestNewGroupTask);

            HttpResponseMessage httpResponseGroupTaskView = await _httpClient.PostAsync($"{UrlController}/Create", requestNewGroupTaskContent);
            var responseGroupTaskView = await DeserializeHelper.TryDeserializeStandardResponse<ResponseGroupTaskView>(httpResponseGroupTaskView);

            Assert.Multiple(() =>
            {
                Assert.That(httpResponseGroupTaskView.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseGroupTaskView.ServiceCode, Is.EqualTo(ServiceCode.GroupTaskCreated));
                Assert.That(responseGroupTaskView?.Data.Name, Is.EqualTo(requestNewGroupTask.Name));
                Assert.That(responseGroupTaskView?.Data.GroupId, Is.EqualTo(requestNewGroupTask.GroupId));
                Assert.That(responseGroupTaskView?.Data.DateStartWork.Date, Is.EqualTo(DateTimeOffset.UtcNow.ToOffsetDateTime().Date));
                Assert.That(responseGroupTaskView?.Data.DateEndWork, Is.EqualTo(requestNewGroupTask.DateEndWork));
            });
        }
    }
}
