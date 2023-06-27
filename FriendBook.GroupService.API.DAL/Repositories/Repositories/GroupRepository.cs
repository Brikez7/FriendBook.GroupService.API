using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.DAL.Repositories
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

        public IQueryable<Group> GetAll()
        {
            return _db.Groups.AsQueryable();
        }

        public async Task<bool> SaveAsync()
        {
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<Group?> Update(Group entity)
        {
            var existingEntity = await _db.Groups.SingleOrDefaultAsync(x => x.Id == entity.Id);

            if (existingEntity != null)
            {
                existingEntity.Name = entity.Name;

                return entity;
            }
            else
            {
                return null;
            }
        }
    }
}