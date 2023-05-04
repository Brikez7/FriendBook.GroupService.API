using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IGroupTaskRepository
    {
        public Task<GroupTask> AddAsync(GroupTask entity);
        public GroupTask Update(GroupTask entity);
        public bool Delete(GroupTask entity);
        public IQueryable<GroupTask> Get();
        public Task<bool> SaveAsync();
    }
}