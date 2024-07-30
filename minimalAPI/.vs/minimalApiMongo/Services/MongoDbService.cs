using MongoDB.Driver;

namespace minimalApiMongo.Services
{
    public class MongoDbService
    {
        /// <summary>
        /// Armazena a configuracao da aplicacao
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Armazena uma referencia ao MongoDB
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Recebe a confg da aplicacao como parametro
        /// </summary>
        /// <param name="configuration">Object configuration</param>
        public MongoDbService(IConfiguration configuration)
        {
            //Atribui a config recebida em _configuration
            _configuration = configuration;

            //Obtem a string de conexao atraves do _configuration
            var connectionString = _configuration.GetConnectionString("DbConnection");

            //Cria um objeto MongoUrl que recebe como parametro a string de conexao
            var mongoUrl = MongoUrl.Create(connectionString);
            
            //Cria um cliente MongoClient para se connectar ao MongoDb
            var mongoClient = new MongoClient(mongoUrl);

            //Obtem a referencia ao bd com o nome especifico na string
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        /// <summary>
        /// Propiedade para acessar o banco de dados
        /// Retorna a referencia ao bd
        /// </summary>
        public IMongoDatabase GetDatabase => _database;
    }
}
