using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalApiMongo.Domains;
using minimalApiMongo.Services;
using MongoDB.Driver;

namespace minimalApiMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Client> _client;

        private readonly IMongoCollection<User> _user;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService"></param>
        public ClientController(MongoDbService mongoDbService)
        {
            //obtem a collection "product"
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");

            //obtem a collection "product"
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<ActionResult<List<Client>>> Get()
        {
            try
            {
                var clients = await _client.Find(FilterDefinition<Client>.Empty).ToListAsync();

                return Ok(clients);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(string id)
        {
            var client = await _client.Find(p => p.Id == id).FirstOrDefaultAsync();

            return client is not null ? Ok(client) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Post(Client newClient)
        {
                try
                {
                    // Criar um novo usuário com informações do cliente
                    var newUser = new User
                    {
                        Name = newClient.User!.Name, // Ajuste de acordo com as propriedades disponíveis
                        Email = newClient.User.Email, // Ajuste de acordo com as propriedades disponíveis
                        Password = newClient.User.Password // Ajuste de acordo com as propriedades disponíveis
                    };

                    await _user.InsertOneAsync(newUser);

                    // Definir o UserId do cliente com o Id do usuário recém-criado
                    newClient.UserId = newUser.Id;

                    // Inserir o cliente na coleção de clientes
                    await _client.InsertOneAsync(newClient);

                    return  (newClient);
                }
                catch (Exception e)
                {
                    
                    return BadRequest(e.Message);
                }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var deleteResult = await _client.DeleteOneAsync(p => p.Id == id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(string id, Client client)
        {
            try
            {
                var filter = Builders<Client>.Filter.Eq(x => x.Id, client.Id);

                await _client.ReplaceOneAsync(filter, client);

                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


    }
}
