using minimalApiMongo.Domains;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace minimalApiMongo.ViewModel
{
    public class OrderViewModel
    {
        [BsonId]//Define que esta propiedade e Id do objeto
                //define o nome do campo no MongoDb como _id e o tipo como ObjectId
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("data")]
        public DateTime Data { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("clientId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ClientId { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();

        [BsonElement("productId")]

        public List<string>? ProductId { get; set; }
    }
}
