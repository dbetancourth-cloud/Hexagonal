using System.Collections.Generic;
using ProductosHex.Application.DTOs;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Application.Services
{
    public interface IClienteService
    {
        IEnumerable<Cliente> ObtenerTodos();
        PagedResult<Cliente> ObtenerPaginado(int pagina, int tamanoPagina);
        Cliente ObtenerPorId(int id);
        void CrearCliente(Cliente cliente);
        void ActualizarCliente(Cliente cliente);
        void EliminarCliente(int id);
    }
}