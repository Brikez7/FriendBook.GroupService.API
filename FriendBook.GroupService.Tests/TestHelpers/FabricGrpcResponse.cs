using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal class FabricGrpcResponse
    {
        public static BaseResponse<ResponseProfiles> CreateResponseProfiles(ServiceCode serviceCode, params Profile[] profiles) 
        {
            var responseProfiles = new ResponseProfiles();
            responseProfiles.Profiles.AddRange(profiles);
            return new StandardResponse<ResponseProfiles>() 
            {
                Data = responseProfiles,
                ServiceCode = serviceCode,
            };
        }
        public static Task<BaseResponse<ResponseProfiles>> CreateTaskResponseProfiles(ServiceCode serviceCode, params Profile[] profiles)
        {
            return Task.FromResult(CreateResponseProfiles(serviceCode, profiles));
        }

        public static BaseResponse<ResponseUserExists> CreateResponseUserExists(bool exists, ServiceCode serviceCode)
        {
            return new StandardResponse<ResponseUserExists>()
            {
                Data = new ResponseUserExists() { Exists = exists },
                ServiceCode = serviceCode,
            };
        }
        public static Task<BaseResponse<ResponseUserExists>> CreateTaskResponseUserExists(bool exists, ServiceCode serviceCode)
        {
            return Task.FromResult(CreateResponseUserExists(exists,serviceCode));
        }

        public static BaseResponse<ResponseUsers> CreateResponseUsers(ServiceCode serviceCode, params User[] users)
        {
            var responseUsers = new ResponseUsers();
            responseUsers.Users.AddRange(users);

            return new StandardResponse<ResponseUsers>()
            {
                Data = responseUsers,
                ServiceCode = serviceCode,
            };
        }
        public static Task<BaseResponse<ResponseUsers>> CreateTaskResponseUsers(ServiceCode serviceCode, params User[] users)
        {
            return Task.FromResult(CreateResponseUsers(serviceCode, users));
        }
    }
}
