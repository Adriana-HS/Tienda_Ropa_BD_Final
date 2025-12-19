namespace TiendaRopaPOS.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Subcategoria { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Talla { get; set; } = string.Empty;
    }
}

