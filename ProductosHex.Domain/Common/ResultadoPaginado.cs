using System.Collections.Generic;
namespace ProductosHex.Domain.Common
{
    public class ResultadoPaginado<T>
    {
        public List<T> Items { get; set; }
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalPaginas => (int)System.Math.Ceiling((double)TotalRegistros / TamanoPagina);
    }
}