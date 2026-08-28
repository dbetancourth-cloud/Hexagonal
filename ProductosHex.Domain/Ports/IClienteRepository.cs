using System.Collections.Generic;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Domain.Ports
{
    public interface IClienteRepository
    {
        IEnumerable<Cliente> ObtenerTodos();
        (List<Cliente> Items, int Total) ObtenerPaginado(int pagina, int tamanoPagina);
        Cliente ObtenerPorId(int id);
        Cliente ObtenerPorDocumento(string numeroDocumento);
        void Agregar(Cliente cliente);
        void Actualizar(Cliente cliente);
        void Eliminar(int id);
    }
}