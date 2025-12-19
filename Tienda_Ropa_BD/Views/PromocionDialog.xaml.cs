using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace TiendaRopaPOS.Views
{
    public partial class PromocionDialog : Window
    {
        public string Nombre => TxtNombre.Text.Trim();
        public string? Descripcion => string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim();
        public decimal? Porcentaje { get; private set; }
        public decimal? Monto { get; private set; }
        public DateTime FechaInicio => DpFechaInicio.SelectedDate ?? DateTime.Now;
        public DateTime FechaFin => DpFechaFin.SelectedDate ?? DateTime.Now;

        public PromocionDialog()
        {
            try
            {
                InitializeComponent();
                if (DpFechaInicio != null)
                    DpFechaInicio.SelectedDate = DateTime.Now;
                if (DpFechaFin != null)
                    DpFechaFin.SelectedDate = DateTime.Now.AddDays(30);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el diálogo:\n\n{ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw; // Re-lanzar para que el diálogo no se muestre si hay error crítico
            }
        }

        private void RbPorcentaje_Checked(object sender, RoutedEventArgs e)
        {
            // PROTECCIÓN: Si los controles aún no se han creado, no hacemos nada.
            if (TxtLabelValor == null || TxtValor == null) return;

            TxtLabelValor.Text = "Valor Porcentaje (%):";
            TxtValor.Text = "";
        }

        private void RbMonto_Checked(object sender, RoutedEventArgs e)
        {
            // PROTECCIÓN: Si los controles aún no se han creado, no hacemos nada.
            if (TxtLabelValor == null || TxtValor == null) return;

            TxtLabelValor.Text = "Valor Monto ($):";
            TxtValor.Text = "";
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    MessageBox.Show("Por favor ingrese el nombre de la promoción", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TxtValor == null || !decimal.TryParse(TxtValor.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) || valor <= 0)
                {
                    MessageBox.Show("Por favor ingrese un valor válido mayor a cero", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DpFechaInicio == null || DpFechaFin == null || DpFechaInicio.SelectedDate == null || DpFechaFin.SelectedDate == null)
                {
                    MessageBox.Show("Por favor seleccione ambas fechas", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (FechaFin < FechaInicio)
                {
                    MessageBox.Show("La fecha fin debe ser posterior a la fecha inicio", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (RbPorcentaje == null || RbMonto == null)
                {
                    MessageBox.Show("Error: Los controles no están inicializados correctamente", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (RbPorcentaje.IsChecked == true)
                {
                    // Validar que el porcentaje esté entre 0 y 1 (0% a 100%)
                    if (valor > 1)
                    {
                        MessageBox.Show("El porcentaje debe estar entre 0 y 1 (ej: 0.15 para 15%)", "Validación", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    Porcentaje = valor;
                    Monto = null;
                }
                else if (RbMonto.IsChecked == true)
                {
                    Monto = valor;
                    Porcentaje = null;
                }
                else
                {
                    MessageBox.Show("Por favor seleccione un tipo de descuento (Porcentaje o Monto)", "Validación", 
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