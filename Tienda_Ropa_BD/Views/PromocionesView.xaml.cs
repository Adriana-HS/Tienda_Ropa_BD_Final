using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TiendaRopaPOS.Services;

namespace TiendaRopaPOS.Views
{
    public partial class PromocionesView : UserControl
    {
        private readonly PromocionService _promocionService;

        public PromocionesView()
        {
            InitializeComponent();
            _promocionService = new PromocionService();

            Loaded += async (s, e) =>
            {
                await LoadPromocionesAsync();
            };
        }

        private sealed class PromocionRow
        {
            public int IdPromocion { get; init; }
            public string Nombre { get; init; } = string.Empty;
            public string Estado { get; init; } = string.Empty;
        }

        private async Task LoadPromocionesAsync()
        {
            try
            {
                var promociones = await _promocionService.ObtenerPromocionesActivasAsync();
                var rows = new List<PromocionRow>();

                foreach (var promo in promociones)
                {
                    rows.Add(new PromocionRow
                    {
                        IdPromocion = promo.Key,
                        Nombre = promo.Value,
                        Estado = "Activa"
                    });
                }

                DgPromociones.ItemsSource = rows;
                TxtPromocionesInfo.Text = rows.Count > 0
                    ? $"Promociones activas ({rows.Count})"
                    : "No hay promociones activas";
            }
            catch (Exception ex)
            {
                TxtPromocionesInfo.Text = "Error al cargar promociones";
                DgPromociones.ItemsSource = null;
                MessageBox.Show($"Error al cargar promociones: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCrearPromocion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PromocionDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Llamar al servicio (async)
                    _ = CrearPromocionAsync(dialog);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async System.Threading.Tasks.Task CrearPromocionAsync(PromocionDialog dialog)
        {
            try
            {
                var idPromocion = await _promocionService.CrearPromocionAsync(
                    dialog.Nombre,
                    dialog.Descripcion,
                    dialog.Porcentaje,
                    dialog.Monto,
                    dialog.FechaInicio,
                    dialog.FechaFin);

                MessageBox.Show($"Promoción creada exitosamente (ID: {idPromocion})", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadPromocionesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAsignarPromocion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AsignarPromocionDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _ = AsignarPromocionAsync(dialog);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async System.Threading.Tasks.Task AsignarPromocionAsync(AsignarPromocionDialog dialog)
        {
            try
            {
                await _promocionService.AsignarPromocionAsync(
                    dialog.IdPromocion,
                    dialog.MontoMinCompra,
                    dialog.IdProducto,
                    dialog.IdCategoria,
                    dialog.IdSubcategoria);

                MessageBox.Show("Promoción asignada exitosamente", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadPromocionesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al asignar promoción: {ex.Message}\n\nDetalles: {ex.InnerException?.Message ?? "Sin detalles adicionales"}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDesactivarPromocion_Click(object sender, RoutedEventArgs e)
        {
            var selectDialog = new SelectPromocionDialog();
            if (selectDialog.ShowDialog() == true && selectDialog.SelectedPromocionId.HasValue)
            {
                int idPromocion = selectDialog.SelectedPromocionId.Value;

                var confirmacion = MessageBox.Show(
                    $"¿Está seguro de desactivar la promoción seleccionada?",
                    "Confirmar Desactivación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _promocionService.DesactivarPromocionAsync(idPromocion);
                        MessageBox.Show("Promoción desactivada exitosamente", "Éxito",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        await LoadPromocionesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al desactivar promoción: {ex.Message}\n\nDetalles: {ex.ToString()}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task DesactivarPromocionAsync(int idPromocion)
        {
            try
            {
                await _promocionService.DesactivarPromocionAsync(idPromocion);
                MessageBox.Show("Promoción desactivada exitosamente", "Éxito", 
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

