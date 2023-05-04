using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.DAL.Repositories.Repositories
{
    namespace FriendBook.GroupService.API.DAL.Repositories.Repositories
    {
        public class GroupTaskRepository : IGroupTaskRepository
        {
            private readonly GroupAppDBContext _db;

            public GroupTaskRepository(GroupAppDBContext db)
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
                _db.GroupTasks.Remove(entity);

                return true;
            }
            public IQueryable<GroupTask> Get()
            {
                return _db.GroupTasks.AsQueryable();
            }
            public async Task<bool> SaveAsync()
            {
                await _db.SaveChangesAsync();

                return true;
            }
            public GroupTask Update(GroupTask entity)
            {
                var updatedEntity = _db.GroupTasks.Update(entity);

                return updatedEntity.Entity;
            }
        }
    }
}

