using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{productId:int}", Name = "GetProduct")] // Nombre de la ruta
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // El usuario envió una petición incorrecta
        [ProducesResponseType(StatusCodes.Status404NotFound)] // No se encontró el recurso
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);

            if (product == null)
            {
                return NotFound($"El producto con el id {productId} no existe.");
            }

            var productDto = _mapper.Map<ProductDto>(product);

            return Ok(productDto);
        }
    }
}
