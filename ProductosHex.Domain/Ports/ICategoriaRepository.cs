using System.Collections.Generic;
using ProductosHex.Domain.Entities;
namespace ProductosHex.Domain.Ports
{
    public interface ICategoriaRepository
    {
        IEnumerable<Categoria> ObtenerTodas();
        (List<Categoria> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina);
        Categoria ObtenerPorId(int id);
        void Agregar(Categoria categoria);
        void Eliminar(int id);
    }
}