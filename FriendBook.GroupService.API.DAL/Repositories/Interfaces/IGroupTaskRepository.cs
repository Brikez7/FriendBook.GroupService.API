using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IGroupTaskRepository
    {
        public Task<GroupTask> AddAsync(GroupTask entity);
        public GroupTask Update(GroupTask entity);
        public bool Delete(GroupTask entity);
        public IQueryable<GroupTask> GetAll();
        public Task<int> SaveAsync();
    }
}