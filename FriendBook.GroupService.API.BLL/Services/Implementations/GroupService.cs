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
                return new StandardResponse<ResponseGroupView>()
                {
                    ServiceCode = ServiceCode.GroupAlreadyExists,
                    Message = "Group with name already exists"
                };
            }

            Group group = new Group(groupName, createrId);
            var createdGroup = await _groupRepository.AddAsync(group);

            var accountStatusGroup = new AccountStatusGroup(createdGroup.CreaterId,(Guid)createdGroup.Id!,RoleAccount.Creater);
            var accountCreaterStatus = await _accountStatusGroupRepository.AddAsync(accountStatusGroup);

            await _groupRepository.SaveAsync();

            return new StandardResponse<ResponseGroupView>()
            {
                Data = new ResponseGroupView(createdGroup),
                ServiceCode = ServiceCode.GroupCreated
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid groupId, Guid createrId)
        {
            var entity = await _groupRepository.GetAll().SingleOrDefaultAsync( x => x.Id == groupId && x.CreaterId == createrId);

            if (entity is null) 
            {
                return new StandardResponse<bool>()
                {
                    Message = "Group not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            var result = _groupRepository.Delete(entity);
            await _groupRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = result,
                ServiceCode = ServiceCode.GroupDeleted
            };
        }

        public BaseResponse<IQueryable<Group>> GetGroupOData()
        {
            var groups = _groupRepository.GetAll();

            return new StandardResponse<IQueryable<Group>>()
            {
                Data = groups,
                ServiceCode = ServiceCode.GroupReadied
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
                return new StandardResponse<ResponseGroupView[]>() { Data = listGroupDTO, ServiceCode = ServiceCode.GroupReadied};
            }
            return new StandardResponse<ResponseGroupView[]>
            {
                Message = "Groups not found",
                ServiceCode = ServiceCode.EntityNotFound
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
                return new StandardResponse<ResponseAccountGroup[]>
                {
                    Message = "Groups not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            return new StandardResponse<ResponseAccountGroup[]>
            {
                Data = accountGroupDTOs,
                ServiceCode = ServiceCode.GroupWithStatusMapped
            };
        }

        public async Task<BaseResponse<ResponseGroupView>> UpdateGroup(RequestUpdateGroup groupDTO, Guid creatorId)
        {
            if (!await _groupRepository.GetAll().AnyAsync(x => x.CreaterId == creatorId && x.Id == groupDTO.GroupId))
                return new StandardResponse<ResponseGroupView> { Message = "Group not found or you not access update group", ServiceCode = ServiceCode.UserNotAccess };

            Group updatedGroup = new(groupDTO.Name,creatorId);
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