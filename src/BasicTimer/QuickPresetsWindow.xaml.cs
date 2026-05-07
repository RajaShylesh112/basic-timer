using System;
using System.Windows;
using System.Windows.Controls;

namespace BasicTimer
{
    public partial class QuickPresetsWindow : Window
    {
        public int? SelectedSeconds { get; private set; }

        public QuickPresetsWindow()
        {
            InitializeComponent();
        }

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string secondsStr)
            {
                if (int.TryParse(secondsStr, out int seconds))
                {
                    SelectedSeconds = seconds;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
