using System;
using System.Collections.Generic;
using ProductosHex.Domain.Common;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repositorio;
        private readonly ICategoriaRepository _categoriaRepo; // <- Se inyecta el repositorio de categorías

        public ProductoService(IProductoRepository repositorio, ICategoriaRepository categoriaRepo)
        {
            _repositorio = repositorio;
            _categoriaRepo = categoriaRepo;
        }

        public ResultadoPaginado<Producto> ListarProductosPaginado(int pagina, int tamanoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1 || tamanoPagina > 100) tamanoPagina = 20;

            return _repositorio.ObtenerPaginado(pagina, tamanoPagina);
        }

        public List<Producto> ListarProductos()
        {
            return _repositorio.ObtenerTodos();
        }

        public Producto ObtenerProducto(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public void CrearProducto(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.");

            // 1. Validar que la categoría exista antes de guardar
            var categoria = _categoriaRepo.ObtenerPorId(producto.CategoriaId);
            if (categoria == null)
            {
                throw new ArgumentException($"La categoría con ID {producto.CategoriaId} no existe.");
            }

            _repositorio.Agregar(producto);
        }

        public void ActualizarProducto(Producto producto)
        {
            var existente = _repositorio.ObtenerPorId(producto.Id);
            if (existente == null)
                throw new Exception("El producto no existe.");

            // 2. Validar categoría al actualizar
            var categoria = _categoriaRepo.ObtenerPorId(producto.CategoriaId);
            if (categoria == null)
            {
                throw new ArgumentException($"La categoría con ID {producto.CategoriaId} no existe.");
            }

            _repositorio.Actualizar(producto);
        }

        public void EliminarProducto(int id)
        {
            _repositorio.Eliminar(id);
        }
    }
}