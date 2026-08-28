using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosHex.Domain.Entities
{
    [Table("FacturaDetalles")]
    public class FacturaDetalle
    {
        public int Id { get; set; }

        public int FacturaEncabezadoId { get; set; }
        public virtual FacturaEncabezado FacturaEncabezado { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        public string ProductoNombreHistorico { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioBruto { get; set; }
        public decimal PorcentajeDescuento { get; set; } 
        public decimal PorcentajeIva { get; set; }
        public decimal ValorIva { get; set; }
    }
}