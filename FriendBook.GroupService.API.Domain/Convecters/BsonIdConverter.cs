using MongoDB.Bson;
using System.Text.Json;

namespace FriendBook.GroupService.API.Domain
{
    public class BsonIdConverter : System.Text.Json.Serialization.JsonConverter<ObjectId>
    {
        public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Cannot parse ObjectId");
            }

            string value = reader.GetString() ?? "";
            return new ObjectId(value);
        }

        public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}