using System;
using System.Collections.Generic;

namespace TiendaRopaPOS.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
    
    public class ItemPedido
    {
    public int IdItem { get; set; }
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty; 
    public string Talla { get; set; } = string.Empty;

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }
    }
    
    public class Factura
    {
        public int IdPedido { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal Total { get; set; }
        public List<ItemPedido> Items { get; set; } = new();
    }
}

