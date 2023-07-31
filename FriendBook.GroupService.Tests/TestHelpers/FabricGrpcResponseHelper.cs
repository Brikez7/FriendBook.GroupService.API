using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal class FabricGrpcResponseHelper
    {
        public static BaseResponse<ResponseProfiles> CreateResponseProfiles(ServiceCode serviceCode,params (Guid, string)[] IdAndLogin) 
        {
            var responseProfiles = new ResponseProfiles();
            responseProfiles.Profiles.AddRange(
                IdAndLogin.Select(x => new Profile() { FullName = "", Id = x.Item1.ToString(), Login = x.Item2 })
            );
            return new StandardResponse<ResponseProfiles>() 
            {
                Data = responseProfiles,
                ServiceCode = serviceCode,
            };
        }
        public static Task<BaseResponse<ResponseProfiles>> CreateTaskResponseProfiles(ServiceCode serviceCode, params (Guid, string)[] IdAndLogin)
        {
            return Task.FromResult(CreateResponseProfiles(serviceCode,IdAndLogin));
        }
        public static BaseResponse<ResponseUserExists> CreateResponseUserExists(bool exists, ServiceCode service)
        {
            return new StandardResponse<ResponseUserExists>()
            {
                Data = new ResponseUserExists() { Exists = exists },
                ServiceCode = service,
            };
        }
        public static Task<BaseResponse<ResponseUserExists>> CreateTaskResponseUserExists(bool exists, ServiceCode serviceCode)
        {
            return Task.FromResult(CreateResponseUserExists(exists,serviceCode));
        }
    }
}
