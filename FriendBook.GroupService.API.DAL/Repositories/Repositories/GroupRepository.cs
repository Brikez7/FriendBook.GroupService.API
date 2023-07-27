using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly GroupDBContext _db;

        public GroupRepository(GroupDBContext db)
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
            var result = _db.Groups.Remove(entity);

            return result != null;
        }

        public IQueryable<Group> GetAll()
        {
            return _db.Groups.AsQueryable();
        }

        public async Task<int> SaveAsync()
        {
            var countEntriers = await _db.SaveChangesAsync();

            return countEntriers;
        }

        public Group Update(Group entity)
        {
            var existingEntity = _db.Groups.Update(entity);

            return existingEntity.Entity;
        }
    }
}