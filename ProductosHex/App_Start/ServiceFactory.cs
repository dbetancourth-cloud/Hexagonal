using ProductosHex.Application.Services;
using ProductosHex.Domain.Ports;
using ProductosHex.Infrastructure.Logging;
using ProductosHex.Infrastructure.Repositories;

namespace ProductosHex
{
    public static class ServiceFactory
    {
        public static IFacturaService CrearFacturaService()
        {
            IFacturaRepository facturaRepo = new FacturaRepositoryEF();
            IClienteRepository clienteRepo = new ClienteRepositoryEF();
            IProductoRepository productoRepo = new ProductoRepositoryEF();
            ILoggerService logger = new FileLoggerService();

#pragma warning disable CS0436 // El tipo entra en conflicto con un tipo importado
            return new FacturaService(facturaRepo, clienteRepo, productoRepo, logger);
#pragma warning restore CS0436 // El tipo entra en conflicto con un tipo importado
        }

        public static IProductoService CrearProductoService()
        {
            IProductoRepository productoRepo = new ProductoRepositoryEF();
            ICategoriaRepository categoriaRepo = new CategoriaRepositoryEF(); // Instancia del repositorio

            return new ProductoService(productoRepo, categoriaRepo); // Inyección de ambas dependencias
        }

        public static ICategoriaService CrearCategoriaService()
        {
            ICategoriaRepository categoriaRepo = new CategoriaRepositoryEF();
            return new CategoriaService(categoriaRepo);
        }

        public static IClienteService CrearClienteService()
        {
            IClienteRepository clienteRepo = new ClienteRepositoryEF();
            return new ClienteService(clienteRepo);
        }
    }
}