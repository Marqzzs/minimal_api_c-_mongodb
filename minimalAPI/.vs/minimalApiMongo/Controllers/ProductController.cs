using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalApiMongo.Domains;
using minimalApiMongo.Services;
using MongoDB.Driver;
using System.Text.Json;

namespace minimalApiMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Product> _product;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService"></param>
        public ProductController(MongoDbService mongoDbService)
        {
            //obtem a collection "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                var products = await _product.Find(FilterDefinition<Product>.Empty).ToListAsync();

                return Ok(products);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            var product = await _product.Find(p => p.Id == id).FirstOrDefaultAsync();

            return product is not null ? Ok(product) : NotFound();
        }


        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product newProduct)
        {
            try
            {
                await _product.InsertOneAsync(newProduct);

                return Ok();
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
                var deleteResult = await _product.DeleteOneAsync(p => p.Id == id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(string id, [FromBody] JsonElement updatedFields)
        {
            if (updatedFields.TryGetProperty("price", out var priceElement) && priceElement.TryGetDecimal(out var price))
            {
                var update = Builders<Product>.Update.Set(p => p.Price, price);
                var result = await _product.UpdateOneAsync(p => p.Id == id, update);

                if (result.MatchedCount == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return BadRequest("Invalid product data.");
        }

        [HttpPut]
        public async Task<ActionResult> Update(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);

                await _product.ReplaceOneAsync(filter, product);

                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


    }
}
