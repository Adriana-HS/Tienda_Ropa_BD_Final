using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class SubcategoriaDialog : Window
    {
        public class CategoriaItem
        {
            public int IdCategoria { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }

        public string Nombre => TxtNombre.Text.Trim();
        public string? Detalles => string.IsNullOrWhiteSpace(TxtDetalles.Text) ? null : TxtDetalles.Text.Trim();
        public int IdCategoria { get; private set; }

        public SubcategoriaDialog()
        {
            try
            {
                InitializeComponent();
                // Cargar categorías después de que el diálogo se muestre
                Loaded += async (s, e) =>
                {
                    try
                    {
                        await LoadCategoriasAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar categorías:\n\n{ex.Message}", "Error",
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

        private async Task LoadCategoriasAsync()
        {
            try
            {
                var categoriaService = new CategoriaService();
                var categorias = await categoriaService.ObtenerCategoriasAsync();

                var categoriaItems = new List<CategoriaItem>();
                foreach (var cat in categorias)
                {
                    categoriaItems.Add(new CategoriaItem
                    {
                        IdCategoria = cat.IdCategoria,
                        Nombre = cat.Nombre
                    });
                }

                // Actualizar en el hilo de UI
                await Dispatcher.InvokeAsync(() =>
                {
                    CmbCategoria.ItemsSource = categoriaItems;
                });
            }
            catch (Exception ex)
            {
                // Mostrar error en el hilo de UI
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar categorías:\n\n{ex.Message}\n\nDetalle: {ex.GetType().Name}\n\nEl diálogo permanecerá abierto pero no podrá guardar subcategorías sin categorías.", "Error al Cargar Categorías",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MessageBox.Show("Por favor ingrese el nombre de la subcategoría", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbCategoria.SelectedItem == null)
            {
                MessageBox.Show("Por favor seleccione una categoría", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IdCategoria = ((CategoriaItem)CmbCategoria.SelectedItem).IdCategoria;

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