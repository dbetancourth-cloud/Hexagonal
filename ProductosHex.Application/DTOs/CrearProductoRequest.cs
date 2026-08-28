using System.ComponentModel.DataAnnotations;

namespace ProductosHex.Application.DTOs
{
    public class CrearProductoRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe especificar un ID de categoría válido")]
        public int CategoriaId { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100")]
        public decimal PorcentajeIva { get; set; } = 19m;
    }
}