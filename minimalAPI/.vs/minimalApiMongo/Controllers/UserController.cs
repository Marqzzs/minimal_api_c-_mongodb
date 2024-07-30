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
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da coleção de usuários
        /// </summary>
        private readonly IMongoCollection<User> _user;

        /// <summary>
        /// Construtor que recebe como dependência o objeto da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">Serviço para acessar o MongoDB</param>
        public UserController(MongoDbService mongoDbService)
        {
            // Obtém a coleção "user"
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        /// <summary>
        /// Obtém a lista de todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            try
            {
                //Faz um find e guarda a lista
                var users = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();
                //Retorna um ok e a lista
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Obtém um usuário específico pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Usuário correspondente ao ID</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            //Faz um find e guarda um user especifico
            var user = await _user.Find(p => p.Id == id).FirstOrDefaultAsync();
            //Faz um ternario para ver se o que foi buscado e not null se sim retorna o resultado
            return user is not null ? Ok(user) : NotFound();
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="newUser">Dados do novo usuário</param>
        /// <returns>Usuário criado</returns>
        [HttpPost]
        public async Task<ActionResult<User>> Post(User newUser)
        {
            try
            {
                //Insere um novo usuario
                await _user.InsertOneAsync(newUser);
                //Retorna um okay
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Exclui um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Resultado da exclusão</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                //Busca um user especifico e guarda em uma variavel e deleta o mesmo
                var deleteResult = await _user.DeleteOneAsync(p => p.Id == id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="user">Dados atualizados do usuário</param>
        /// <returns>Resultado da atualização</returns>
        [HttpPut]
        public async Task<ActionResult> Update(string id, User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
                await _user.ReplaceOneAsync(filter, user);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
