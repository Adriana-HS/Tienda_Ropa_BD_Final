using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiendaRopaPOS.Models;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class InventarioView : UserControl
    {
        private readonly InventarioService _inventarioService;
        private Producto? _productoSeleccionado;

        public InventarioView()
        {
            InitializeComponent();
            _inventarioService = new InventarioService();
            
            Loaded += async (s, e) =>
            {
                await LoadProductos();
            };
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

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _productoSeleccionado = DgProductos.SelectedItem as Producto;
            bool tieneSeleccion = _productoSeleccionado != null;
            BtnEditarPrecio.IsEnabled = tieneSeleccion;
            BtnAjustarStock.IsEnabled = tieneSeleccion;
            BtnVerDetalle.IsEnabled = tieneSeleccion;
        }

        private async void BtnEditarPrecio_Click(object sender, RoutedEventArgs e)
        {
            if (_productoSeleccionado == null) return;

            var dialog = new PrecioDialog(_productoSeleccionado);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _inventarioService.EditarPrecioProductoAsync(
                        _productoSeleccionado.IdProducto, dialog.PrecioBase, dialog.PrecioVenta);
                    MessageBox.Show("Precios actualizados exitosamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnAjustarStock_Click(object sender, RoutedEventArgs e)
        {
            if (_productoSeleccionado == null) return;

            var dialog = new StockDialog(_productoSeleccionado);
            if (dialog.ShowDialog() == true)
            {
            try
            {
                await _inventarioService.ActualizarStockAsync(
                    _productoSeleccionado.IdProducto, dialog.CantidadAjuste);
                MessageBox.Show("Stock actualizado exitosamente", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadProductos();
            }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (_productoSeleccionado == null) return;

            try
            {
                var producto = await _inventarioService.ObtenerProductoInfoAsync(_productoSeleccionado.IdProducto);
                if (producto != null)
                {
                    var dialog = new ProductoDetalleDialog(producto);
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_productoSeleccionado != null)
            {
                BtnVerDetalle_Click(sender, e);
            }
        }

        private async void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ProductoDialog();
                var result = dialog.ShowDialog();
                
                if (result == true)
                {
                    try
                    {
                        await _inventarioService.InsertarProductoAsync(
                            dialog.Nombre,
                            dialog.Descripcion,
                            dialog.PrecioBase,
                            dialog.PrecioVenta,
                            dialog.Color,
                            dialog.Talla,
                            dialog.IdSubcategoria);
                        
                        MessageBox.Show("Producto creado exitosamente", "Éxito", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadProductos();
                    }
                    catch (Exception ex)
                    {
                        // Mostrar error detallado
                        var errorMsg = $"Error al crear producto:\n\n{ex.Message}";
                        if (ex.InnerException != null)
                        {
                            errorMsg += $"\n\nDetalle: {ex.InnerException.Message}";
                        }
                        MessageBox.Show(errorMsg, "Error al Crear Producto", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturar cualquier error al crear o mostrar el diálogo
                MessageBox.Show($"Error al abrir el diálogo de nuevo producto:\n\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnNuevaCategoria_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CategoriaDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var categoriaService = new CategoriaService();
                    await categoriaService.InsertarCategoriaAsync(dialog.Nombre, dialog.Detalles);
                    MessageBox.Show("Categoría creada exitosamente", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnNuevaSubcategoria_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SubcategoriaDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var categoriaService = new CategoriaService();
                    await categoriaService.AgregarSubcategoriaAsync(dialog.Nombre, dialog.IdCategoria, dialog.Detalles);
                    MessageBox.Show("Subcategoría creada exitosamente", "Éxito", 
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
}

