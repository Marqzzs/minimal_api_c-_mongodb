using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalApiMongo.Domains;
using minimalApiMongo.Services;
using minimalApiMongo.ViewModel;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace minimalApiMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da coleção de ordens
        /// </summary>
        private readonly IMongoCollection<Order> _order;

        /// <summary>
        /// Armazena os dados de acesso da coleção de clientes
        /// </summary>
        private readonly IMongoCollection<Client> _client;

        /// <summary>
        /// Armazena os dados de acesso da coleção de produtos
        /// </summary>
        private readonly IMongoCollection<Product> _product;

        /// <summary>
        /// Construtor que recebe como dependência o objeto da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">Serviço para acessar o MongoDB</param>
        public OrderController(MongoDbService mongoDbService)
        {
            // Obtém a coleção "client"
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");

            // Obtém a coleção "order"
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");

            // Obtém a coleção "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        /// <summary>
        /// Obtém a lista de todas as ordens
        /// </summary>
        /// <returns>Lista de ordens</returns>
        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                //Faz uma filtragem e tras a lista do pedidos
                var orders = await _order.Find(FilterDefinition<Order>.Empty).ToListAsync();
                //retorna um ok e a lista
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Obtém uma ordem específica pelo ID
        /// </summary>
        /// <param name="id">ID da ordem</param>
        /// <returns>Ordem correspondente ao ID</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            //Busca por id um objeto especifico
            var order = await _order.Find(p => p.Id == id).FirstOrDefaultAsync();
            //Faz um ternario, se nao for null retorna o objeto buscado, caso seja null retorna um not found
            return order is not null ? Ok(order) : NotFound();
        }

        /// <summary>
        /// Cria uma nova ordem
        /// </summary>
        /// <param name="newOrder">Dados da nova ordem</param>
        /// <returns>Ordem criada</returns>
        [HttpPost]
        public async Task<ActionResult<Order>> Post(OrderViewModel newOrderViewModel)
        {
            try
            {
                Order order = new Order();
                order.Id = newOrderViewModel.Id;
                order.Data = newOrderViewModel.Data;
                order.Status = newOrderViewModel.Status;
                order.ProductId = newOrderViewModel.ProductId;
                order.ClientId = newOrderViewModel.ClientId;

                var client = await _client.Find(x => x.Id == order.ClientId).FirstOrDefaultAsync();

                if (client == null)
                {
                    return NotFound();
                }

                order.Client = client;

                await _order.InsertOneAsync(order);

                return Ok(order);


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Exclui uma ordem pelo ID
        /// </summary>
        /// <param name="id">ID da ordem</param>
        /// <returns>Resultado da exclusão</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                //Busca um objeto pelo id e deleta o mesmo
                var deleteResult = await _order.DeleteOneAsync(p => p.Id == id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Atualiza uma ordem existente
        /// </summary>
        /// <param name="id">ID da ordem</param>
        /// <param name="order">Dados atualizados da ordem</param>
        /// <returns>Resultado da atualização</returns>
        [HttpPut]
        public async Task<ActionResult> Update(string id, Order order)
        {
            try
            {
                //Faz uma filtragem de um objeto buscado
                var filter = Builders<Order>.Filter.Eq(x => x.Id, order.Id);
                //Faz uma atualizacao do mesmo
                await _order.ReplaceOneAsync(filter, order);
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
