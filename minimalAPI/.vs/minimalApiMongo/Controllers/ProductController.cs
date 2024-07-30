using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalApiMongo.Domains;
using minimalApiMongo.Services;
using MongoDB.Driver;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace minimalApiMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da coleção de produtos
        /// </summary>
        private readonly IMongoCollection<Product> _product;

        /// <summary>
        /// Construtor que recebe como dependência o objeto da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">Serviço para acessar o MongoDB</param>
        public ProductController(MongoDbService mongoDbService)
        {
            // Obtém a coleção "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        /// <summary>
        /// Obtém a lista de todos os produtos
        /// </summary>
        /// <returns>Lista de produtos</returns>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                // Obtém todos os produtos da coleção
                var products = await _product.Find(FilterDefinition<Product>.Empty).ToListAsync();
                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Obtém um produto específico pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Produto correspondente ao ID</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            // Busca o produto na coleção pelo ID
            var product = await _product.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product is not null ? Ok(product) : NotFound();
        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <param name="newProduct">Dados do novo produto</param>
        /// <returns>Produto criado</returns>
        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product newProduct)
        {
            try
            {
                // Insere o novo produto na coleção
                await _product.InsertOneAsync(newProduct);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Exclui um produto pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Resultado da exclusão</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                // Exclui o produto da coleção pelo ID
                var deleteResult = await _product.DeleteOneAsync(p => p.Id == id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Atualiza parcialmente um produto
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <param name="updatedFields">Campos atualizados do produto</param>
        /// <returns>Resultado da atualização parcial</returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(string id, [FromBody] JsonElement updatedFields)
        {
            // Verifica se o campo "price" está presente nos campos atualizados
            if (updatedFields.TryGetProperty("price", out var priceElement) && priceElement.TryGetDecimal(out var price))
            {
                // Cria uma atualização para o campo "price"
                var update = Builders<Product>.Update.Set(p => p.Price, price);
                // Aplica a atualização no produto correspondente ao ID
                var result = await _product.UpdateOneAsync(p => p.Id == id, update);

                // Verifica se algum produto foi atualizado
                if (result.MatchedCount == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return BadRequest("Invalid product data.");
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        /// <param name="product">Dados atualizados do produto</param>
        /// <returns>Resultado da atualização</returns>
        [HttpPut]
        public async Task<ActionResult> Update(Product product)
        {
            try
            {
                // Cria um filtro para encontrar o produto pelo ID
                var filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
                // Substitui o produto existente pelo novo produto
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
