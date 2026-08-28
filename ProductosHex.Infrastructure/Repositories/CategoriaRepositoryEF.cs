using System.Collections.Generic;
using System.Linq;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;
using ProductosHex.Infrastructure.Data;

namespace ProductosHex.Infrastructure.Repositories
{
    public class CategoriaRepositoryEF : ICategoriaRepository
    {
        public IEnumerable<Categoria> ObtenerTodas()
        {
            using (var db = new ProductosDbContext())
            {
                return db.Categorias.ToList();
            }
        }

        public (List<Categoria> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina)
        {
            using (var db = new ProductosDbContext())
            {
                var query = db.Categorias.AsQueryable();

                int total = query.Count();

                var items = query
                    .OrderBy(c => c.Id)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return (items, total);
            }
        }

        // Método agregado para buscar una categoría por su ID
        public Categoria ObtenerPorId(int id)
        {
            using (var db = new ProductosDbContext())
            {
                return db.Categorias.Find(id);
            }
        }

        public void Agregar(Categoria categoria)
        {
            using (var db = new ProductosDbContext())
            {
                db.Categorias.Add(categoria);
                db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            using (var db = new ProductosDbContext())
            {
                var categoria = db.Categorias.Find(id);
                if (categoria != null)
                {
                    db.Categorias.Remove(categoria);
                    db.SaveChanges();
                }
            }
        }
    }
}