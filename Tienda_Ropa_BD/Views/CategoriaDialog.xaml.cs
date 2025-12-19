using System;
using System.Windows;

namespace TiendaRopaPOS.Views
{
    public partial class CategoriaDialog : Window
    {
        public string Nombre => TxtNombre.Text.Trim();
        public string? Detalles => string.IsNullOrWhiteSpace(TxtDetalles.Text) ? null : TxtDetalles.Text.Trim();

        public CategoriaDialog()
        {
            try
            {
                InitializeComponent();
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
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    MessageBox.Show("Por favor ingrese el nombre de la categoría", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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