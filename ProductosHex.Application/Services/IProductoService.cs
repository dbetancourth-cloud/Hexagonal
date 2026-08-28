using System.Collections.Generic;
using ProductosHex.Domain.Common;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Application.Services
{
    public interface IProductoService
    {
        ResultadoPaginado<Producto> ListarProductosPaginado(int pagina, int tamanoPagina);
        List<Producto> ListarProductos();
        Producto ObtenerProducto(int id);
        void CrearProducto(Producto producto);
        void ActualizarProducto(Producto producto);
        void EliminarProducto(int id);
    }
}