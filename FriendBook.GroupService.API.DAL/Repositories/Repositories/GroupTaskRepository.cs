using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class GroupTaskRepository : IGroupTaskRepository
    {
        private readonly GroupDBContext _db;

        public GroupTaskRepository(GroupDBContext db)
        {
            _db = db;
        }
        public async Task<GroupTask> AddAsync(GroupTask entity)
        {
            var createdEntity = await _db.GroupTasks.AddAsync(entity);

            return createdEntity.Entity;
        }

        public bool Delete(GroupTask entity)
        {
            var result = _db.GroupTasks.Remove(entity);

            return result != null;
        }
        public IQueryable<GroupTask> GetAll()
        {
            return _db.GroupTasks.AsQueryable();
        }
        public async Task<int> SaveAsync()
        {
            var result = await _db.SaveChangesAsync();

            return result;
        }
        public GroupTask Update(GroupTask entity)
        {
            var updatedEntity = _db.GroupTasks.Update(entity);

            return updatedEntity.Entity;
        }
    }
}

