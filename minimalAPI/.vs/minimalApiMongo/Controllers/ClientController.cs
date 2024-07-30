using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalApiMongo.Domains;
using minimalApiMongo.Services;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace minimalApiMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da coleção de clientes
        /// </summary>
        private readonly IMongoCollection<Client> _client;

        /// <summary>
        /// Armazena os dados de acesso da coleção de usuários
        /// </summary>
        private readonly IMongoCollection<User> _user;

        /// <summary>
        /// Construtor que recebe como dependência o objeto da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">Serviço para acessar o MongoDB</param>
        public ClientController(MongoDbService mongoDbService)
        {
            // Obtém a coleção "client"
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");

            // Obtém a coleção "user"
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        /// <summary>
        /// Obtém a lista de todos os clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        public async Task<ActionResult<List<Client>>> Get()
        {
            try
            {
                //Busca a lista dos clients
                var clients = await _client.Find(FilterDefinition<Client>.Empty).ToListAsync();
                //Retorna um ok e a lista de objetos
                return Ok(clients);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Obtém um cliente específico pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Cliente correspondente ao ID</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(string id)
        {
            //Faz um find e busca um client especifico
            var client = await _client.Find(p => p.Id == id).FirstOrDefaultAsync();
            //Faz a verificacao se e null ou nao e retorna o resultado
            return client is not null ? Ok(client) : NotFound();
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="newClient">Dados do novo cliente</param>
        /// <returns>Cliente criado</returns>
        [HttpPost]
        public async Task<ActionResult<Client>> Post(Client newClient)
        {
            try
            {
                // Criar um novo usuário com informações do cliente
                var newUser = new User
                {
                    Name = newClient.User!.Name,
                    Email = newClient.User.Email,
                    Password = newClient.User.Password
                };

                await _user.InsertOneAsync(newUser);

                // Definir o UserId do cliente com o Id do usuário recém-criado
                newClient.UserId = newUser.Id;

                // Inserir o cliente na coleção de clientes
                await _client.InsertOneAsync(newClient);

                return Ok(newClient);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Exclui um cliente pelo ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Resultado da exclusão</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                //Deleta um client buscado por id
                var deleteResult = await _client.DeleteOneAsync(p => p.Id == id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="client">Dados atualizados do cliente</param>
        /// <returns>Resultado da atualização</returns>
        [HttpPut]
        public async Task<ActionResult> Update(string id, Client client)
        {
            try
            {
                //Filtra um client especifico pelo id
                var filter = Builders<Client>.Filter.Eq(x => x.Id, client.Id);
                //Atualiza o client filtrado
                await _client.ReplaceOneAsync(filter, client);
                //Retorna um ok
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
