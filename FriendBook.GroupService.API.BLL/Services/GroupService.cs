using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _commentRepository;

        public GroupService(IGroupRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<BaseResponse<Group>> CreateGroup(Group comment)
        {
            var createdRelationship = await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveAsync();

            return new StandartResponse<Group>()
            {
                Data = createdRelationship,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroup(Guid id)
        {
            var createdRelationship = _commentRepository.Delete(new Group(id));
            await _commentRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = createdRelationship,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public BaseResponse<IQueryable<Group>> GetGroupOData()
        {
            var contents = _commentRepository.GetAsync();
            if (contents.Count() == 0)
            {

                return new StandartResponse<IQueryable<Group>>()
                {
                    Message = "entity not found"
                };
            }

            return new StandartResponse<IQueryable<Group>>()
            {
                Data = contents,
                StatusCode = StatusCode.GroupRead
            };
        }
    }
}