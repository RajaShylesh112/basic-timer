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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace BasicTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimerViewModel VM = new();
        private DispatcherTimer RedrawTimer = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = VM;

            RedrawTimer.Interval = TimeSpan.FromMilliseconds(10);
            RedrawTimer.Tick += RedrawTimer_Tick; ;
            RedrawTimer.Start();

            TaskbarItemInfo = new();
        }

        private void RedrawTimer_Tick(object sender, EventArgs e)
        {
            VM.Tick();
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = VM.ProgressFraction;
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e) => VM.ProgressWidthMax = MainCanvas.ActualWidth;
        private void MenuItem_Restart_Click(object sender, RoutedEventArgs e) => VM.Timer.Restart();
        private void MenuItem_Stop_Click(object sender, RoutedEventArgs e) => VM.Timer.Stop();
        private void MenuItem_Start_Click(object sender, RoutedEventArgs e) => VM.Timer.Start();
        private void MenuItem_Pause_Click(object sender, RoutedEventArgs e) => VM.Timer.Pause();
        private void MenuItem_Reset_Click(object sender, RoutedEventArgs e) => VM.Timer.Reset();
        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(VM.Text);
        private void MenuItem_ExitApp_Click(object sender, RoutedEventArgs e) => Close();
        private void MenuItem_CloseMenu_Click(object sender, RoutedEventArgs e) { }

        private void MenuItem_FontSize_Click(object sender, RoutedEventArgs e) =>
            VM.FontSize = int.Parse(((MenuItem)sender).Tag.ToString()!);

        private void MenuItem_ProgressUnitSize_Click(object sender, RoutedEventArgs e) =>
            VM.ProgressWidthSeconds = int.Parse(((MenuItem)sender).Tag.ToString()!);

        private void MenuItem_ToggleTitleBar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WindowStyle == WindowStyle.None)
                {
                    // To show title bar, we need to disable transparency first
                    AllowsTransparency = false;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResizeWithGrip;
                    VM.WindowHeight = ActualHeight + SystemParameters.WindowCaptionHeight * 2;
                }
                else
                {
                    // To hide title bar, enable transparency and set to None
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    AllowsTransparency = true;
                    VM.WindowHeight = ActualHeight - SystemParameters.WindowCaptionHeight * 2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling title bar: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_ToggleAlwaysOnTop_Click(object sender, RoutedEventArgs e) =>
            Topmost = ((MenuItem)sender).IsChecked;

        private void MenuItem_Version_Click(object sender, RoutedEventArgs e) =>
            System.Diagnostics.Process.Start("explorer", "https://timer.swharden.com");

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }



        private void MenuItem_ShowTitle_Click(object sender, RoutedEventArgs e)
        {
            var win = new SetTitleWindow(VM.Title);
            if (win.ShowDialog() == true)
            {
                VM.Title = win.NewTitle;
            }
        }

        private void MenuItem_ColorPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string colorName)
            {
                VM.SetColorPreset(colorName);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.P:
                case Key.Space:
                    VM.Timer.Pause();
                    return;
                case Key.R:
                    VM.Timer.Restart();
                    return;
                case Key.S:
                    VM.Timer.Reset();
                    return;
                case Key.T:
                    MenuItem_ShowTitle_Click(this, null!);
                    return;
            }
        }

        private void MenuItem_SetTime_Click(object sender, RoutedEventArgs e)
        {
            var win = new SetTimeWindow(VM.Timer.Minutes, VM.Timer.Seconds);
            if (win.ShowDialog() == true && win.TotalSeconds is not null)
            {
                VM.Timer.Set(win.TotalSeconds.Value);
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Start inline editing on double-click
            StartInlineEdit();
        }

        private void StartInlineEdit()
        {
            VM.EditableText = VM.Text;
            TimerLabel.Visibility = Visibility.Collapsed;
            EditTextBox.Visibility = Visibility.Visible;
            EditTextBox.SelectAll();
            EditTextBox.Focus();
        }

        private void EndInlineEdit(bool save = false)
        {
            if (save)
            {
                // Parse the edited text and set the timer
                if (TryParseTimeString(VM.EditableText, out int hours, out int minutes, out int seconds))
                {
                    VM.Timer.Set(hours, minutes, seconds);
                }
            }
            
            EditTextBox.Visibility = Visibility.Collapsed;
            TimerLabel.Visibility = Visibility.Visible;
        }

        private bool TryParseTimeString(string timeStr, out int hours, out int minutes, out int seconds)
        {
            hours = 0;
            minutes = 0;
            seconds = 0;

            if (string.IsNullOrWhiteSpace(timeStr))
                return false;

            // Remove negative sign if present for parsing
            bool isNegative = timeStr.StartsWith("-");
            if (isNegative)
                timeStr = timeStr.Substring(1);

            string[] parts = timeStr.Split(':');
            
            try
            {
                if (parts.Length == 2)
                {
                    // MM:SS format
                    minutes = int.Parse(parts[0]);
                    seconds = int.Parse(parts[1]);
                }
                else if (parts.Length == 3)
                {
                    // HH:MM:SS format
                    hours = int.Parse(parts[0]);
                    minutes = int.Parse(parts[1]);
                    seconds = int.Parse(parts[2]);
                }
                else
                {
                    return false;
                }

                // Apply negative if needed
                if (isNegative)
                {
                    hours = -hours;
                    minutes = -minutes;
                    seconds = -seconds;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    EndInlineEdit(true);
                    e.Handled = true;
                    break;
                case Key.Escape:
                    EndInlineEdit(false);
                    e.Handled = true;
                    break;
            }
        }

        private void EditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EndInlineEdit(true);
        }

        private void MenuItem_QuickPresets_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new QuickPresetsWindow();
                if (win.ShowDialog() == true && win.SelectedSeconds.HasValue)
                {
                    VM.Timer.Set(win.SelectedSeconds.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Quick Presets: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_SessionHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new SessionHistoryWindow(VM.SessionHistory);
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Session History: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
