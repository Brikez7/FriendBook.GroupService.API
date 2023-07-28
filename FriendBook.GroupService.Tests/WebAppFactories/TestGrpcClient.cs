using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.Domain.Response;
using NSubstitute;

namespace FriendBook.GroupService.Tests.WebAppFactories
{
    public class TestGrpcClient : IGrpcClient
    {
        internal IGrpcClient MockGrpcClient = Substitute.For<IGrpcClient>();

        public TestGrpcClient()
        {
        }

        public Task<BaseResponse<ResponseUserExists>> CheckUserExists(Guid userId)
        {
            return MockGrpcClient.CheckUserExists(userId);
        }

        public Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken)
        {
            return MockGrpcClient.GetProfiles(login, accessToken);
        }

        public Task<BaseResponse<ResponseUsers>> GetUsersLoginWithId(Guid[] usersId)
        {
            return MockGrpcClient.GetUsersLoginWithId(usersId);
        }

        public void Refresh()
        {
            MockGrpcClient = Substitute.For<GrpcClient>();
        }
    }
}
