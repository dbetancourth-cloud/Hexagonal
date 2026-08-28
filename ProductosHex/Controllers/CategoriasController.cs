using System.Web.Http;
using ProductosHex.Domain.Entities;
using ProductosHex.Application.Services;

namespace ProductosHex.Controllers
{
    [RoutePrefix("api/categorias")]
    public class CategoriasController : ApiController
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController()
        {
            _categoriaService = ServiceFactory.CrearCategoriaService();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerPaginado([FromUri] int pagina = 1, [FromUri] int tamanoPagina = 10)
        {
            var resultado = _categoriaService.ObtenerPaginado(pagina, tamanoPagina);
            return Ok(resultado);
        }

        [HttpGet]
        [Route("todas")]
        public IHttpActionResult ObtenerTodas()
        {
            var resultado = _categoriaService.ObtenerTodas();
            return Ok(resultado);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear([FromBody] Categoria categoria)
        {
            if (categoria == null)
                return BadRequest("La categoría no puede ser nula.");

            _categoriaService.CrearCategoria(categoria);
            return Ok("Categoría creada con éxito.");
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            _categoriaService.EliminarCategoria(id);
            return Ok("Categoría eliminada con éxito.");
        }
    }
}