using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        public GroupService(IGroupRepository groupRepository, IAccountStatusGroupRepository accountStatusGroupRepository)
        {
            _groupRepository = groupRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
        }

        public async Task<BaseResponse<ResponseGroupView>> CreateGroup(string groupName, Guid creatorId)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.Name == groupName)) 
                return new StandardResponse<ResponseGroupView> { ServiceCode = ServiceCode.GroupAlreadyExists, Message = "Group with name already exists" };

            var newGroup = new Group(groupName, creatorId);
            var addedGroup = await _groupRepository.AddAsync(newGroup);

            var newAccountStatusGroup = new AccountStatusGroup(addedGroup.CreatorId,(Guid)addedGroup.Id!,RoleAccount.Creator);
            var addedAccountCreatorStatus = await _accountStatusGroupRepository.AddAsync(newAccountStatusGroup);

            await _groupRepository.SaveAsync();

            return new StandardResponse<ResponseGroupView>()
            {
                Data = new ResponseGroupView(addedGroup),
                ServiceCode = ServiceCode.GroupCreated
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid groupId, Guid creatorId)
        {
            var deleteGroup = await _groupRepository.GetAll().SingleOrDefaultAsync( x => x.Id == groupId && x.CreatorId == creatorId);

            if (deleteGroup is null) 
                return new StandardResponse<bool> { Message = "Group not found", ServiceCode = ServiceCode.EntityNotFound };

            var result = _groupRepository.Delete(deleteGroup);
            await _groupRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = result,
                ServiceCode = ServiceCode.GroupDeleted
            };
        }

        public async Task<BaseResponse<ResponseGroupView[]>> GetGroupsByCreatorId(Guid userId)
        {
            var listGroupDTO = await _groupRepository.GetAll()
                                                     .Where(x => x.CreatorId == userId)
                                                     .Select(x => new ResponseGroupView(x))
                                                     .ToArrayAsync();

            if(listGroupDTO is null || listGroupDTO.Length < 1) 
                return new StandardResponse<ResponseGroupView[]>{ Message = "Groups not found", ServiceCode = ServiceCode.EntityNotFound };

            return new StandardResponse<ResponseGroupView[]>() 
            {
                Data = listGroupDTO,
                ServiceCode = ServiceCode.GroupReadied
            };
        }

        public async Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId)
        {

            var accountStatusGroups = _accountStatusGroupRepository.GetAll()
                                                                   .Where(x => x.AccountId == userId)
                                                                   .Include(x => x.Group);

            ResponseAccountGroup[] responseAccountGroups = await accountStatusGroups.Select(x => new ResponseAccountGroup(x.Group!.Name, x.IdGroup, x.RoleAccount > RoleAccount.Default))
                                                                                    .ToArrayAsync();

            if (responseAccountGroups.Length == 0)
                return new StandardResponse<ResponseAccountGroup[]>{ Message = "Groups not found", ServiceCode = ServiceCode.EntityNotFound };

            return new StandardResponse<ResponseAccountGroup[]>
            {
                Data = responseAccountGroups,
                ServiceCode = ServiceCode.GroupWithStatusMapped
            };
        }

        public async Task<BaseResponse<ResponseGroupView>> UpdateGroup(RequestUpdateGroup requestUpdateGroup, Guid creatorId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreatorId == creatorId && x.Id == requestUpdateGroup.GroupId))
                return new StandardResponse<ResponseGroupView> { Message = "Group not found or you not access update group", ServiceCode = ServiceCode.UserNotAccess };

            var updatedGroup = new Group(requestUpdateGroup.Name,creatorId);
            updatedGroup = _groupRepository.Update(updatedGroup);

            await _groupRepository.SaveAsync();

            return new StandardResponse<ResponseGroupView>()
            {
                Data = new ResponseGroupView(updatedGroup),
                ServiceCode = ServiceCode.GroupUpdated
            };
        }
    }
}