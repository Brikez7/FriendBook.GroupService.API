using FriendBook.CommentServer.API.Domain.Entities;

namespace FriendBook.CommentServer.API.DAL.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        public Task<Group> AddAsync(Group entity);
        public Group Update(Group entity);
        public bool Delete(Group entity);
        public IQueryable<Group> GetAsync();
        public Task<bool> SaveAsync();
    }
}