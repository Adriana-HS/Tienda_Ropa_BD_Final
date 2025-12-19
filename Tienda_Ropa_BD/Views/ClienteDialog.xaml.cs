using System;
using System.Windows;
using TiendaRopaPOS.Models;

namespace TiendaRopaPOS.Views
{
    public partial class ClienteDialog : Window
    {
        public string Nombre => TxtNombre.Text.Trim();
        public string Apellido => TxtApellido.Text.Trim();
        public string Email => TxtEmail.Text.Trim();
        public string Telefono => TxtTelefono.Text.Trim();

        public ClienteDialog(Cliente? cliente = null)
        {
            try
            {
                InitializeComponent();
                
                if (cliente != null)
                {
                    if (TxtNombre != null) TxtNombre.Text = cliente.Nombre;
                    if (TxtApellido != null) TxtApellido.Text = cliente.Apellido;
                    if (TxtEmail != null) TxtEmail.Text = cliente.Email;
                    if (TxtTelefono != null) TxtTelefono.Text = cliente.Telefono;
                    Title = "Editar Cliente";
                }
                else
                {
                    Title = "Nuevo Cliente";
                }
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
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Telefono))
            {
                MessageBox.Show("Por favor complete todos los campos", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

