using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
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

        public async Task<BaseResponse<GroupDTO>> CreateGroup(string groupName, Guid createrId)
        {
            Group group = new Group(groupName,createrId);
            if (await _groupRepository.GetAll().AnyAsync(x => x.Name == group.Name)) 
            {
                return new StandartResponse<GroupDTO>()
                {
                    StatusCode = StatusCode.GroupExists,
                    Message = "Group still exists"
                };
            }

            var createdGroup = await _groupRepository.AddAsync(group);

            var accountStatusGroup = new AccountStatusGroup(createdGroup.CreaterId,(Guid)createdGroup.Id,RoleAccount.Creater);
            var accountCreaterStatus = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);

            await _groupRepository.SaveAsync();

            return new StandartResponse<GroupDTO>()
            {
                Data = new GroupDTO(createdGroup),
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid id, Guid userId)
        {
            var entity = await _groupRepository.GetAll().SingleOrDefaultAsync( x => x.Id == id && x.CreaterId == userId);

            if (entity is null) 
            {
                return new StandartResponse<bool>()
                {
                    Message = "Group not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var Result1 = _groupRepository.Delete(entity);
            var Result2 = await _groupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result1 && Result2,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public BaseResponse<IQueryable<Group>> GetGroupOData()
        {
            var groups = _groupRepository.GetAll();

            return new StandartResponse<IQueryable<Group>>()
            {
                Data = groups,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<GroupDTO[]>> GeyGroupsByUserId(Guid userId)
        {
            var listGroupDTO = await _groupRepository.GetAll()
                                                     .Where(x => x.CreaterId == userId)
                                                     .Select(x => new GroupDTO(x))
                                                     .ToArrayAsync();

            if(listGroupDTO.Length == 0) 
            {
                return new StandartResponse<GroupDTO[]>
                {
                    Message = "Grous not founded",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            return new StandartResponse<GroupDTO[]>() { Data = listGroupDTO };
        }

        public async Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId)
        {

            var accountStatusGroup = await _accountStatusGroupRepository.GetAll()
                                                                        .Where(x => x.AccountId == userId)
                                                                        .Include(x => x.Group)
                                                                        .ToListAsync();

            ResponseAccountGroup[] accountGroupDTOs = accountStatusGroup.Select(x => new ResponseAccountGroup(x.Group.Name, x.IdGroup, x.RoleAccount > RoleAccount.Default))
                                                                        .ToArray();

            if (accountGroupDTOs.Length == 0)
            {
                return new StandartResponse<ResponseAccountGroup[]>
                {
                    Message = "No groups where you belong have been found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            return new StandartResponse<ResponseAccountGroup[]>
            {
                Data = accountGroupDTOs
            };
        }

        public async Task<BaseResponse<GroupDTO>> UpdateGroup(GroupDTO group, Guid userId)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.Name == group.Name))
            {
                return new StandartResponse<GroupDTO>()
                {
                    StatusCode = StatusCode.GroupExists,
                    Message = "Group with name already exists"
                };
            }

            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == userId && x.Id == group.GroupId)) 
            {
                return new StandartResponse<GroupDTO>
                {
                    Message = "Group not exists or you not creater",
                    StatusCode = StatusCode.InternalServerError,
                };   
            }

            Group? updatedGroup = new Group(group,userId);
            updatedGroup = await _groupRepository.Update(updatedGroup);

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