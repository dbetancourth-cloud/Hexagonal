using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductosHex.Application.DTOs;
using ProductosHex.Application.Services;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Tests
{
    [TestClass]
    public class CategoriaServiceTests
    {
        private Mock<ICategoriaRepository> _repositoryMock;
        private CategoriaService _service;

        [TestInitialize]
        public void Setup()
        {
            // Instanciamos el mock del repositorio (puerto de salida)
            _repositoryMock = new Mock<ICategoriaRepository>();

            // Inyectamos el mock en el servicio real de aplicación
            _service = new CategoriaService(_repositoryMock.Object);
        }

        #region Pruebas para ObtenerTodas

        [TestMethod]
        public void ObtenerTodas_DebeRetornarListaDeCategorias()
        {
            // Arrange
            var categoriasEsperadas = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Electrónica" },
                new Categoria { Id = 2, Nombre = "Hogar" }
            };

            _repositoryMock.Setup(r => r.ObtenerTodas())
                           .Returns(categoriasEsperadas);

            // Act
            var resultado = _service.ObtenerTodas();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count());
            _repositoryMock.Verify(r => r.ObtenerTodas(), Times.Once);
        }

        #endregion

        #region Pruebas para ObtenerPaginado

        [TestMethod]
        public void ObtenerPaginado_ConValoresValidos_DebeMapearYPaginarCorrectamente()
        {
            // Arrange
            int pagina = 1;
            int tamanoPagina = 10;
            var itemsFake = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Laptops" }
            };

            _repositoryMock.Setup(r => r.ObtenerPaginado(pagina, tamanoPagina))
                           .Returns((Items: itemsFake, Total: 15));

            // Act
            var resultado = _service.ObtenerPaginado(pagina, tamanoPagina);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Items.Count());
            Assert.AreEqual(15, resultado.TotalRegistros);
            Assert.AreEqual(1, resultado.PaginaActual);
            Assert.AreEqual(10, resultado.TamanoPagina);

            _repositoryMock.Verify(r => r.ObtenerPaginado(1, 10), Times.Once);
        }

        [TestMethod]
        public void ObtenerPaginado_ConPaginaMenorAUno_DebeAjustarPaginaAUno()
        {
            // Arrange
            int paginaInvalida = -5;
            int tamanoPagina = 10;

            _repositoryMock.Setup(r => r.ObtenerPaginado(1, 10))
                           .Returns((Items: new List<Categoria>(), Total: 0));

            // Act
            var resultado = _service.ObtenerPaginado(paginaInvalida, tamanoPagina);

            // Assert
            Assert.AreEqual(1, resultado.PaginaActual);
            _repositoryMock.Verify(r => r.ObtenerPaginado(1, 10), Times.Once);
        }

        [TestMethod]
        public void ObtenerPaginado_ConTamanoPaginaInvalido_DebeAjustarTamanoADiez()
        {
            // Arrange
            int pagina = 1;
            int tamanoInvalido = 500; // Mayor a 100

            _repositoryMock.Setup(r => r.ObtenerPaginado(1, 10))
                           .Returns((Items: new List<Categoria>(), Total: 0));

            // Act
            var resultado = _service.ObtenerPaginado(pagina, tamanoInvalido);

            // Assert
            Assert.AreEqual(10, resultado.TamanoPagina);
            _repositoryMock.Verify(r => r.ObtenerPaginado(1, 10), Times.Once);
        }

        #endregion

        #region Pruebas para CrearCategoria

        [TestMethod]
        public void CrearCategoria_ConNombreValido_DebeLlamarAgregarEnRepositorio()
        {
            // Arrange
            var nuevaCategoria = new Categoria { Nombre = "Herramientas" };

            // Act
            _service.CrearCategoria(nuevaCategoria);

            // Assert
            _repositoryMock.Verify(r => r.Agregar(It.Is<Categoria>(c => c.Nombre == "Herramientas")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearCategoria_ConNombreVacio_DebeLanzarExcepcion()
        {
            // Arrange
            var categoriaInvalida = new Categoria { Nombre = "" };

            // Act
            _service.CrearCategoria(categoriaInvalida);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearCategoria_ConNombreEspaciosEnBlanco_DebeLanzarExcepcion()
        {
            // Arrange
            var categoriaInvalida = new Categoria { Nombre = "   " };

            // Act
            _service.CrearCategoria(categoriaInvalida);
        }

        #endregion

        #region Pruebas para EliminarCategoria

        [TestMethod]
        public void EliminarCategoria_DebeLlamarEliminarEnRepositorio()
        {
            // Arrange
            int idEliminar = 3;

            // Act
            _service.EliminarCategoria(idEliminar);

            // Assert
            _repositoryMock.Verify(r => r.Eliminar(idEliminar), Times.Once);
        }

        #endregion
    }
}