using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class AccountStatusGroupService : IAccountStatusGroupService
    {
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IGroupRepository _groupRepository;
        public AccountStatusGroupService(IAccountStatusGroupRepository accountStatusGroupRepository, IGroupRepository groupRepository)
        {
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<ResponseAccountStatusGroupView>> CreateAccountStatusGroup(Guid creatorId, RequestNewAccountStatusGroup requestNewAccountStatusGroup)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreatorId == creatorId && x.Id == requestNewAccountStatusGroup.GroupId))
                return new StandardResponse<ResponseAccountStatusGroupView> { Message = "you not access for add new account", ServiceCode = ServiceCode.UserNotAccess };

            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestNewAccountStatusGroup.GroupId && x.AccountId == requestNewAccountStatusGroup.AccountId))
                return new StandardResponse<ResponseAccountStatusGroupView>(){ Message = $"New account in group already exists", ServiceCode = ServiceCode.AccountStatusGroupAlreadyExists };

            var newAccountStatusGroup = new AccountStatusGroup(requestNewAccountStatusGroup);

            var addedAccountStatusGroup = await _accountStatusGroupRepository.AddAsync(newAccountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<ResponseAccountStatusGroupView>()
            {
                Data = new ResponseAccountStatusGroupView(addedAccountStatusGroup),
                ServiceCode = ServiceCode.AccountStatusGroupCreated
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid deletedAccountStatusGroupId, Guid creatorId)
        {
            var deletedAccountStatusGroup = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.Id == deletedAccountStatusGroupId);

            if(deletedAccountStatusGroup is null)
                return new StandardResponse<bool> { Message = "Account in group not found", ServiceCode = ServiceCode.EntityNotFound };

            if (deletedAccountStatusGroup is null || !await _groupRepository.GetAll().AnyAsync(x => x.CreatorId == creatorId && x.Id == deletedAccountStatusGroup.IdGroup))
                return new StandardResponse<bool> { Message = "You not access for delete", ServiceCode = ServiceCode.UserNotAccess };

            var Result = _accountStatusGroupRepository.Delete(deletedAccountStatusGroup);
            await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = Result,
                ServiceCode = ServiceCode.AccountStatusGroupDeleted
            };
        }
        public async Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid groupId, ResponseProfiles profileDTOs)
        {
            var usersInSearchedGroupId = await _accountStatusGroupRepository.GetAll()
                                                                            .Where(x => x.IdGroup == groupId)
                                                                            .Select(x => x.AccountId)
                                                                            .ToArrayAsync();

            if (usersInSearchedGroupId is null || usersInSearchedGroupId.Length < 1)
                return new StandardResponse<Profile[]> { Message = "Group not found", ServiceCode = ServiceCode.EntityNotFound };

            var profilesInGroup = profileDTOs.Profiles.AsEnumerable().Join(usersInSearchedGroupId,
                profile => Guid.Parse(profile.Id),
                id => id,
                (profile, id) => profile);

            return new StandardResponse<Profile[]>
            {
                Data = profilesInGroup.ToArray(),
                ServiceCode = ServiceCode.AccountStatusWithGroupMapped,
            };
        }

        public async Task<BaseResponse<ResponseAccountStatusGroupView>> UpdateAccountStatusGroup(RequestUpdateAccountStatusGroup accountStatusGroup, Guid creatorId)
        {
            var updateAccountStatusGroup = await _accountStatusGroupRepository.GetAll().SingleOrDefaultAsync(x => x.Id == accountStatusGroup.Id);

            if(updateAccountStatusGroup is null)
                return new StandardResponse<ResponseAccountStatusGroupView> { Message = "Account not found", ServiceCode = ServiceCode.EntityNotFound };

            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreatorId == creatorId && x.Id == updateAccountStatusGroup.IdGroup))
                return new StandardResponse<ResponseAccountStatusGroupView> { Message = "You not access update account", ServiceCode = ServiceCode.UserNotAccess };

            updateAccountStatusGroup.RoleAccount = accountStatusGroup.RoleAccount;
            var updatedAccountStatusGroup = _accountStatusGroupRepository.Update(updateAccountStatusGroup);
            var result = await _accountStatusGroupRepository.SaveAsync();

            return new StandardResponse<ResponseAccountStatusGroupView>()
            {
                Data = new ResponseAccountStatusGroupView(updatedAccountStatusGroup),
                ServiceCode = ServiceCode.AccountStatusGroupUpdated
            };
        }
    }
}
