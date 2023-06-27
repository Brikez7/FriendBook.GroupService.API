using FriendBook.GroupService.API.BLL.gRPCClients;
using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Settings;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using StatusCode = FriendBook.GroupService.API.Domain.InnerResponse.StatusCode;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GrpcService : IGrpcService
    {
        private readonly GrpcSettings _identityGrpcSettings;

        public GrpcService(IOptions<GrpcSettings> identityGrpcSettings)
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

            if (!response.Exists)
            {
                return new StandartResponse<ResponseUserExists>()
                {
                    Message = "Account not exists or server not connected",
                    StatusCode = StatusCode.InternalServerError,
                };
            }

            return new StandartResponse<ResponseUserExists> { Data = response};
        }

        public async Task<BaseResponse<ResponseProfiles>> GetProfiles(string login, string accessToken)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            ResponseProfiles response;
            using (var channel = GrpcChannel.ForAddress("http://localhost:5001", new GrpcChannelOptions()
            { HttpHandler = httpClientHandler }))
            {
                var requestUserLogin = new RequestUserLogin() {Login = login };

                var headers = new Metadata();
                headers.Add("Authorization",  accessToken);

                var client = new PublicContact.PublicContactClient(channel);
                response = await client.GetProfilesAsync(requestUserLogin, headers);
            }
            if (response.Profiles is null) 
            {
                return new StandartResponse<ResponseProfiles> { Message = "Profiles not found", StatusCode = StatusCode.InternalServerError };
            }
            return new StandartResponse<ResponseProfiles> { Data = response };
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
                response = await client.GetUsersLoginWithIdAsync(requestUsersId);
            }
            if (response.Users is null) 
            {
                return new StandartResponse<ResponseUsers> { Message = "Users with id not found", StatusCode = StatusCode.InternalServerError };
            }
            return new StandartResponse<ResponseUsers> { Data = response };
        }
    }
}
