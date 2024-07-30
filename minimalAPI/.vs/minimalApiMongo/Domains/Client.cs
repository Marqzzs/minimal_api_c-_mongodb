using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace minimalApiMongo.Domains
{
    public class Client
    {
        [BsonId]//Define que esta propiedade e Id do objeto
                //define o nome do campo no MongoDb como _id e o tipo como ObjectId
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("cpf")]
        public string? CPF { get; set; }

        [BsonElement("phone")]
        public string? Phone{ get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }

        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonIgnore]
        public User? User { get; set; }

        //adiciona um dicionario para atributos adicionas
        public Dictionary<string, string> AddtionalAttributes { get; set; }

        /// <summary>
        /// Ao ser instanciado um obj da classe Product, o atributo AddtionalAttributes ja vira com um novo dicionario e portanto habilitado para adicionar + atributos
        /// </summary>
        public Client()
        {
            AddtionalAttributes = new Dictionary<string, string>();
        }
    }
}
