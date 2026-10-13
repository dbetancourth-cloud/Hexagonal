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
    public class ClienteServiceTests
    {
        private Mock<IClienteRepository> _repositoryMock;
        private ClienteService _service;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IClienteRepository>();
            _service = new ClienteService(_repositoryMock.Object);
        }

        #region Pruebas para ObtenerTodos

        [TestMethod]
        public void ObtenerTodos_DebeRetornarListaDeClientes()
        {
            // Arrange
            var clientesEsperados = new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Carlos Pérez" },
                new Cliente { Id = 2, Nombre = "Ana Gómez" }
            };

            _repositoryMock.Setup(r => r.ObtenerTodos())
                           .Returns(clientesEsperados);

            // Act
            var resultado = _service.ObtenerTodos();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count());
            _repositoryMock.Verify(r => r.ObtenerTodos(), Times.Once);
        }

        #endregion

        #region Pruebas para ObtenerPaginado

        [TestMethod]
        public void ObtenerPaginado_ConValoresValidos_DebeMapearYPaginarCorrectamente()
        {
            // Arrange
            int pagina = 1;
            int tamanoPagina = 10;
            var itemsFake = new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Juan Rodríguez" }
            };

            _repositoryMock.Setup(r => r.ObtenerPaginado(pagina, tamanoPagina))
                           .Returns((Items: itemsFake, Total: 5));

            // Act
            var resultado = _service.ObtenerPaginado(pagina, tamanoPagina);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Items.Count());
            Assert.AreEqual(5, resultado.TotalRegistros);
            Assert.AreEqual(1, resultado.PaginaActual);
            Assert.AreEqual(10, resultado.TamanoPagina);
            _repositoryMock.Verify(r => r.ObtenerPaginado(1, 10), Times.Once);
        }

        [TestMethod]
        public void ObtenerPaginado_ConPaginaMenorAUno_DebeAjustarPaginaAUno()
        {
            // Arrange
            int paginaInvalida = 0;
            int tamanoPagina = 10;

            _repositoryMock.Setup(r => r.ObtenerPaginado(1, 10))
                           .Returns((Items: new List<Cliente>(), Total: 0));

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
            int tamanoInvalido = 150; // Mayor a 100

            _repositoryMock.Setup(r => r.ObtenerPaginado(1, 10))
                           .Returns((Items: new List<Cliente>(), Total: 0));

            // Act
            var resultado = _service.ObtenerPaginado(pagina, tamanoInvalido);

            // Assert
            Assert.AreEqual(10, resultado.TamanoPagina);
            _repositoryMock.Verify(r => r.ObtenerPaginado(1, 10), Times.Once);
        }

        #endregion

        #region Pruebas para ObtenerPorId

        [TestMethod]
        public void ObtenerPorId_CuandoExiste_DebeRetornarCliente()
        {
            // Arrange
            int idBusqueda = 1;
            var clienteEsperado = new Cliente { Id = idBusqueda, Nombre = "María Lopez" };

            _repositoryMock.Setup(r => r.ObtenerPorId(idBusqueda))
                           .Returns(clienteEsperado);

            // Act
            var resultado = _service.ObtenerPorId(idBusqueda);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("María Lopez", resultado.Nombre);
            _repositoryMock.Verify(r => r.ObtenerPorId(idBusqueda), Times.Once);
        }

        #endregion

        #region Pruebas para CrearCliente

        [TestMethod]
        public void CrearCliente_ConNombreValido_DebeLlamarAgregarEnRepositorio()
        {
            // Arrange
            var nuevoCliente = new Cliente { Nombre = "Roberto Martínez" };

            // Act
            _service.CrearCliente(nuevoCliente);

            // Assert
            _repositoryMock.Verify(r => r.Agregar(It.Is<Cliente>(c => c.Nombre == "Roberto Martínez")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearCliente_ConNombreVacio_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteInvalido = new Cliente { Nombre = "" };

            // Act
            _service.CrearCliente(clienteInvalido);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearCliente_ConNombreEspaciosEnBlanco_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteInvalido = new Cliente { Nombre = "   " };

            // Act
            _service.CrearCliente(clienteInvalido);
        }

        #endregion

        #region Pruebas para ActualizarCliente y EliminarCliente

        [TestMethod]
        public void ActualizarCliente_DebeLlamarActualizarEnRepositorio()
        {
            // Arrange
            var cliente = new Cliente { Id = 1, Nombre = "Cliente Actualizado" };

            // Act
            _service.ActualizarCliente(cliente);

            // Assert
            _repositoryMock.Verify(r => r.Actualizar(cliente), Times.Once);
        }

        [TestMethod]
        public void EliminarCliente_DebeLlamarEliminarEnRepositorio()
        {
            // Arrange
            int idEliminar = 5;

            // Act
            _service.EliminarCliente(idEliminar);

            // Assert
            _repositoryMock.Verify(r => r.Eliminar(idEliminar), Times.Once);
        }

        #endregion
    }
}