using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProductosHex.Domain.Entities
{
    [Table("Facturas")]
    public class FacturaEncabezado
    {
        public FacturaEncabezado()
        {
            Fecha = DateTime.Now;
            Detalles = new List<FacturaDetalle>();
        }
        public int Id { get; set; }
        [StringLength(20)]
        public string NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }
        public string ClienteNombreHistorico { get; set; }
        public string ClienteEmailHistorico { get; set; }
        public string ClienteDocumentoHistorico { get; set; }
        public decimal Descuento { get; set; }
        public decimal Iva { get; set; }
        public decimal ValorTotal { get; set; }
        public virtual ICollection<FacturaDetalle> Detalles { get; set; }
    }
}