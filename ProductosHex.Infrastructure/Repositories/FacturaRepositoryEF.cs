using global::System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;
using ProductosHex.Infrastructure.Data;

namespace ProductosHex.Infrastructure.Repositories
{
    public class FacturaRepositoryEF : IFacturaRepository
    {
        public void Agregar(FacturaEncabezado factura)
        {
            using (var db = new ProductosDbContext())
            {
                db.Facturas.Add(factura);
                db.SaveChanges();
            }
        }

        public FacturaEncabezado ObtenerPorId(int id)
        {
            using (var db = new ProductosDbContext())
            {
                return db.Facturas
                    .Include(f => f.Cliente)  // <- Carga la información del Cliente
                    .Include(f => f.Detalles)
                    .FirstOrDefault(f => f.Id == id);
            }
        }

        public (List<FacturaEncabezado> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina)
        {
            using (var db = new ProductosDbContext())
            {
                var query = db.Facturas.AsQueryable();

                int total = query.Count();

                var items = query
                    .Include(f => f.Cliente)  // <- Carga la información del Cliente
                    .Include(f => f.Detalles)
                    .OrderByDescending(f => f.Id)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (items, total);
            }
        }
    }
}