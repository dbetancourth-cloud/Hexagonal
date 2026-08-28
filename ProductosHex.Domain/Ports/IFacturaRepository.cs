using System.Collections.Generic;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Domain.Ports
{
    public interface IFacturaRepository
    {
        void Agregar(FacturaEncabezado factura);
        FacturaEncabezado ObtenerPorId(int id);
        (List<FacturaEncabezado> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina);
    }
}