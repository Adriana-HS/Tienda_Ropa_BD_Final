using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class SelectPromocionDialog : Window
    {
        private readonly PromocionService _promocionService;

        public int? SelectedPromocionId { get; private set; }

        public SelectPromocionDialog()
        {
            try
            {
                InitializeComponent();
                _promocionService = new PromocionService();
                // Cargar promociones después de que el diálogo se muestre
                Loaded += async (s, e) => 
                {
                    try
                    {
                        await LoadPromocionesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cargar promociones: {ex.Message}", "Error",
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

        private async Task LoadPromocionesAsync()
        {
            try
            {
                var promociones = await _promocionService.ObtenerPromocionesActivasAsync();
                CboPromociones.ItemsSource = promociones;
                if (promociones.Count > 0)
                {
                    CboPromociones.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar promociones: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (CboPromociones.SelectedValue != null)
            {
                SelectedPromocionId = (int)CboPromociones.SelectedValue;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Por favor seleccione una promoción.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
