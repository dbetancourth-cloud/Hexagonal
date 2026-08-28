using System;
using System.Collections.Generic;
using ProductosHex.Application.DTOs;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repositorio;

        public ClienteService(IClienteRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos();
        }

        public PagedResult<Cliente> ObtenerPaginado(int pagina, int tamanoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1 || tamanoPagina > 100) tamanoPagina = 10;

            var resultado = _repositorio.ObtenerPaginado(pagina, tamanoPagina);

            return new PagedResult<Cliente>
            {
                Items = resultado.Items,
                TotalRegistros = resultado.Total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public Cliente ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public void CrearCliente(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new ArgumentException("El nombre del cliente es obligatorio");

            _repositorio.Agregar(cliente);
        }

        public void ActualizarCliente(Cliente cliente)
        {
            _repositorio.Actualizar(cliente);
        }

        public void EliminarCliente(int id)
        {
            _repositorio.Eliminar(id);
        }
    }
}