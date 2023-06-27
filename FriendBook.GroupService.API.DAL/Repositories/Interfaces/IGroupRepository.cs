using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        public Task<Group> AddAsync(Group entity);
        public Task<Group?> Update(Group entity);
        public bool Delete(Group entity);
        public IQueryable<Group> GetAll();
        public Task<bool> SaveAsync();
    }
}