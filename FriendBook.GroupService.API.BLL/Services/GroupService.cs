using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository commentRepository)
        {
            _groupRepository = commentRepository;
        }

        public async Task<BaseResponse<Group>> CreateGroup(Group group)
        {
            var createdGroup = await _groupRepository.AddAsync(group);
            await _groupRepository.SaveAsync();

            return new StandartResponse<Group>()
            {
                Data = createdGroup,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid id)
        {
            var Result = _groupRepository.Delete(new Group(id));
            await _groupRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public BaseResponse<IQueryable<Group>> GetGroupOData()
        {
            var groups = _groupRepository.GetAsync();
            if (groups.Count() == 0)
            {
                return new StandartResponse<IQueryable<Group>>()
                {
                    Message = "entity not found"
                };
            }

            return new StandartResponse<IQueryable<Group>>()
            {
                Data = groups,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<Group>> UpdateGroup(Group group)
        {
            var updatedGroup = _groupRepository.Update(group);
            await _groupRepository.SaveAsync();

            return new StandartResponse<Group>()
            {
                Data = updatedGroup,
                StatusCode = StatusCode.GroupUpdate
            };
        }
    }
}