using System;
using System.Linq;
using ProductosHex.Application.DTOs;
using ProductosHex.Domain.Entities;
using ProductosHex.Domain.Ports;

namespace ProductosHex.Application.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IProductoRepository _productoRepo;
        private readonly ILoggerService _logger;

        public FacturaService(
            IFacturaRepository facturaRepo,
            IClienteRepository clienteRepo,
            IProductoRepository productoRepo,
            ILoggerService logger)
        {
            _facturaRepo = facturaRepo;
            _clienteRepo = clienteRepo;
            _productoRepo = productoRepo;
            _logger = logger;
        }

        public FacturaResponse CrearFactura(CrearFacturaRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Intento de crear factura con una solicitud nula.");
                throw new ArgumentException("La solicitud no puede ser nula.");
            }

            _logger.LogInfo($"Iniciando proceso de creación de factura para Cliente Doc: {request.NumeroDocumento}");

            var cliente = _clienteRepo.ObtenerPorDocumento(request.NumeroDocumento);
            if (cliente == null)
            {
                _logger.LogWarning($"Intento fallido de crear factura: El cliente con documento '{request.NumeroDocumento}' no existe.");
                throw new ArgumentException($"El cliente con documento '{request.NumeroDocumento}' no existe");
            }

            if (request.Detalles == null || !request.Detalles.Any())
            {
                _logger.LogWarning($"Intento fallido de crear factura para cliente Doc '{request.NumeroDocumento}': No incluye detalles.");
                throw new ArgumentException("La factura debe tener al menos un detalle");
            }

            if (request.Descuento < 0 || request.Descuento > 100)
            {
                _logger.LogWarning($"Intento fallido: Porcentaje de descuento global inválido ({request.Descuento}%).");
                throw new ArgumentException("El porcentaje de descuento debe estar entre 0 y 100");
            }

            var factura = new FacturaEncabezado
            {
                ClienteId = cliente.Id,
                ClienteNombreHistorico = cliente.Nombre,
                ClienteEmailHistorico = cliente.Email,
                ClienteDocumentoHistorico = cliente.NumeroDocumento,
                Descuento = request.Descuento,
                Fecha = DateTime.Now
            };

            decimal subtotalNetoAcumulado = 0;
            decimal totalIvaAcumulado = 0;

            foreach (var detalleReq in request.Detalles)
            {
                if (detalleReq.Cantidad <= 0)
                {
                    _logger.LogWarning($"Intento fallido: Cantidad inválida ({detalleReq.Cantidad}) para el producto ID {detalleReq.ProductoId}.");
                    throw new ArgumentException($"La cantidad del producto {detalleReq.ProductoId} debe ser mayor a cero");
                }

                if (detalleReq.PorcentajeDescuento < 0 || detalleReq.PorcentajeDescuento > 100)
                {
                    throw new ArgumentException($"Porcentaje de descuento inválido para el producto ID {detalleReq.ProductoId}");
                }

                var producto = _productoRepo.ObtenerPorId(detalleReq.ProductoId);
                if (producto == null)
                {
                    _logger.LogWarning($"Intento fallido: El producto ID {detalleReq.ProductoId} no existe.");
                    throw new ArgumentException($"El producto {detalleReq.ProductoId} no existe");
                }

                if (producto.Stock < detalleReq.Cantidad)
                {
                    _logger.LogWarning($"Stock insuficiente para '{producto.Nombre}'. Solicitado: {detalleReq.Cantidad}, Actual: {producto.Stock}.");
                    throw new ArgumentException($"Stock insuficiente para '{producto.Nombre}'. Stock actual: {producto.Stock}");
                }

                // 1. Precio bruto por ítem
                var precioBruto = detalleReq.Cantidad * producto.Precio;

                // 2. Descuentos en cascada (por producto y global)
                var factorDescuentoProducto = 1 - (detalleReq.PorcentajeDescuento / 100m);
                var factorDescuentoGlobal = 1 - (request.Descuento / 100m);

                var baseNetaItem = precioBruto * factorDescuentoProducto * factorDescuentoGlobal;

                // 3. IVA sobre la base gravable ya descontada
                var valorIvaItem = baseNetaItem * (producto.PorcentajeIva / 100m);

                factura.Detalles.Add(new FacturaDetalle
                {
                    ProductoId = producto.Id,
                    ProductoNombreHistorico = producto.Nombre,
                    Cantidad = detalleReq.Cantidad,
                    PrecioUnitario = producto.Precio,
                    PrecioBruto = precioBruto,
                    // Si el ítem no especifica un descuento propio, se toma el global
                    PorcentajeDescuento = detalleReq.PorcentajeDescuento > 0 ? detalleReq.PorcentajeDescuento : request.Descuento,
                    PorcentajeIva = producto.PorcentajeIva,
                    ValorIva = valorIvaItem
                });

                subtotalNetoAcumulado += baseNetaItem;
                totalIvaAcumulado += valorIvaItem;

                producto.Stock -= detalleReq.Cantidad;
                _productoRepo.Actualizar(producto);
            }

            factura.Iva = totalIvaAcumulado;
            factura.ValorTotal = subtotalNetoAcumulado + totalIvaAcumulado;

            _facturaRepo.Agregar(factura);

            if (string.IsNullOrEmpty(factura.NumeroFactura))
            {
                factura.NumeroFactura = $"FAC-{factura.Id:D6}";
            }

            _logger.LogInfo($"Factura {factura.NumeroFactura} (ID: {factura.Id}) creada exitosamente. Total: {factura.ValorTotal:C}");

            return MapearAResponse(factura);
        }

        public FacturaResponse ObtenerFactura(int id)
        {
            _logger.LogInfo($"Consultando factura ID: {id}");

            var factura = _facturaRepo.ObtenerPorId(id);
            if (factura == null)
            {
                _logger.LogWarning($"Factura ID {id} no fue encontrada.");
                return null;
            }

            return MapearAResponse(factura);
        }

        public PagedResult<FacturaResponse> ObtenerPaginado(int pagina, int tamanoPagina)
        {
            _logger.LogInfo($"Consultando listado de facturas paginado: Página {pagina}, Tamaño {tamanoPagina}");

            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1 || tamanoPagina > 100) tamanoPagina = 10;

            var (items, total) = _facturaRepo.ObtenerPaginado(pagina, tamanoPagina);

            var respuestas = items.Select(MapearAResponse).ToList();

            return new PagedResult<FacturaResponse>
            {
                Items = respuestas,
                TotalRegistros = total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        private FacturaResponse MapearAResponse(FacturaEncabezado factura)
        {
            return new FacturaResponse
            {
                Id = factura.Id,
                NumeroFactura = factura.NumeroFactura ?? $"FAC-{factura.Id:D6}",
                Fecha = factura.Fecha != default ? factura.Fecha : DateTime.Now,
                Cliente = new ClienteResumen
                {
                    Id = factura.ClienteId,
                    Nombre = factura.ClienteNombreHistorico,
                    NumeroDocumento = factura.ClienteDocumentoHistorico ?? factura.Cliente?.NumeroDocumento
                },
                Descuento = factura.Descuento,
                Iva = factura.Iva,
                ValorTotal = factura.ValorTotal,
                Detalles = factura.Detalles.Select(d => new FacturaDetalleResponse
                {
                    ProductoId = d.ProductoId,
                    Producto = d.ProductoNombreHistorico,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    PrecioBruto = d.PrecioBruto,
                    PorcentajeDescuento = d.PorcentajeDescuento > 0 ? d.PorcentajeDescuento : factura.Descuento,
                    PorcentajeIva = d.PorcentajeIva,
                    ValorIva = d.ValorIva
                }).ToList()
            };
        }
    }
}