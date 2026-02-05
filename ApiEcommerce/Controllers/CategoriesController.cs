using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(PolicyNames.AllowSpecificOrigin)]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        //[EnableCors(PolicyNames.AllowSpecificOrigin)]
        [AllowAnonymous] // Permite el acceso anónimo a este endpoint
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }

            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")] // Nombre de la ruta
        //[ResponseCache(Duration = 10)] // La respuesta de este endpoint se almacenará en caché durante 10 segundos
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)] // Usando el perfil de caché definido en Program.cs
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // El usuario envió una petición incorrecta
        [ProducesResponseType(StatusCodes.Status404NotFound)] // No se encontró el recurso
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult GetCategory(int id)
        {
            Console.WriteLine($"Categoría con el ID: {id} a las {DateTime.Now}.");
            var category = _categoryRepository.GetCategory(id);
            Console.WriteLine($"Respuesta con el ID: {id}.");
            if (category == null)
            {
                return NotFound($"La categoría con el id {id} no existe.");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Mal formada la solicitud
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // No autenticado
        [ProducesResponseType(StatusCodes.Status201Created)] // Recurso creado
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe.");
                return BadRequest(ModelState);

            }

            var category = _mapper.Map<Category>(createCategoryDto);

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal guardando el registro {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")] // Nombre de la ruta
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Mal formada la solicitud
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // No autenticado
        [ProducesResponseType(StatusCodes.Status404NotFound)] // No se encontró el recurso
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoría con el id {id} no existe.");
            }

            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe.");
                return BadRequest(ModelState);

            }

            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        
        [HttpDelete("{id:int}", Name = "DeleteCategory")] // Nombre de la ruta
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // El usuario no está autorizado para ingresar a este recurso
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Mal formada la solicitud
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // No autenticado
        [ProducesResponseType(StatusCodes.Status404NotFound)] // No se encontró el recurso
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            if(!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoría con el id {id} no existe.");
            }

            var category = _categoryRepository.GetCategory(id);

            if (category == null)
            {
                return NotFound($"La categoría con el id {id} no existe.");
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
