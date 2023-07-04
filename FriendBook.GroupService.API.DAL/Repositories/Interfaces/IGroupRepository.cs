using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        public Task<Group> AddAsync(Group entity);
        public Group Update(Group entity);
        public bool Delete(Group entity);
        public IQueryable<Group> GetAll();
        public Task<int> SaveAsync();
    }
}