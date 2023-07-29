using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal class FabricGrpcResponseHelper
    {
        public static ResponseProfiles CreateResponseProfiles(params (Guid, string)[] IdAndLogin) 
        {
            var responseProfiles = new ResponseProfiles();
            responseProfiles.Profiles.AddRange(
                IdAndLogin.Select(x => new Profile() { FullName = "", Id = x.Item1.ToString(), Login = x.Item2 })
            );
            return responseProfiles;
        }
    }
}
