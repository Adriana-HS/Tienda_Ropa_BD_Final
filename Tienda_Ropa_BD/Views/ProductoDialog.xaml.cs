using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.SqlClient;
using TiendaRopaPOS.Config;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class ProductoDialog : Window
    {
        public class SubcategoriaItem
        {
            public int IdSubcategoria { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }

        public string Nombre => TxtNombre.Text.Trim();
        public string? Descripcion => string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim();
        public decimal PrecioBase { get; private set; }
        public decimal PrecioVenta { get; private set; }
        public string Color => TxtColor.Text.Trim();
        public string Talla => TxtTalla.Text.Trim();
        public int IdSubcategoria { get; private set; }

        public ProductoDialog()
        {
            try
            {
                InitializeComponent();
                // Cargar subcategorías después de que el diálogo se muestre
                Loaded += async (s, e) => 
                {
                    try
                    {
                        await LoadSubcategoriasAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar subcategorías:\n\n{ex.Message}", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el diálogo:\n\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw; // Re-lanzar para que el diálogo no se muestre si hay error crítico
            }
        }

        private async Task LoadSubcategoriasAsync()
        {
            try
            {
                var categoriaService = new CategoriaService();
                var subcategoriasDb = await categoriaService.ObtenerSubcategoriasAsync();

                var subcategorias = new List<SubcategoriaItem>();
                foreach (var sub in subcategoriasDb)
                {
                    subcategorias.Add(new SubcategoriaItem
                    {
                        IdSubcategoria = sub.IdSubcategoria,
                        Nombre = sub.Nombre
                    });
                }

                // Actualizar en el hilo de UI
                CmbSubcategoria.ItemsSource = subcategorias;
            }
            catch (Exception ex)
            {
                // Mostrar error en el hilo de UI
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar subcategorías:\n\n{ex.Message}\n\nDetalle: {ex.GetType().Name}\n\nEl diálogo permanecerá abierto pero no podrá guardar productos sin subcategorías.", "Error al Cargar Subcategorías", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MessageBox.Show("Por favor ingrese el nombre del producto", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrecioBase.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioBase) || precioBase < 0)
            {
                MessageBox.Show("Por favor ingrese un precio base válido (≥ 0)", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrecioVenta.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioVenta) || precioVenta < 0)
            {
                MessageBox.Show("Por favor ingrese un precio de venta válido (≥ 0)", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Color))
            {
                MessageBox.Show("Por favor ingrese el color del producto", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Talla))
            {
                MessageBox.Show("Por favor ingrese la talla del producto", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbSubcategoria.SelectedValue == null)
            {
                MessageBox.Show("Por favor seleccione una subcategoría", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PrecioBase = precioBase;
            PrecioVenta = precioVenta;
            IdSubcategoria = (int)CmbSubcategoria.SelectedValue;
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

