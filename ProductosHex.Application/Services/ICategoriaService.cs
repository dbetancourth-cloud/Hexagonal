using System.Collections.Generic;
using ProductosHex.Application.DTOs;
using ProductosHex.Domain.Entities;

namespace ProductosHex.Application.Services
{
    public interface ICategoriaService
    {
        IEnumerable<Categoria> ObtenerTodas();
        PagedResult<Categoria> ObtenerPaginado(int pagina, int tamanoPagina);
        void CrearCategoria(Categoria categoria);
        void EliminarCategoria(int id);
    }
}