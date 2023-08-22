using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace FriendBook.GroupService.API.DAL.Repositories.Interfaces
{
    public interface IStageGroupTaskRepository
    {
        public Task<StageGroupTask> AddAsync(StageGroupTask entity);
        public Task<bool> Update(FilterDefinition<StageGroupTask> filter, UpdateDefinition<StageGroupTask> updateDefinition);
        public Task<bool> Delete(Expression<Func<StageGroupTask,bool>> predicate);
        public Task<List<StageGroupTask>> GetAllByIdGroupTask(Guid groupTaskId);
        public IMongoQueryable<StageGroupTask> GetQueryable();
        public Task<StageGroupTask> GetFirstOrDefault(Expression<Func<StageGroupTask, bool>>? expression);
        public IMongoCollection<StageGroupTask> GetCollection();
    }
}
