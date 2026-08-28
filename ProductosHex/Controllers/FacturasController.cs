using System.Web.Http;
using ProductosHex.Application.DTOs;
using ProductosHex.Application.Services;

namespace ProductosHex.Controllers
{
    [RoutePrefix("api/facturas")]
    public class FacturasController : ApiController
    {
        private readonly IFacturaService _facturaService;

        public FacturasController()
        {
            _facturaService = ServiceFactory.CrearFacturaService();
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear([FromBody] CrearFacturaRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _facturaService.CrearFactura(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObtenerPorId(int id)
        {
            var factura = _facturaService.ObtenerFactura(id);
            if (factura == null)
                return NotFound();

            return Ok(factura);
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerTodas([FromUri] int pagina = 1, [FromUri] int tamanoPagina = 10)
        {
            var resultado = _facturaService.ObtenerPaginado(pagina, tamanoPagina);
            return Ok(resultado);
        }
    }
}