using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.GrpcServices
{
    public interface IGrpcService
    {
        public Task<BaseResponse<ResponseUserExists>> CheckUserExists(Guid userId);
        public Task<BaseResponse<ResponseUsers>> GetUsersLoginWithId(Guid[] usersId);
        public Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken);
    }
}
