using System;
using System.Collections.Generic;
using System.Data.Entity; // Importante para el método .Include()
using System.Linq;
using ProductosHex.Domain.Common;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;
using ProductosHex.Infrastructure.Data;

namespace ProductosHex.Infrastructure.Repositories
{
    public class ProductoRepositoryEF : IProductoRepository
    {
        public void Agregar(Producto producto)
        {
            using (var db = new ProductosDbContext())
            {
                db.Productos.Add(producto);
                db.SaveChanges();
            }
        }

        public Producto ObtenerPorId(int id)
        {
            using (var db = new ProductosDbContext())
            {
                // Agregado .Include(p => p.Categoria)
                return db.Productos
                         .Include(p => p.Categoria)
                         .FirstOrDefault(p => p.Id == id);
            }
        }

        public List<Producto> ObtenerTodos()
        {
            using (var db = new ProductosDbContext())
            {
                // Agregado .Include(p => p.Categoria)
                return db.Productos
                         .Include(p => p.Categoria)
                         .ToList();
            }
        }

        public ResultadoPaginado<Producto> ObtenerPaginado(int pagina, int cantidadPorPagina)
        {
            using (var db = new ProductosDbContext())
            {
                var totalElementos = db.Productos.Count();

                // Agregado .Include(p => p.Categoria)
                var elementos = db.Productos
                    .Include(p => p.Categoria)
                    .OrderBy(p => p.Id)
                    .Skip((pagina - 1) * cantidadPorPagina)
                    .Take(cantidadPorPagina)
                    .ToList();

                return new ResultadoPaginado<Producto>
                {
                    Items = elementos,
                    Pagina = pagina,
                    TamanoPagina = cantidadPorPagina,
                    TotalRegistros = totalElementos
                };
            }
        }

        public void Actualizar(Producto producto)
        {
            using (var db = new ProductosDbContext())
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            using (var db = new ProductosDbContext())
            {
                var producto = db.Productos.FirstOrDefault(p => p.Id == id);
                if (producto != null)
                {
                    db.Productos.Remove(producto);
                    db.SaveChanges();
                }
            }
        }
    }
}