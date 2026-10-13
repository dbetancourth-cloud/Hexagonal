using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductosHex.Application.Services;
using ProductosHex.Domain.Common;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Tests
{
    [TestClass]
    public class ProductoServiceTests
    {
        private Mock<IProductoRepository> _productoRepositoryMock;
        private Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private ProductoService _service;

        [TestInitialize]
        public void Setup()
        {
            _productoRepositoryMock = new Mock<IProductoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();

            _service = new ProductoService(
                _productoRepositoryMock.Object,
                _categoriaRepositoryMock.Object
            );
        }

        #region Pruebas para ObtenerProducto

        [TestMethod]
        public void ObtenerProducto_CuandoExiste_DebeRetornarProducto()
        {
            // Arrange
            int idBusqueda = 1;
            var productoEsperado = new Producto { Id = idBusqueda, Nombre = "Laptop Dell", CategoriaId = 1 };

            _productoRepositoryMock.Setup(r => r.ObtenerPorId(idBusqueda))
                                   .Returns(productoEsperado);

            // Act
            var resultado = _service.ObtenerProducto(idBusqueda); // Usamos ObtenerProducto

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Laptop Dell", resultado.Nombre);
            _productoRepositoryMock.Verify(r => r.ObtenerPorId(idBusqueda), Times.Once);
        }

        #endregion

        #region Pruebas para ListarProductos

        [TestMethod]
        public void ListarProductos_DebeRetornarListaCompleta()
        {
            // Arrange
            var productosFake = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Mouse", CategoriaId = 1 },
                new Producto { Id = 2, Nombre = "Teclado", CategoriaId = 1 }
            };

            _productoRepositoryMock.Setup(r => r.ObtenerTodos())
                                   .Returns(productosFake);

            // Act
            var resultado = _service.ListarProductos();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
            _productoRepositoryMock.Verify(r => r.ObtenerTodos(), Times.Once);
        }

        #endregion

        #region Pruebas para CrearProducto

        [TestMethod]
        public void CrearProducto_ConNombreValidoYCategoriaExistente_DebeAgregar()
        {
            // Arrange
            var nuevoProducto = new Producto { Nombre = "Teclado Mecánico", CategoriaId = 1 };
            var categoriaExistente = new Categoria { Id = 1, Nombre = "Periféricos" };

            _categoriaRepositoryMock.Setup(c => c.ObtenerPorId(nuevoProducto.CategoriaId))
                                    .Returns(categoriaExistente);

            // Act
            _service.CrearProducto(nuevoProducto);

            // Assert
            _productoRepositoryMock.Verify(r => r.Agregar(It.Is<Producto>(p => p.Nombre == "Teclado Mecánico")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearProducto_ConNombreVacio_DebeLanzarExcepcion()
        {
            // Arrange
            var productoInvalido = new Producto { Nombre = "", CategoriaId = 1 };

            // Act
            _service.CrearProducto(productoInvalido);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearProducto_CuandoCategoriaNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var productoSinCategoria = new Producto { Nombre = "Monitor", CategoriaId = 99 };

            _categoriaRepositoryMock.Setup(c => c.ObtenerPorId(99))
                                    .Returns((Categoria)null);

            // Act
            _service.CrearProducto(productoSinCategoria);
        }

        #endregion

        #region Pruebas para ActualizarProducto

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ActualizarProducto_CuandoProductoNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var productoInexistente = new Producto { Id = 50, Nombre = "Impresora", CategoriaId = 1 };

            _productoRepositoryMock.Setup(r => r.ObtenerPorId(50))
                                   .Returns((Producto)null);

            // Act
            _service.ActualizarProducto(productoInexistente);
        }

        #endregion

        #region Pruebas para EliminarProducto

        [TestMethod]
        public void EliminarProducto_DebeLlamarEliminarEnRepositorio()
        {
            // Arrange
            int idEliminar = 10;

            // Act
            _service.EliminarProducto(idEliminar);

            // Assert
            _productoRepositoryMock.Verify(r => r.Eliminar(idEliminar), Times.Once);
        }

        #endregion
    }
}