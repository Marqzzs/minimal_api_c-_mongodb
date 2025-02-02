﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace minimalApiMongo.Domains
{
    public class Order
    {
        [BsonId]//Define que esta propiedade e Id do objeto
                //define o nome do campo no MongoDb como _id e o tipo como ObjectId
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("data")]
        public DateTime Data{ get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        // Referência ao cliente que fez o pedido
        [JsonIgnore]
        [BsonElement("clientId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ClientId { get; set; }

        // Lista de produtos associados ao pedido
        [BsonElement("products")]
        public List<Product> Products { get; set; } = new List<Product>();
        [BsonElement("productId")]
        [JsonIgnore]
        public List<string>? ProductId { get; set; }

        public Client Client { get; set; }

    }
}
