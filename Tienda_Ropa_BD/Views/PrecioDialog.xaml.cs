using System;
using System.Globalization;
using System.Windows;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Views
{
    public partial class PrecioDialog : Window
    {
        public decimal PrecioBase { get; private set; }
        public decimal PrecioVenta { get; private set; }

        public PrecioDialog(Producto producto)
        {
            try
            {
                InitializeComponent();
                if (TxtProductoNombre != null)
                    TxtProductoNombre.Text = $"Producto: {producto.Nombre}";
                if (TxtPrecioBase != null)
                    TxtPrecioBase.Text = producto.PrecioBase.ToString("F2", CultureInfo.InvariantCulture);
                if (TxtPrecioVenta != null)
                    TxtPrecioVenta.Text = producto.PrecioVenta.ToString("F2", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el diálogo:\n\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw; // Re-lanzar para que el diálogo no se muestre si hay error crítico
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtPrecioBase == null || TxtPrecioVenta == null ||
                    !decimal.TryParse(TxtPrecioBase.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioBase) ||
                    !decimal.TryParse(TxtPrecioVenta.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioVenta))
                {
                    MessageBox.Show("Por favor ingrese valores numéricos válidos", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (precioBase < 0 || precioVenta < 0)
                {
                    MessageBox.Show("Los precios no pueden ser negativos", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                PrecioBase = precioBase;
                PrecioVenta = precioVenta;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

