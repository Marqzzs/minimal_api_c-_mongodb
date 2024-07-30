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
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Order> _order;

        private readonly IMongoCollection<Client> _client;

        private readonly IMongoCollection<Product> _product;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService"></param>
        public OrderController(MongoDbService mongoDbService)
        {
            //obtem a collection "product"
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");

            //obtem a collection "product"
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");

            //obtem a collection "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                var orders = await _order.Find(FilterDefinition<Order>.Empty).ToListAsync();

                return Ok(orders);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            var order = await _order.Find(p => p.Id == id).FirstOrDefaultAsync();

            return order is not null ? Ok(order) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Post(Order newOrder)
        {
            try
            {
                // Verificar se o ClientId fornecido corresponde a um cliente existente
                var client = await _client.Find(c => c.Id == newOrder.ClientId).FirstOrDefaultAsync();
                if (client == null)
                {
                    return BadRequest("ClientId inválido. O cliente não foi encontrado.");
                }

                // Inserir a ordem na coleção de ordens
                await _order.InsertOneAsync(newOrder);

                return Ok(newOrder);
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
                var deleteResult = await _order.DeleteOneAsync(p => p.Id == id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(string id, Order order)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(x => x.Id, order.Id);

                await _order.ReplaceOneAsync(filter, order);

                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


    }
}
