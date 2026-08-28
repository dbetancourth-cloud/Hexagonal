using System;
using System.Collections.Generic;

namespace ProductosHex.Application.DTOs
{
    public class CrearFacturaRequest
    {
        public string NumeroDocumento { get; set; }
        public decimal Descuento { get; set; } // Porcentaje global (ej: 10 para 10%)
        public List<CrearFacturaDetalleRequest> Detalles { get; set; }
    }

    public class CrearFacturaDetalleRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PorcentajeDescuento { get; set; } // Porcentaje por producto (ej: 5 para 5%)
    }

    public class FacturaResponse
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public ClienteResumen Cliente { get; set; }
        public decimal Descuento { get; set; }
        public decimal Iva { get; set; }
        public decimal ValorTotal { get; set; }
        public List<FacturaDetalleResponse> Detalles { get; set; }
    }

    public class ClienteResumen
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NumeroDocumento { get; set; }
    }

    public class FacturaDetalleResponse
    {
        public int ProductoId { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioBruto { get; set; }
        public decimal PorcentajeDescuento { get; set; } // Opcional: para retornar el % aplicado al ítem
        public decimal PorcentajeIva { get; set; }
        public decimal ValorIva { get; set; }
    }
}