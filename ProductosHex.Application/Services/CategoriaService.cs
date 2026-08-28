using System.Collections.Generic;
using ProductosHex.Application.DTOs;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repositorio;

        public CategoriaService(ICategoriaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IEnumerable<Categoria> ObtenerTodas()
        {
            return _repositorio.ObtenerTodas();
        }

        public PagedResult<Categoria> ObtenerPaginado(int pagina, int tamanoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1 || tamanoPagina > 100) tamanoPagina = 10;

            // Línea 27: Ya inferirá correctamente los tipos de 'items' y 'total'
            var resultado = _repositorio.ObtenerPaginado(pagina, tamanoPagina);

            return new PagedResult<Categoria>
            {
                Items = resultado.Items,
                TotalRegistros = resultado.Total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public void CrearCategoria(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new System.ArgumentException("El nombre de la categoría es obligatorio");

            _repositorio.Agregar(categoria);
        }

        public void EliminarCategoria(int id)
        {
            _repositorio.Eliminar(id);
        }
    }
}