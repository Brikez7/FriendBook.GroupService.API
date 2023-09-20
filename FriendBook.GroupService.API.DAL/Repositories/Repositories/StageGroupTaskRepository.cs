using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace FriendBook.GroupService.API.DAL.Repositories
{
    public class StageGroupTaskRepository : IStageGroupTaskRepository
    {
        private readonly IMongoCollection<StageGroupTask> _collection;

        public StageGroupTaskRepository(IMongoCollection<StageGroupTask> collection)
        {
            _collection = collection;
        }

        public async Task<StageGroupTask> AddAsync(StageGroupTask entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }
        public async Task<bool> Update(FilterDefinition<StageGroupTask> filter, UpdateDefinition<StageGroupTask> updateDefinition)
        {
            var result = await _collection.UpdateManyAsync(filter, updateDefinition);
            return result.IsAcknowledged;
        }
        public async Task<bool> Delete(Expression<Func<StageGroupTask, bool>> predicate)
        {
            var result = await _collection.DeleteManyAsync(predicate);
            return result.IsAcknowledged;
        }
        public IMongoQueryable<StageGroupTask> GetQueryable()
        {
            return _collection.AsQueryable();
        }

        public IMongoCollection<StageGroupTask> GetCollection()
        {
            return _collection;
        }
        public async Task<List<StageGroupTask>> GetAllByIdGroupTask(Guid groupTaskId)
        {
            var filter = Builders<StageGroupTask>.Filter.Where(x => x.IdGroupTask == groupTaskId);
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task<StageGroupTask> GetFirstOrDefault(Expression<Func<StageGroupTask, bool>>? expression)
        {
            var filter = Builders<StageGroupTask>.Filter.Where(expression);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }




    }
}
