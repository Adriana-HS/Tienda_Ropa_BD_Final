using System;
using System.Globalization;
using System.Windows;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Views
{
    public partial class StockDialog : Window
    {
        public int CantidadAjuste { get; private set; }

        public StockDialog(Producto producto)
        {
            try
            {
                InitializeComponent();
                if (TxtProductoNombre != null)
                    TxtProductoNombre.Text = $"Producto: {producto.Nombre}";
                if (TxtStockActual != null)
                    TxtStockActual.Text = $"Stock Actual: {producto.StockActual}";
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
                if (TxtCantidad == null || !int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
                {
                    MessageBox.Show("Por favor ingrese una cantidad válida mayor a cero", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (RbEntrada == null)
                {
                    MessageBox.Show("Error: Los controles no están inicializados correctamente", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Si es salida, convertir a negativo
                CantidadAjuste = RbEntrada.IsChecked == true ? cantidad : -cantidad;
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

