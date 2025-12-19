using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiendaRopaPOS.Models;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class HistorialView : UserControl
    {
        private readonly HistorialService _historialService;
        private readonly ClienteService _clienteService;
        private readonly VentaService _ventaService;
        private Pedido? _pedidoSeleccionado;

        public HistorialView()
        {
            InitializeComponent();
            _historialService = new HistorialService();
            _clienteService = new ClienteService();
            _ventaService = new VentaService();
            
            Loaded += async (s, e) =>
            {
                await LoadClientes();
            };
        }

        private async Task LoadClientes()
        {
            try
            {
                var clientes = await _clienteService.ObtenerClientesAsync();
                CmbClientes.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnCargarHistorial.IsEnabled = CmbClientes.SelectedItem != null;
        }

        private async void BtnCargarHistorial_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClientes.SelectedItem is not Cliente cliente)
            {
                return;
            }

            try
            {
                var pedidos = await _historialService.ObtenerHistorialClienteAsync(cliente.IdCliente);
                DgPedidos.ItemsSource = pedidos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar historial: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgPedidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _pedidoSeleccionado = DgPedidos.SelectedItem as Pedido;
            bool tieneSeleccion = _pedidoSeleccionado != null;
            BtnVerFactura.IsEnabled = tieneSeleccion;
            // Solo permitir eliminar pedidos pendientes
            BtnEliminarPedido.IsEnabled = tieneSeleccion && 
                EsPendiente(_pedidoSeleccionado?.EstadoPedido);
            BtnActualizarEstado.IsEnabled = tieneSeleccion && PuedeActualizarEstado(_pedidoSeleccionado?.EstadoPedido);
        }

        private static bool EsPendiente(string? estadoPedido)
        {
            if (string.IsNullOrWhiteSpace(estadoPedido))
                return false;

            var estado = estadoPedido.Trim();
            return estado.Equals("P", StringComparison.OrdinalIgnoreCase) ||
                   estado.Contains("Pend", StringComparison.OrdinalIgnoreCase);
        }

        private static bool PuedeActualizarEstado(string? estadoPedido)
        {
            if (string.IsNullOrWhiteSpace(estadoPedido))
                return false;

            var codigo = ObtenerCodigoEstadoDesdeTexto(estadoPedido);
            return codigo != "E";
        }

        private static string ObtenerCodigoEstadoDesdeTexto(string? estadoPedido)
        {
            if (string.IsNullOrWhiteSpace(estadoPedido))
                return "P";

            if (estadoPedido.Contains("Enviado", StringComparison.OrdinalIgnoreCase))
                return "E";

            if (estadoPedido.Contains("Confirmado", StringComparison.OrdinalIgnoreCase) ||
                estadoPedido.Contains("Pagado", StringComparison.OrdinalIgnoreCase))
                return "C";

            return "P";
        }

        private async void BtnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            if (_pedidoSeleccionado == null)
                return;

            await MostrarFactura(_pedidoSeleccionado.IdPedido);
        }

        private async void BtnActualizarEstado_Click(object sender, RoutedEventArgs e)
        {
            if (_pedidoSeleccionado == null)
                return;

            if (!PuedeActualizarEstado(_pedidoSeleccionado.EstadoPedido))
            {
                MessageBox.Show("No se puede actualizar el estado de un pedido confirmado/pagado.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var estadoActual = ObtenerCodigoEstadoDesdeTexto(_pedidoSeleccionado.EstadoPedido);
            var inputDialog = new InputDialog(
                "Actualizar Estado del Pedido",
                $"Pedido #{_pedidoSeleccionado.IdPedido}\nEstado actual: {_pedidoSeleccionado.EstadoPedido}\n\nIngrese el nuevo estado:\n- P = Pendiente\n- E = Enviado\n- C = Confirmado/Pagado",
                estadoActual == "C" ? "E" : "C");

            if (inputDialog.ShowDialog() != true || string.IsNullOrWhiteSpace(inputDialog.Result))
                return;

            var estadoInput = inputDialog.Result.Trim().ToUpperInvariant();
            if (estadoInput != "P" && estadoInput != "E" && estadoInput != "C")
            {
                MessageBox.Show("Estado inválido. Debe ser P (Pendiente), E (Enviado) o C (Confirmado/Pagado)", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (estadoActual == "C" && estadoInput != "E")
            {
                MessageBox.Show("Un pedido confirmado/pagado (C) solo puede pasar a Enviado/Entregado (E).", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _ventaService.ActualizarEstadoPedidoAsync(_pedidoSeleccionado.IdPedido, estadoInput);

                if (CmbClientes.SelectedItem is Cliente cliente)
                {
                    var pedidos = await _historialService.ObtenerHistorialClienteAsync(cliente.IdCliente);
                    DgPedidos.ItemsSource = pedidos;
                }

                MessageBox.Show("Estado actualizado exitosamente", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar estado: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DgPedidos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_pedidoSeleccionado != null)
            {
                await MostrarFactura(_pedidoSeleccionado.IdPedido);
            }
        }

        private async Task MostrarFactura(int idPedido)
        {
            try
            {
                var factura = await _ventaService.ObtenerFacturaAsync(idPedido);
                var dialog = new FacturaDialog(factura)
                {
                    Owner = Window.GetWindow(this)
                };
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar factura: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnEliminarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (_pedidoSeleccionado == null)
                return;

            if (!EsPendiente(_pedidoSeleccionado.EstadoPedido))
            {
                MessageBox.Show("Solo se pueden eliminar pedidos en estado 'Pendiente'.", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Está seguro de eliminar el pedido #{_pedidoSeleccionado.IdPedido}?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacion == MessageBoxResult.Yes)
            {
                try
                {
                    await _ventaService.EliminarPedidoAsync(_pedidoSeleccionado.IdPedido);
                    
                    // Recargar historial
                    if (CmbClientes.SelectedItem is Cliente cliente)
                    {
                        var pedidos = await _historialService.ObtenerHistorialClienteAsync(cliente.IdCliente);
                        DgPedidos.ItemsSource = pedidos;
                    }
                    
                    MessageBox.Show("Pedido eliminado exitosamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar pedido: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

