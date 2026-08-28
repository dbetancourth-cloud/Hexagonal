using System.Web.Http;
using ProductosHex.Application.DTOs;
using ProductosHex.Application.Services;
using ProductosHex.Domain.Entities;
namespace ProductosHex.Controllers
{
    [RoutePrefix("api/productos")]
    public class ProductosController : ApiController
    {
        private readonly IProductoService _service;

        public ProductosController() : this(ServiceFactory.CrearProductoService())
        {
        }

        public ProductosController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet, Route("")]
        public IHttpActionResult ObtenerTodos(int pagina = 1, int tamanoPagina = 20)
        {
            var resultado = _service.ListarProductosPaginado(pagina, tamanoPagina);
            return Ok(resultado);
        }

        [HttpGet, Route("{id:int}")]
        public IHttpActionResult ObtenerPorId(int id)
        {
            var producto = _service.ObtenerProducto(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost, Route("")]
        public IHttpActionResult Crear([FromBody] CrearProductoRequest request)
        {
            if (request == null)
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Mapeo del DTO a la Entidad de Dominio
            var producto = new Producto
            {
                Nombre = request.Nombre,
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                PorcentajeIva = request.PorcentajeIva
            };

            _service.CrearProducto(producto);
            return Ok(producto);
        }

        [HttpPut, Route("{id:int}")]
        public IHttpActionResult Actualizar(int id, [FromBody] CrearProductoRequest request)
        {
            if (request == null)
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var producto = new Producto
            {
                Id = id,
                Nombre = request.Nombre,
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                PorcentajeIva = request.PorcentajeIva
            };

            _service.ActualizarProducto(producto);
            return Ok(producto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            try
            {
                _service.EliminarProducto(id);
                return Ok("Producto eliminado con éxito.");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return BadRequest("No se puede eliminar el producto porque ya está registrado en una o más facturas.");
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}