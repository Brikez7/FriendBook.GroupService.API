using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class ContactGroupService : IContactGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        public ContactGroupService(IGroupRepository groupRepository, IAccountStatusGroupRepository accountStatusGroupRepository)
        {
            _groupRepository = groupRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
        }

        public async Task<BaseResponse<ResponseGroupView>> CreateGroup(string groupName, Guid createrId)
        {
            if (await _groupRepository.GetAll().AnyAsync(x => x.Name == groupName)) 
            {
                return new StandartResponse<ResponseGroupView>()
                {
                    StatusCode = StatusCode.GroupAlreadyExists,
                    Message = "Group with name already exists"
                };
            }

            Group group = new Group(groupName, createrId);
            var createdGroup = await _groupRepository.AddAsync(group);

            var accountStatusGroup = new AccountStatusGroup(createdGroup.CreaterId,(Guid)createdGroup.Id!,RoleAccount.Creater);
            var accountCreaterStatus = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);

            await _groupRepository.SaveAsync();

            return new StandartResponse<ResponseGroupView>()
            {
                Data = new ResponseGroupView(createdGroup),
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid groupId, Guid createrId)
        {
            var entity = await _groupRepository.GetAll().SingleOrDefaultAsync( x => x.Id == groupId && x.CreaterId == createrId);

            if (entity is null) 
            {
                return new StandartResponse<bool>()
                {
                    Message = "Group not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var result = _groupRepository.Delete(entity);
            await _groupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = result,
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

        public async Task<BaseResponse<ResponseGroupView[]>> GetGroupsByCreaterId(Guid userId)
        {
            var listGroupDTO = await _groupRepository.GetAll()
                                                     .Where(x => x.CreaterId == userId)
                                                     .Select(x => new ResponseGroupView(x))
                                                     .ToArrayAsync();

            if(listGroupDTO?.Length > 0) 
            {
                return new StandartResponse<ResponseGroupView[]>() { Data = listGroupDTO, StatusCode = StatusCode.GroupRead};
            }
            return new StandartResponse<ResponseGroupView[]>
            {
                Message = "Grous not found",
                StatusCode = StatusCode.EntityNotFound
            };
        }

        public async Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId)
        {

            var accountStatusGroup = await _accountStatusGroupRepository.GetAll()
                                                                        .Where(x => x.AccountId == userId)
                                                                        .Include(x => x.Group)
                                                                        .ToListAsync();

            ResponseAccountGroup[] accountGroupDTOs = accountStatusGroup.Select(x => new ResponseAccountGroup(x.Group!.Name, x.IdGroup, x.RoleAccount > RoleAccount.Default))
                                                                        .ToArray();

            if (accountGroupDTOs.Length == 0)
            {
                return new StandartResponse<ResponseAccountGroup[]>
                {
                    Message = "Groups not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            return new StandartResponse<ResponseAccountGroup[]>
            {
                Data = accountGroupDTOs,
                StatusCode = StatusCode.AccountStatusGroupRead
            };
        }

        public async Task<BaseResponse<RequestGroupUpdate>> UpdateGroup(RequestGroupUpdate groupDTO, Guid createrId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == createrId && x.Id == groupDTO.GroupId))
                return new StandartResponse<RequestGroupUpdate> { Message = "Group not found or you not access update group", StatusCode = StatusCode.UserNotAccess };

            Group? updatedGroup = new Group(groupDTO.Name,createrId);
            updatedGroup = _groupRepository.Update(updatedGroup);

            await _groupRepository.SaveAsync();

            return new StandartResponse<RequestGroupUpdate>()
            {
                Data = new RequestGroupUpdate(updatedGroup!),
                StatusCode = StatusCode.GroupUpdate
            };
        }
    }
}