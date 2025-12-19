namespace TiendaRopaPOS.Models
{
    public class Subcategoria
    {
        public int IdSubcategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Detalles { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
    }
}