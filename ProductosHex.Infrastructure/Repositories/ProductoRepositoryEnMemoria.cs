using System.Collections.Generic;
using System.Linq;
using ProductosHex.Domain.Common; // 👈 OBLIGATORIO
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Infrastructure.Repositories
{
    public class ProductoRepositoryEnMemoria : IProductoRepository
    {
        private static List<Producto> _productos = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Teclado", Precio = 50000, Stock = 10 },
            new Producto { Id = 2, Nombre = "Mouse", Precio = 25000, Stock = 20 }
        };

        // 🔹 DEBE SER PUBLIC Y COINCIDIR CON LA INTERFAZ
        public ResultadoPaginado<Producto> ObtenerPaginado(int pagina, int tamanoPagina)
        {
            var total = _productos.Count;

            var items = _productos
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            return new ResultadoPaginado<Producto>
            {
                Items = items,
                TotalRegistros = total,
                Pagina = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public List<Producto> ObtenerTodos() => _productos;

        public Producto ObtenerPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

        public void Agregar(Producto producto)
        {
            producto.Id = _productos.Any() ? _productos.Max(p => p.Id) + 1 : 1;
            _productos.Add(producto);
        }

        public void Actualizar(Producto producto)
        {
            var existente = ObtenerPorId(producto.Id);
            if (existente != null)
            {
                existente.Nombre = producto.Nombre;
                existente.Precio = producto.Precio;
                existente.Stock = producto.Stock;
            }
        }

        public void Eliminar(int id)
        {
            var existente = ObtenerPorId(id);
            if (existente != null) _productos.Remove(existente);
        }
    }
}