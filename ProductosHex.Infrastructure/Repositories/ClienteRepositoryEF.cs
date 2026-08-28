using System.Collections.Generic;
using System.Linq;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;
using ProductosHex.Infrastructure.Data;

namespace ProductosHex.Infrastructure.Repositories
{
    public class ClienteRepositoryEF : IClienteRepository
    {
        public IEnumerable<Cliente> ObtenerTodos()
        {
            using (var db = new ProductosDbContext())
            {
                return db.Clientes.ToList();
            }
        }

        public (List<Cliente> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina)
        {
            using (var db = new ProductosDbContext())
            {
                var query = db.Clientes.AsQueryable();

                int total = query.Count();

                var items = query
                    .OrderBy(c => c.Id)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (items, total);
            }
        }

        public Cliente ObtenerPorId(int id)
        {
            using (var db = new ProductosDbContext())
            {
                return db.Clientes.Find(id);
            }
        }
        public Cliente ObtenerPorDocumento(string numeroDocumento)
        {
            using (var db = new ProductosDbContext())
            {
                return db.Clientes.FirstOrDefault(c => c.NumeroDocumento == numeroDocumento);
            }
        }

        public void Agregar(Cliente cliente)
        {
            using (var db = new ProductosDbContext())
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
            }
        }

        public void Actualizar(Cliente cliente)
        {
            using (var db = new ProductosDbContext())
            {
                var clienteExistente = db.Clientes.Find(cliente.Id);
                if (clienteExistente != null)
                {
                    clienteExistente.NumeroDocumento = cliente.NumeroDocumento;
                    clienteExistente.Nombre = cliente.Nombre;
                    clienteExistente.Telefono = cliente.Telefono;
                    clienteExistente.Email = cliente.Email;
                    db.SaveChanges();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (var db = new ProductosDbContext())
            {
                var cliente = db.Clientes.Find(id);
                if (cliente != null)
                {
                    db.Clientes.Remove(cliente);
                    db.SaveChanges();
                }
            }
        }
    }
}