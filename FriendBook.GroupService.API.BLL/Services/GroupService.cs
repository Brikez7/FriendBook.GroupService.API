using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
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

        public async Task<BaseResponse<GroupDTO>> CreateGroup(Group group)
        {
            var exists = await _groupRepository.Get().AnyAsync(x => x.Name == group.Name);
            if (exists) 
            {
                return new StandartResponse<GroupDTO>()
                {
                    StatusCode = StatusCode.GroupExists,
                    Message = "Group exists"
                };
            }

            var createdGroup = await _groupRepository.AddAsync(group);

            var accountStatusGroup = new AccountStatusGroup(createdGroup.CreaterId,(Guid)createdGroup.Id,RoleAccount.Creater);
            var accountCreaterStatus = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);

            await _groupRepository.SaveAsync();
            await _accountStatusGroupRepository.SaveAsync();

            return new StandartResponse<GroupDTO>()
            {
                Data = new GroupDTO(createdGroup),
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid id)
        {
            var entity = await _groupRepository.Get().SingleOrDefaultAsync( x => x.Id == id);

            if (entity is null) 
            {
                return new StandartResponse<bool>()
                {
                    Message = "Group not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var Result = _groupRepository.Delete(entity);
            await _groupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public BaseResponse<IQueryable<Group>> GetGroupOData()
        {
            var groups = _groupRepository.Get();

            return new StandartResponse<IQueryable<Group>>()
            {
                Data = groups,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<GroupDTO>> UpdateGroup(Group group)
        {
            var exists = await _groupRepository.Get().AnyAsync(x => x.Name == group.Name);
            if (exists)
            {
                return new StandartResponse<GroupDTO>()
                {
                    StatusCode = StatusCode.GroupExists,
                    Message = "Group exists"
                };
            }

            var updatedGroup = await _groupRepository.Update(group);

            if (updatedGroup is null) 
            {
                return new StandartResponse<GroupDTO>()
                {
                    Message = "Group not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            await _groupRepository.SaveAsync();

            return new StandartResponse<GroupDTO>()
            {
                Data = new GroupDTO(updatedGroup),
                StatusCode = StatusCode.GroupUpdate
            };
        }
    }
}