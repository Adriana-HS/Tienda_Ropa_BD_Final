using System.Windows;
using System.Windows.Input;

namespace TiendaRopaPOS.Views
{
    public partial class InputDialog : Window
    {
        public string Result => TxtInput.Text;

        public InputDialog(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            TxtPrompt.Text = prompt;
            TxtInput.Text = defaultValue;
            TxtInput.Focus();
            TxtInput.SelectAll();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnAceptar_Click(sender, e);
            }
        }
    }
}

