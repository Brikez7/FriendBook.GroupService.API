using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.DAL.Repositories.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly GroupAppDBContext _db;

        public GroupRepository(GroupAppDBContext db)
        {
            _db = db;
        }

        public async Task<Group> AddAsync(Group entity)
        {
            var createdEntity = await _db.Groups.AddAsync(entity);

            return createdEntity.Entity;
        }

        public bool Delete(Group entity)
        {
            _db.Groups.Remove(entity);

            return true;
        }

        public IQueryable<Group> Get()
        {
            return _db.Groups.AsQueryable();
        }

        public async Task<bool> SaveAsync()
        {
            await _db.SaveChangesAsync();

            return true;
        }

        public Group Update(Group entity)
        {
            var updatedEntity = _db.Groups.Update(entity);

            return updatedEntity.Entity;
        }
    }
}