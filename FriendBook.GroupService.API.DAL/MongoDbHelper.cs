using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FriendBook.GroupService.API.DAL
{
    public static class MongoDbHelper
    {
        public static IFindFluent<T,T> Where<T>(this IMongoCollection<T> mongoCollection, Expression<Func<T,bool>> expression) 
        {
            var filter = Builders<T>.Filter.Where(expression);
            return mongoCollection.Find(filter);
        }
        public static IEnumerable<BsonDocument> SelectToDocument<T>(this IFindFluent<T,T> mongoCollection, params Expression<Func<T, object>>[] fieldDefinitions)
        {
            ProjectionDefinition<T>? proj = Builders<T>.Projection.Include(fieldDefinitions[0]);
            foreach (var fieldDefinition in fieldDefinitions[1..])
            {
                proj = proj.Include(fieldDefinition);
            }

            return mongoCollection.Project(proj).ToEnumerable();
        }
    }
}
