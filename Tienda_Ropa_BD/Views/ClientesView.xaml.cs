using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiendaRopaPOS.Models;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class ClientesView : UserControl
    {
        private readonly ClienteService _clienteService;
        private System.Windows.Threading.DispatcherTimer? _searchTimer;

        public ClientesView()
        {
            InitializeComponent();
            _clienteService = new ClienteService();
            
            // Timer para búsqueda con delay
            _searchTimer = new System.Windows.Threading.DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(500);
            _searchTimer.Tick += async (s, e) =>
            {
                _searchTimer.Stop();
                await LoadClientes(TxtBusqueda.Text);
            };
            
            Loaded += async (s, e) =>
            {
                await LoadClientes(); 
            };
        }

        private async Task LoadClientes(string? filtro = null)
        {
            try
            {
                var clientes = await _clienteService.ObtenerClientesAsync(filtro);
                DgClientes.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer?.Stop();
            _searchTimer?.Start();
        }

        private async void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ClienteDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _clienteService.InsertarClienteAsync(
                        dialog.Nombre, dialog.Apellido, dialog.Email, dialog.Telefono);
                    MessageBox.Show("Cliente creado exitosamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadClientes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnBuscarDetalle_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Búsqueda Detallada", "Ingrese ID de Cliente o Email:", "");
            if (inputDialog.ShowDialog() != true || string.IsNullOrWhiteSpace(inputDialog.Result))
                return;
            
            var input = inputDialog.Result;

            try
            {
                Cliente? cliente = null;
                if (int.TryParse(input, out int id))
                {
                    cliente = await _clienteService.ObtenerClienteDetalleAsync(idCliente: id);
                }
                else
                {
                    // Buscar por email usando el SP
                    cliente = await _clienteService.ObtenerClienteDetalleAsync(email: input);
                }

                if (cliente != null)
                {
                    var dialog = new ClienteDialog(cliente);
                    if (dialog.ShowDialog() == true)
                    {
                        await _clienteService.EditarClienteAsync(
                            cliente.IdCliente, dialog.Nombre, dialog.Apellido, dialog.Email, dialog.Telefono);
                        MessageBox.Show("Cliente actualizado exitosamente", "Éxito", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadClientes();
                    }
                }
                else
                {
                    MessageBox.Show("Cliente no encontrado", "Información", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Puedes agregar lógica aquí si necesitas
        }

        private async void DgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgClientes.SelectedItem is Cliente cliente)
            {
                var dialog = new ClienteDialog(cliente);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        await _clienteService.EditarClienteAsync(
                            cliente.IdCliente, dialog.Nombre, dialog.Apellido, dialog.Email, dialog.Telefono);
                        MessageBox.Show("Cliente actualizado exitosamente", "Éxito", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadClientes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

