using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class AccountStatusGroupRepository : IAccountStatusGroupRepository
    {
        private GroupDBContext _dbContext;

        public AccountStatusGroupRepository(GroupDBContext dbContext)
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
            var result = _dbContext.AccountsStatusGroups.Remove(entity);

            return result != null;
        }

        public IQueryable<AccountStatusGroup> GetAll()
        {
            return _dbContext.AccountsStatusGroups;
        }

        public async Task<int> SaveAsync()
        {
            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

        public AccountStatusGroup Update(AccountStatusGroup entity)
        {
            var updatedEntity = _dbContext.AccountsStatusGroups.Update(entity);
            
            return updatedEntity.Entity;
        }
    }
}
