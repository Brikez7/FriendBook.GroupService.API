using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.GrpcServices
{
    public interface IGrpcClient
    {
        public Task<BaseResponse<ResponseUserExists>> CheckUserExists(Guid userId);
        public Task<BaseResponse<ResponseUsers>> GetUsers(Guid[] usersId);
        public Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken);
    }
}
