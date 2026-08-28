using System.Collections.Generic;
using ProductosHex.Domain.Common;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Domain.Ports
{
    public interface IProductoRepository
    {
        ResultadoPaginado<Producto> ObtenerPaginado(int pagina, int tamanoPagina);
        List<Producto> ObtenerTodos();
        Producto ObtenerPorId(int id);
        void Agregar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
    }
}