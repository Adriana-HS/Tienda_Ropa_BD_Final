using System;

namespace TiendaRopaPOS.Models
{
    public class Promocion
    {
        public int IdPromocion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = string.Empty; // "Porcentaje" o "Monto"
        public decimal? ValorPorcentaje { get; set; }
        public decimal? ValorMonto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activa { get; set; }
    }
}

