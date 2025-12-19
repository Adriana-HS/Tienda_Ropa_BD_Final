using System.Globalization;
using System.Windows;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Views
{
    public partial class ProductoDetalleDialog : Window
    {
        public ProductoDetalleDialog(Producto producto)
        {
            InitializeComponent();
            
            TxtId.Text = producto.IdProducto.ToString();
            TxtNombre.Text = producto.Nombre;
            TxtColor.Text = producto.Color;
            TxtTalla.Text = producto.Talla;
            TxtDescripcion.Text = producto.Descripcion;
            TxtCategoria.Text = producto.Categoria;
            TxtSubcategoria.Text = producto.Subcategoria;
            TxtPrecioBase.Text = producto.PrecioBase.ToString("C2", CultureInfo.CurrentCulture);
            TxtPrecioVenta.Text = producto.PrecioVenta.ToString("C2", CultureInfo.CurrentCulture);
            TxtStock.Text = producto.StockActual.ToString();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

