using System.Web.Http;
using ProductosHex.Domain.Entities;
using ProductosHex.Application.Services;

namespace ProductosHex.Controllers
{
    [RoutePrefix("api/clientes")]
    public class ClientesController : ApiController
    {
        private readonly IClienteService _clienteService;

        public ClientesController()
        {
            _clienteService = ServiceFactory.CrearClienteService();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerPaginado([FromUri] int pagina = 1, [FromUri] int tamanoPagina = 10)
        {
            var resultado = _clienteService.ObtenerPaginado(pagina, tamanoPagina);
            return Ok(resultado);
        }

        [HttpGet]
        [Route("todos")]
        public IHttpActionResult ObtenerTodos()
        {
            var resultado = _clienteService.ObtenerTodos();
            return Ok(resultado);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObtenerPorId(int id)
        {
            var cliente = _clienteService.ObtenerPorId(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear([FromBody] Cliente cliente)
        {
            if (cliente == null)
                return BadRequest("El cliente no puede ser nulo.");

            _clienteService.CrearCliente(cliente);
            return Ok("Cliente creado con éxito.");
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Actualizar(int id, [FromBody] Cliente cliente)
        {
            if (cliente == null)
                return BadRequest("El cliente no puede ser nulo.");

            cliente.Id = id;
            _clienteService.ActualizarCliente(cliente);
            return Ok("Cliente actualizado con éxito.");
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            _clienteService.EliminarCliente(id);
            return Ok("Cliente eliminado con éxito.");
        }
    }
}