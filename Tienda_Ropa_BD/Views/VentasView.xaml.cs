using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiendaRopaPOS.Models;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class VentasView : UserControl
    {
        private readonly VentaService _ventaService;
        private readonly ClienteService _clienteService;
        private readonly InventarioService _inventarioService;
        private int _idPedidoActual = 0;
        private string _estadoPedidoActual = "";
        private List<ItemPedido> _itemsCarrito = new();

        public VentasView()
        {
            InitializeComponent();
            _ventaService = new VentaService();
            _clienteService = new ClienteService();
            _inventarioService = new InventarioService();
            
            Loaded += async (s, e) =>
            {
                await LoadClientes();
                await LoadProductos();
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

        private async Task LoadProductos()
        {
            try
            {
                var productos = await _inventarioService.ObtenerProductosAsync();
                DgProductos.ItemsSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnNuevoPedido.IsEnabled = CmbClientes.SelectedItem != null && _idPedidoActual == 0;
        }

        private async void BtnCancelarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (_idPedidoActual == 0)
            {
                return;
            }

            if (!string.Equals(_estadoPedidoActual, "P", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    $"No se puede cancelar el pedido #{_idPedidoActual} porque su estado actual es '{_estadoPedidoActual}'.\n\n" +
                    "Solo se pueden cancelar pedidos en estado Pendiente (P).",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Está seguro de cancelar el pedido #{_idPedidoActual}? Se eliminarán todos los productos del carrito.",
                "Confirmar Cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacion == MessageBoxResult.Yes)
            {
                try
                {
                    await _ventaService.EliminarPedidoAsync(_idPedidoActual);
                    
                    // Resetear
                    _idPedidoActual = 0;
                    _estadoPedidoActual = "";
                    _itemsCarrito.Clear();
                    DgCarrito.ItemsSource = null;
                    ActualizarTotales();
                    CmbClientes.SelectedItem = null;
                    CmbClientes.IsEnabled = true;
                    BtnNuevoPedido.IsEnabled = false;
                    BtnCancelarPedido.IsEnabled = false;
                    BtnAgregar.IsEnabled = false;
                    BtnFinalizarVenta.IsEnabled = false;
                    BtnActualizarEstado.IsEnabled = false;
                    
                    MessageBox.Show("Pedido cancelado exitosamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cancelar pedido: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnNuevoPedido_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClientes.SelectedItem is not Cliente cliente)
            {
                MessageBox.Show("Por favor seleccione un cliente", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _idPedidoActual = await _ventaService.CrearPedidoAsync(cliente.IdCliente);
                _estadoPedidoActual = "P";
                _itemsCarrito.Clear();
                DgCarrito.ItemsSource = null;
                ActualizarTotales();
                
                BtnNuevoPedido.IsEnabled = false;
                BtnCancelarPedido.IsEnabled = true;
                BtnAgregar.IsEnabled = true;
                BtnFinalizarVenta.IsEnabled = false;
                BtnActualizarEstado.IsEnabled = true;
                CmbClientes.IsEnabled = false;
                
                MessageBox.Show($"Pedido #{_idPedidoActual} creado exitosamente.\n\nAhora puede agregar productos al carrito:\n- Seleccione un producto y haga clic en 'Agregar'\n- O haga doble clic en un producto para agregarlo automáticamente", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear pedido: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (_idPedidoActual == 0)
            {
                MessageBox.Show("Debe crear un pedido primero", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgProductos.SelectedItem is not Producto producto)
            {
                MessageBox.Show("Por favor seleccione un producto", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor ingrese una cantidad válida", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _ventaService.AgregarProductoAsync(_idPedidoActual, producto.IdProducto, cantidad);
                await ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ActualizarCarrito()
        {
            try
            {
                // Obtener items con id_detalle para poder eliminarlos
                // ObtenerItemsPedidoAsync ya incluye los nombres de productos
                var items = await _ventaService.ObtenerItemsPedidoAsync(_idPedidoActual);
                
                _itemsCarrito = items;
                DgCarrito.ItemsSource = _itemsCarrito;
                DgCarrito.SelectedItem = null;
                
                // Calcular totales desde los items
                ActualizarTotalesDesdeItems(items);

                BtnFinalizarVenta.IsEnabled = _idPedidoActual > 0 && _itemsCarrito.Count > 0;
                BtnEliminarItem.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar carrito: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarTotalesDesdeItems(List<ItemPedido> items)
        {
            if (items == null || items.Count == 0)
            {
                TxtSubtotal.Text = "$0.00";
                TxtDescuento.Text = "$0.00";
                TxtTotal.Text = "$0.00";
                return;
            }

            // Calcular subtotal (suma de todos los subtotales de items)
            decimal subtotal = items.Sum(item => item.Subtotal);
            
            // Calcular descuento total (suma de descuentos por cantidad)
            decimal descuentoTotal = items.Sum(item => item.Descuento * item.Cantidad);
            
            // El total es el subtotal (ya incluye los descuentos aplicados)
            decimal total = subtotal;

            TxtSubtotal.Text = (subtotal + descuentoTotal).ToString("C2", CultureInfo.CurrentCulture);
            TxtDescuento.Text = descuentoTotal.ToString("C2", CultureInfo.CurrentCulture);
            TxtTotal.Text = total.ToString("C2", CultureInfo.CurrentCulture);
        }

        private void ActualizarTotales(Factura? factura = null)
        {
            if (factura != null)
            {
                TxtSubtotal.Text = factura.Subtotal.ToString("C2", CultureInfo.CurrentCulture);
                TxtDescuento.Text = factura.DescuentoTotal.ToString("C2", CultureInfo.CurrentCulture);
                TxtTotal.Text = factura.Total.ToString("C2", CultureInfo.CurrentCulture);
            }
            else
            {
                TxtSubtotal.Text = "$0.00";
                TxtDescuento.Text = "$0.00";
                TxtTotal.Text = "$0.00";
            }
        }

        private void DgProductos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Doble clic en producto = agregar automáticamente con cantidad 1
            if (_idPedidoActual == 0)
            {
                MessageBox.Show("Debe crear un pedido primero haciendo clic en 'Nuevo Pedido'", "Información", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DgProductos.SelectedItem is Producto producto)
            {
                TxtCantidad.Text = "1";
                BtnAgregar_Click(sender, e);
            }
        }

        private void DgCarrito_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool tieneSeleccion = DgCarrito.SelectedItem != null;
            BtnEliminarItem.IsEnabled = tieneSeleccion;
            BtnActualizarEstado.IsEnabled = _idPedidoActual > 0;
        }

        private async void DgCarrito_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Solo procesar si es la columna de Cantidad
            if (e.Column.Header.ToString() != "Cant." || e.EditAction != DataGridEditAction.Commit)
                return;

            if (e.Row.Item is not ItemPedido item)
                return;

            var textBox = e.EditingElement as System.Windows.Controls.TextBox;
            if (textBox == null) return;

            if (!int.TryParse(textBox.Text, out int nuevaCantidad) || nuevaCantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a cero", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Cancel = true;
                return;
            }

            // Si la cantidad es la misma, no hacer nada
            if (nuevaCantidad == item.Cantidad)
                return;

            try
            {
                // Eliminar el item actual
                await _ventaService.EliminarProductoPedidoAsync(item.IdItem);
                
                // Agregar con la nueva cantidad
                await _ventaService.AgregarProductoAsync(_idPedidoActual, item.IdProducto, nuevaCantidad);
                
                // Actualizar carrito
                await ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar cantidad: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                e.Cancel = true;
            }
        }

        private async void BtnEliminarItem_Click(object sender, RoutedEventArgs e)
        {
            if (DgCarrito.SelectedItem is not ItemPedido item)
            {
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Está seguro de eliminar '{item.NombreProducto}' del carrito?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion == MessageBoxResult.Yes)
            {
                try
                {
                    await _ventaService.EliminarProductoPedidoAsync(item.IdItem);
                    await ActualizarCarrito();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnActualizarEstado_Click(object sender, RoutedEventArgs e)
        {
            if (_idPedidoActual == 0)
            {
                MessageBox.Show("No hay un pedido activo", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Crear diálogo para seleccionar estado
            var estados = new[] { "Pendiente (P)", "Confirmado/Pagado (C)", "Enviado (E)" };
            var estadosValores = new[] { "P", "C", "E" };
            
            var inputDialog = new InputDialog(
                "Actualizar Estado del Pedido", 
                $"Pedido #{_idPedidoActual}\n\nSeleccione el nuevo estado:\n1. Pendiente (P)\n2. Confirmado/Pagado (C)\n3. Enviado (E)\n\nIngrese P, C o E:",
                "P");
            
            if (inputDialog.ShowDialog() != true || string.IsNullOrWhiteSpace(inputDialog.Result))
                return;

            var estadoInput = inputDialog.Result.Trim().ToUpper();
            
            if (estadoInput != "P" && estadoInput != "C" && estadoInput != "E")
            {
                MessageBox.Show("Estado inválido. Debe ser P (Pendiente), C (Confirmado) o E (Enviado)", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _ventaService.ActualizarEstadoPedidoAsync(_idPedidoActual, estadoInput);
                _estadoPedidoActual = estadoInput;
                
                string estadoNombre = estadoInput == "P" ? "Pendiente" : 
                                     estadoInput == "C" ? "Confirmado/Pagado" : "Enviado";
                
                MessageBox.Show($"Estado del pedido #{_idPedidoActual} actualizado a: {estadoNombre}", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar estado: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnFinalizarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (_idPedidoActual == 0)
            {
                return;
            }

            if (_itemsCarrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var decision = MessageBox.Show(
                "¿Cómo desea finalizar este pedido?\n\n" +
                "Sí = Marcar como Pagado (C)\n" +
                "No = Guardar como Reserva/Pendiente (P)\n" +
                "Cancelar = Volver",
                "Finalizar Pedido",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (decision == MessageBoxResult.Cancel)
                return;

            try
            {
                var factura = await _ventaService.ObtenerFacturaAsync(_idPedidoActual);

                var mensajeExito = "";
                if (decision == MessageBoxResult.Yes)
                {
                    await _ventaService.ActualizarEstadoPedidoAsync(_idPedidoActual, "C");
                    _estadoPedidoActual = "C";
                    mensajeExito = "Venta finalizada exitosamente";
                }
                else
                {
                    _estadoPedidoActual = "P";
                    mensajeExito = "Pedido guardado como reserva (Pendiente)";
                }

                var facturaDialog = new FacturaDialog(factura)
                {
                    Owner = Window.GetWindow(this)
                };
                facturaDialog.ShowDialog();

                _idPedidoActual = 0;
                _estadoPedidoActual = "";
                _itemsCarrito.Clear();
                DgCarrito.ItemsSource = null;
                ActualizarTotales();
                CmbClientes.SelectedItem = null;
                CmbClientes.IsEnabled = true;
                BtnNuevoPedido.IsEnabled = false;
                BtnCancelarPedido.IsEnabled = false;
                BtnAgregar.IsEnabled = false;
                BtnFinalizarVenta.IsEnabled = false;
                BtnActualizarEstado.IsEnabled = false;
                BtnEliminarItem.IsEnabled = false;

                MessageBox.Show(mensajeExito, "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

