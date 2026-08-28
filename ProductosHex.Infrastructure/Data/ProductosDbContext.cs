using System.Data.Entity;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Infrastructure.Data
{
    public class ProductosDbContext : DbContext
    {
        public ProductosDbContext() : base("name=ProductosDbContext")
        {
            // DESACTIVA LA VERIFICACIÓN DE MIGRACIONES/METADATOS DE EF
            Database.SetInitializer<ProductosDbContext>(null);
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<FacturaEncabezado> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }
    }
}