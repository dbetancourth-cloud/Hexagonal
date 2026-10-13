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
    public class FacturaServiceTests
    {
        private Mock<IFacturaRepository> _facturaRepoMock;
        private Mock<IClienteRepository> _clienteRepoMock;
        private Mock<IProductoRepository> _productoRepoMock;
        private Mock<ILoggerService> _loggerMock;
        private FacturaService _service;

        [TestInitialize]
        public void Setup()
        {
            _facturaRepoMock = new Mock<IFacturaRepository>();
            _clienteRepoMock = new Mock<IClienteRepository>();
            _productoRepoMock = new Mock<IProductoRepository>();
            _loggerMock = new Mock<ILoggerService>();

            _service = new FacturaService(
                _facturaRepoMock.Object,
                _clienteRepoMock.Object,
                _productoRepoMock.Object,
                _loggerMock.Object
            );
        }

        #region Pruebas para CrearFactura (Flujo Exitoso y Cálculos)

        [TestMethod]
        public void CrearFactura_ConDatosValidos_DebeCrearFacturaYActualizarStock()
        {
            // Arrange
            var request = new CrearFacturaRequest
            {
                NumeroDocumento = "123456789",
                Descuento = 0,
                Detalles = new List<CrearFacturaDetalleRequest>
                {
                    new CrearFacturaDetalleRequest { ProductoId = 1, Cantidad = 2, PorcentajeDescuento = 0 }
                }
            };

            var clienteFake = new Cliente { Id = 10, Nombre = "Juan Perez", Email = "juan@test.com", NumeroDocumento = "123456789" };
            var productoFake = new Producto { Id = 1, Nombre = "Laptop", Precio = 1000m, Stock = 5, PorcentajeIva = 19m };

            _clienteRepoMock.Setup(c => c.ObtenerPorDocumento("123456789")).Returns(clienteFake);
            _productoRepoMock.Setup(p => p.ObtenerPorId(1)).Returns(productoFake);

            // Act
            var respuesta = _service.CrearFactura(request);

            // Assert
            Assert.IsNotNull(respuesta);
            Assert.AreEqual("Juan Perez", respuesta.Cliente.Nombre);
            Assert.AreEqual(1, respuesta.Detalles.Count);

            // Verificamos que se actualizó el stock del producto (5 - 2 = 3)
            _productoRepoMock.Verify(p => p.Actualizar(It.Is<Producto>(prod => prod.Stock == 3)), Times.Once);

            // Verificamos que se guardó la factura en el repositorio
            _facturaRepoMock.Verify(f => f.Agregar(It.IsAny<FacturaEncabezado>()), Times.Once);
        }

        #endregion

        #region Pruebas para Validaciones de Excepciones en CrearFactura

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearFactura_ConRequestNulo_DebeLanzarExcepcion()
        {
            // Act
            _service.CrearFactura(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearFactura_CuandoClienteNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var request = new CrearFacturaRequest { NumeroDocumento = "000000" };
            _clienteRepoMock.Setup(c => c.ObtenerPorDocumento("000000")).Returns((Cliente)null);

            // Act
            _service.CrearFactura(request);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearFactura_SinDetalles_DebeLanzarExcepcion()
        {
            // Arrange
            var request = new CrearFacturaRequest
            {
                NumeroDocumento = "12345",
                Detalles = new List<CrearFacturaDetalleRequest>()
            };
            var clienteFake = new Cliente { Id = 1, NumeroDocumento = "12345" };

            _clienteRepoMock.Setup(c => c.ObtenerPorDocumento("12345")).Returns(clienteFake);

            // Act
            _service.CrearFactura(request);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CrearFactura_ConStockInsuficiente_DebeLanzarExcepcion()
        {
            // Arrange
            var request = new CrearFacturaRequest
            {
                NumeroDocumento = "12345",
                Detalles = new List<CrearFacturaDetalleRequest>
                {
                    new CrearFacturaDetalleRequest { ProductoId = 1, Cantidad = 10 }
                }
            };

            var clienteFake = new Cliente { Id = 1, NumeroDocumento = "12345" };
            var productoSinStock = new Producto { Id = 1, Nombre = "Teclado", Stock = 2, Precio = 50 };

            _clienteRepoMock.Setup(c => c.ObtenerPorDocumento("12345")).Returns(clienteFake);
            _productoRepoMock.Setup(p => p.ObtenerPorId(1)).Returns(productoSinStock);

            // Act
            _service.CrearFactura(request);
        }

        #endregion

        #region Pruebas para ObtenerFactura y Paginado

        [TestMethod]
        public void ObtenerFactura_CuandoExiste_DebeRetornarResponse()
        {
            // Arrange
            int id = 5;
            var facturaFake = new FacturaEncabezado
            {
                Id = id,
                NumeroFactura = "FAC-000005",
                ClienteId = 1,
                ClienteNombreHistorico = "Ana Ruiz",
                Detalles = new List<FacturaDetalle>()
            };

            _facturaRepoMock.Setup(f => f.ObtenerPorId(id)).Returns(facturaFake);

            // Act
            var resultado = _service.ObtenerFactura(id);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("FAC-000005", resultado.NumeroFactura);
        }

        [TestMethod]
        public void ObtenerFactura_CuandoNoExiste_DebeRetornarNull()
        {
            // Arrange
            _facturaRepoMock.Setup(f => f.ObtenerPorId(99)).Returns((FacturaEncabezado)null);

            // Act
            var resultado = _service.ObtenerFactura(99);

            // Assert
            Assert.IsNull(resultado);
        }

        #endregion
    }
}