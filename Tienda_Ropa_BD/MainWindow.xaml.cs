using System.Windows;
using System.Windows.Controls;
using TiendaRopaPOS.Views;

namespace TiendaRopaPOS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ClientesView();
        }

        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new InventarioView();
        }

        private void BtnVentas_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new VentasView();
        }

        private void BtnHistorial_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new HistorialView();
        }

        private void BtnPromociones_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new PromocionesView();
        }
    }
}

