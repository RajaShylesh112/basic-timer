using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BasicTimer
{
    public class SessionEntry
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Activity { get; set; } = "Timer Session";
        public bool WasCountDown { get; set; }
        public TimeSpan InitialTime { get; set; }

        public string FormattedDuration => FormatTimeSpan(Duration);
        public string FormattedStartTime => StartTime.ToString("yyyy-MM-dd HH:mm:ss");
        public string FormattedEndTime => EndTime.ToString("yyyy-MM-dd HH:mm:ss");
        public string TypeDescription => WasCountDown ? "Countdown" : "Count Up";

        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            else
                return $"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}";
        }
    }

    public class SessionHistory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly List<SessionEntry> _sessions = new();
        public List<SessionEntry> Sessions => new(_sessions);

        private static readonly string SessionFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BasicTimer",
            "sessions.csv"
        );

        public SessionHistory()
        {
            LoadSessions();
        }

        public void AddSession(SessionEntry session)
        {
            _sessions.Add(session);
            SaveSessions();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sessions)));
        }

        public void ClearHistory()
        {
            _sessions.Clear();
            SaveSessions();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sessions)));
        }

        private void LoadSessions()
        {
            try
            {
                if (File.Exists(SessionFilePath))
                {
                    var lines = File.ReadAllLines(SessionFilePath);
                    for (int i = 1; i < lines.Length; i++) // Skip header
                    {
                        var parts = lines[i].Split(',');
                        if (parts.Length >= 6)
                        {
                            var session = new SessionEntry
                            {
                                StartTime = DateTime.Parse(parts[0]),
                                EndTime = DateTime.Parse(parts[1]),
                                Duration = TimeSpan.Parse(parts[2]),
                                Activity = parts[3],
                                WasCountDown = bool.Parse(parts[4]),
                                InitialTime = TimeSpan.Parse(parts[5])
                            };
                            _sessions.Add(session);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If loading fails, just start with empty list
            }
        }

        private void SaveSessions()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SessionFilePath)!);
                var lines = new List<string>
                {
                    "StartTime,EndTime,Duration,Activity,WasCountDown,InitialTime"
                };
                
                foreach (var session in _sessions)
                {
                    lines.Add($"{session.StartTime:yyyy-MM-dd HH:mm:ss},{session.EndTime:yyyy-MM-dd HH:mm:ss},{session.Duration},{session.Activity},{session.WasCountDown},{session.InitialTime}");
                }
                
                File.WriteAllLines(SessionFilePath, lines);
            }
            catch (Exception)
            {
                // If saving fails, silently continue
            }
        }

        public TimeSpan GetTotalTimeToday()
        {
            var today = DateTime.Today;
            return TimeSpan.FromTicks(_sessions
                .Where(s => s.StartTime.Date == today)
                .Sum(s => s.Duration.Ticks));
        }

        public TimeSpan GetTotalTimeThisWeek()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return TimeSpan.FromTicks(_sessions
                .Where(s => s.StartTime.Date >= startOfWeek)
                .Sum(s => s.Duration.Ticks));
        }

        public int GetSessionCountToday()
        {
            var today = DateTime.Today;
            return _sessions.Count(s => s.StartTime.Date == today);
        }
    }
}
