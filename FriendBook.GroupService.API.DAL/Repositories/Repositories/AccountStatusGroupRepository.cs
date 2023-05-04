using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;

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

        public IQueryable<AccountStatusGroup> GetAsync()
        {
            return _dbContext.AccountsStatusGroups;
        }

        public async Task<bool> SaveAsync()
        {
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public AccountStatusGroup Update(AccountStatusGroup entity)
        {
            var updatedEntity = _dbContext.AccountsStatusGroups.Update(entity);

            return updatedEntity.Entity;
        }
    }
}
