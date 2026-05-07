using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace BasicTimer
{
    public partial class SessionHistoryWindow : Window
    {
        private readonly SessionHistoryViewModel _viewModel;

        public SessionHistoryWindow(SessionHistory sessionHistory)
        {
            InitializeComponent();
            _viewModel = new SessionHistoryViewModel(sessionHistory);
            DataContext = _viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all session history? This action cannot be undone.",
                "Clear History",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.ClearHistory();
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = $"BasicTimer_Sessions_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportToCsv(saveFileDialog.FileName);
                    MessageBox.Show($"Sessions exported successfully to:\n{saveFileDialog.FileName}", 
                        "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting sessions:\n{ex.Message}", 
                        "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Start Time,End Time,Duration,Type,Initial Time,Activity");

            foreach (var session in _viewModel.Sessions.OrderByDescending(s => s.StartTime))
            {
                csv.AppendLine($"\"{session.FormattedStartTime}\",\"{session.FormattedEndTime}\",\"{session.FormattedDuration}\",\"{session.TypeDescription}\",\"{session.InitialTime}\",\"{session.Activity}\"");
            }

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }
    }

    public class SessionHistoryViewModel
    {
        private readonly SessionHistory _sessionHistory;

        public SessionHistoryViewModel(SessionHistory sessionHistory)
        {
            _sessionHistory = sessionHistory;
        }

        public System.Collections.Generic.List<SessionEntry> Sessions => 
            _sessionHistory.Sessions.OrderByDescending(s => s.StartTime).ToList();

        public string TotalTimeToday => FormatTimeSpan(_sessionHistory.GetTotalTimeToday());
        public string TotalTimeThisWeek => FormatTimeSpan(_sessionHistory.GetTotalTimeThisWeek());
        public int SessionCountToday => _sessionHistory.GetSessionCountToday();

        public void ClearHistory()
        {
            _sessionHistory.ClearHistory();
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            else
                return $"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}";
        }
    }
}
