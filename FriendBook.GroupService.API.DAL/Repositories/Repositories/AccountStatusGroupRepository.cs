using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class AccountStatusGroupRepository : IAccountStatusGroupRepository
    {
        private GroupAppDBContext _dbContext;

        public AccountStatusGroupRepository(GroupAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountStatusGroup> AddAsync(AccountStatusGroup entity)
        {
            var createdEntity = await _dbContext.AccountsStatusGroups.AddAsync(entity);

            return createdEntity.Entity;
        }

        public bool Delete(AccountStatusGroup entity)
        {
            var deletedEntity = _dbContext.AccountsStatusGroups.Remove(entity);

            return true;
        }

        public IQueryable<AccountStatusGroup> GetAll()
        {
            return _dbContext.AccountsStatusGroups;
        }

        public async Task<bool> SaveAsync()
        {
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<AccountStatusGroup> Update(AccountStatusGroup entity)
        {
            var existingEntity = await _dbContext.AccountsStatusGroups.SingleOrDefaultAsync(x => x.AccountId == entity.AccountId && x.IdGroup == entity.IdGroup);
            
            if (existingEntity != null)
            {
                existingEntity.RoleAccount = entity.RoleAccount;
            }
            return entity;
        }
    }
}
