using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BasicTimer
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        public Brush ForegroundBrush => ForegroundGroupBox.Background;
        public Brush BackgroundBrush => MainGrid.Background;

        public ColorPickerWindow()
        {
            InitializeComponent();
        }

        private void ForegroundTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            string text = textBox?.Text ?? string.Empty;

            try
            {
                Brush fgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(text));
                ForegroundGroupBox.Background = fgBrush;
            }
            catch (FormatException) { }
        }

        private void BackgroundTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            string text = textBox?.Text ?? string.Empty;

            try
            {
                Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(text));
                MainGrid.Background = bgBrush;
            }
            catch (FormatException) { }
        }
        
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void ColorPickerWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_OK_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                Button_Cancel_Click(sender, e);
            }
        }
    }
}
