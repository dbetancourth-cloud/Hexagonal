using ProductosHex.Application.DTOs;

namespace ProductosHex.Application.Services
{
    public interface IFacturaService
    {
        FacturaResponse CrearFactura(CrearFacturaRequest request);
        FacturaResponse ObtenerFactura(int id);
        PagedResult<FacturaResponse> ObtenerPaginado(int pagina, int tamanoPagina);
    }
}