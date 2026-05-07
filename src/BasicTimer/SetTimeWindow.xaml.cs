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
    /// Interaction logic for SetTimeWindow.xaml
    /// </summary>
    public partial class SetTimeWindow : Window
    {
        public int? TotalSeconds = null;
        private readonly SetTimeViewModel VM;

        public SetTimeWindow(int minutes, int seconds)
        {
            InitializeComponent();
            
            // Convert total minutes to hours and minutes for display
            int totalMinutes = minutes;
            int displayHours = totalMinutes / 60;
            int displayMinutes = totalMinutes % 60;
            
            VM = new() { Hours = displayHours, Minutes = displayMinutes, Seconds = seconds };
            DataContext = VM;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            TotalSeconds = VM.TotalSeconds;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) 
        {
            DialogResult = false;
            Close();
        }
        private void Button_AddHour_Click(object sender, RoutedEventArgs e) => VM.Hours += 1;
        private void Button_SubtractHour_Click(object sender, RoutedEventArgs e) => VM.Hours -= 1;
        private void Button_AddMinute_Click(object sender, RoutedEventArgs e) => VM.Minutes += 1;
        private void Button_SubtractMinute_Click(object sender, RoutedEventArgs e) => VM.Minutes -= 1;
        private void Button_AddSecond_Click(object sender, RoutedEventArgs e) => VM.Seconds += 1;
        private void Button_SubtractSecond_Click(object sender, RoutedEventArgs e) => VM.Seconds -= 1;
    }
}
