using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Views
{
    public partial class FacturaDialog : Window
    {
        public FacturaDialog(Factura factura)
        {
            InitializeComponent();
            
            TxtIdPedido.Text = factura.IdPedido.ToString();
            TxtCliente.Text = factura.Cliente;
            TxtFecha.Text = factura.Fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            TxtSubtotal.Text = factura.Subtotal.ToString("C2", CultureInfo.CurrentCulture);
            TxtDescuento.Text = factura.DescuentoTotal.ToString("C2", CultureInfo.CurrentCulture);
            TxtTotal.Text = factura.Total.ToString("C2", CultureInfo.CurrentCulture);
            
            DgItems.ItemsSource = factura.Items;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            var printableWidth = printDialog.PrintableAreaWidth;
            var printableHeight = printDialog.PrintableAreaHeight;

            var originalTransform = RootGrid.LayoutTransform;
            try
            {
                RootGrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                RootGrid.Arrange(new Rect(RootGrid.DesiredSize));

                var scaleX = printableWidth / RootGrid.ActualWidth;
                var scaleY = printableHeight / RootGrid.ActualHeight;
                var scale = Math.Min(scaleX, scaleY);
                if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
                    scale = 1;

                RootGrid.LayoutTransform = new ScaleTransform(scale, scale);
                RootGrid.Measure(new Size(printableWidth, printableHeight));
                RootGrid.Arrange(new Rect(new Point(0, 0), new Size(printableWidth, printableHeight)));

                printDialog.PrintVisual(RootGrid, $"Factura #{TxtIdPedido.Text}");
            }
            finally
            {
                RootGrid.LayoutTransform = originalTransform;
            }
        }
    }
}

