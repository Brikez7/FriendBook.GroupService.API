using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.Domain.Response;
using NSubstitute;

namespace FriendBook.GroupService.Tests.WebAppFactories
{
    public class TestGrpcClient : IGrpcClient
    {
        internal static IGrpcClient MockGrpcService = Substitute.For<IGrpcClient>();

        public TestGrpcClient()
        {
        }

        public Task<BaseResponse<ResponseUserExists>> CheckUserExists(Guid userId)
        {
            return MockGrpcService.CheckUserExists(userId);
        }

        public Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken)
        {
            return MockGrpcService.GetProfiles(login, accessToken);
        }

        public Task<BaseResponse<ResponseUsers>> GetUsersLoginWithId(Guid[] usersId)
        {
            return MockGrpcService.GetUsersLoginWithId(usersId);
        }

        public void Refresh()
        {
            MockGrpcService = Substitute.For<GrpcClient>();
        }
    }
}
