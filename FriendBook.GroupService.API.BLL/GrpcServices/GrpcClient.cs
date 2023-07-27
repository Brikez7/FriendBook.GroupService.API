using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.Settings;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace FriendBook.GroupService.API.BLL.GrpcServices
{
    public class GrpcClient : IGrpcClient
    {
        private readonly GrpcSettings _identityGrpcSettings;

        public GrpcClient(IOptions<GrpcSettings> identityGrpcSettings)
        {
            _identityGrpcSettings = identityGrpcSettings.Value;
        }
        public async Task<BaseResponse<ResponseUserExists>> CheckUserExists(Guid userId)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            ResponseUserExists response;
            using (var channel = GrpcChannel.ForAddress(_identityGrpcSettings.HostGrpcService, new GrpcChannelOptions() { HttpHandler = httpClientHandler }))
            {
                var client = new PublicAccount.PublicAccountClient(channel);
                response = await client.CheckUserExistsAsync(new RequestUserId { AccountId = userId.ToString() });
            }

            if (response.Exists)
            {
                return new StandartResponse<ResponseUserExists> { Data = response, StatusCode = ServiceCode.UserExists };
            }
            return new StandartResponse<ResponseUserExists>()
            {
                Message = "Account not exists or server not connected",
                StatusCode = ServiceCode.UserNotExists,
            };
        }

        public async Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            ResponseProfiles response;
            using (var channel = GrpcChannel.ForAddress(_identityGrpcSettings.HostGrpcService, new GrpcChannelOptions()
            { HttpHandler = httpClientHandler }))
            {
                var requestUserLogin = new RequestUserLogin() { Login = login };

                var headers = new Metadata
                {
                    { "Authorization", accessToken }
                };

                var client = new PublicContact.PublicContactClient(channel);
                response = await client.GetProfilesAsync(requestUserLogin, headers);
            }
            if (response.Profiles is null)
            {
                return new StandartResponse<ResponseProfiles> { Message = "Profiles not found", StatusCode = ServiceCode.EntityNotFound };
            }
            return new StandartResponse<ResponseProfiles> { Data = response, StatusCode = ServiceCode.GrpcProfileReadied };
        }

        public async Task<BaseResponse<ResponseUsers>> GetUsersLoginWithId(Guid[] usersId)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            ResponseUsers response;
            using (var channel = GrpcChannel.ForAddress(_identityGrpcSettings.HostGrpcService, new GrpcChannelOptions() { HttpHandler = httpClientHandler }))
            {
                var requestUsersId = new RequestUsersId() { };
                requestUsersId.UserId.AddRange(usersId.Select(x => x.ToString()));

                var client = new PublicAccount.PublicAccountClient(channel);
                response = await client.GetUsersLoginByIdAsync(requestUsersId);
            }
            if (response.Users is null)
            {
                return new StandartResponse<ResponseUsers> { Message = "Users with id not found", StatusCode = ServiceCode.EntityNotFound };
            }
            return new StandartResponse<ResponseUsers> { Data = response, StatusCode = ServiceCode.GrpcUsersReadied };
        }
    }
}
