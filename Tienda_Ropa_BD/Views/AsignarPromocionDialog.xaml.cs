using System;
using System.Windows;
using System.Windows.Controls;

namespace TiendaRopaPOS.Views
{
    public partial class AsignarPromocionDialog : Window
    {
        public int IdPromocion { get; private set; }
        public decimal MontoMinCompra { get; private set; }
        public int? IdProducto { get; private set; }
        public int? IdCategoria { get; private set; }
        public int? IdSubcategoria { get; private set; }

        public AsignarPromocionDialog()
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

        private void BtnSeleccionarPromocion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectDialog = new SelectPromocionDialog
                {
                    Owner = this
                };

                if (selectDialog.ShowDialog() == true && selectDialog.SelectedPromocionId.HasValue)
                {
                    TxtIdPromocion.Text = selectDialog.SelectedPromocionId.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar promoción:\n\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RbProducto_Checked(object sender, RoutedEventArgs e)
        {
            // PROTECCIÓN: Si el control aún no se ha creado, no hacemos nada.
            if (TxtLabelId == null) return;
            TxtLabelId.Text = "ID Producto:";
        }

        private void RbCategoria_Checked(object sender, RoutedEventArgs e)
        {
            // PROTECCIÓN: Si el control aún no se ha creado, no hacemos nada.
            if (TxtLabelId == null) return;
            TxtLabelId.Text = "ID Categoría:";
        }

        private void RbSubcategoria_Checked(object sender, RoutedEventArgs e)
        {
            // PROTECCIÓN: Si el control aún no se ha creado, no hacemos nada.
            if (TxtLabelId == null) return;
            TxtLabelId.Text = "ID Subcategoría:";
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtIdPromocion == null || !int.TryParse(TxtIdPromocion.Text, out int idPromocion))
                {
                    MessageBox.Show("Por favor ingrese un ID de promoción válido", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TxtId == null || !int.TryParse(TxtId.Text, out int id))
                {
                    MessageBox.Show("Por favor ingrese un ID válido", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TxtMontoMin == null || !decimal.TryParse(TxtMontoMin.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal montoMin) || montoMin < 0)
                {
                    MessageBox.Show("Por favor ingrese un monto mínimo válido (≥ 0)", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar que se haya seleccionado una opción
                if (RbProducto == null || RbCategoria == null || RbSubcategoria == null)
                {
                    MessageBox.Show("Error: Los controles no están inicializados correctamente", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                IdPromocion = idPromocion;
                MontoMinCompra = montoMin;
                IdProducto = null;
                IdCategoria = null;
                IdSubcategoria = null;

                if (RbProducto.IsChecked == true)
                {
                    IdProducto = id;
                }
                else if (RbCategoria.IsChecked == true)
                {
                    IdCategoria = id;
                }
                else if (RbSubcategoria.IsChecked == true)
                {
                    IdSubcategoria = id;
                }
                else
                {
                    MessageBox.Show("Por favor seleccione una opción (Producto, Categoría o Subcategoría)", "Validación", 
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

