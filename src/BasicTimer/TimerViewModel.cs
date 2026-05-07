using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace BasicTimer
{
    internal class TimerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public readonly TimeTracker Timer = new();
        public string Text => Timer.ToString();
        
        // Session tracking
        public readonly SessionHistory SessionHistory = new();
        private DateTime? _sessionStartTime;
        private TimeSpan _sessionInitialTime;
        
        private string _editableText = string.Empty;
        public string EditableText 
        { 
            get => _editableText; 
            set 
            { 
                _editableText = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditableText)));
            } 
        }

        public bool CountDown 
        { 
            get => !Timer.CountingUpward; 
            set => Timer.CountingUpward = !value; 
        } 



        public bool ShowHours 
        { 
            get => Timer.ShowHours; 
            set 
            { 
                Timer.ShowHours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowHours)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            } 
        }

        private Brush _progressBackgroundBrush = new SolidColorBrush(Colors.White);
        public Brush ProgressBackgroundBrush 
        { 
            get => _progressBackgroundBrush; 
            set 
            { 
                _progressBackgroundBrush = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressBackgroundBrush))); 
            } 
        }

        private Brush _progressForegroundBrush = new SolidColorBrush(Colors.Blue);
        public Brush ProgressForegroundBrush 
        { 
            get => _progressForegroundBrush; 
            set 
            { 
                _progressForegroundBrush = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressForegroundBrush))); 
            } 
        }

        public void Tick()
        {
            bool wasRunning = Timer.IsRunning;
            var timeBefore = Timer.TimeOnClock;
            
            Timer.UpdateTime();
            
            bool isRunning = Timer.IsRunning;
            var timeAfter = Timer.TimeOnClock;

            // Track session start when timer starts
            if (!wasRunning && isRunning)
            {
                StartSession();
            }
            
            // Track session end when timer stops (any reason)
            if (wasRunning && !isRunning && !_appExiting)
            {
                EndSession();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressWidth)));

            // Removed beep functionality as requested
        }

        private bool _appExiting = false;
        public void OnAppExit()
        {
            _appExiting = true;
            
            // If there's an active session when app closes, end it and log it
            if (_sessionStartTime.HasValue && Timer.IsRunning)
            {
                var endTime = DateTime.Now;
                var duration = endTime - _sessionStartTime.Value;
                
                if (duration.TotalSeconds >= 1) // Only log sessions longer than 1 second
                {
                    var entry = new SessionEntry
                    {
                        StartTime = _sessionStartTime.Value,
                        EndTime = endTime,
                        Duration = duration,
                        Activity = string.IsNullOrEmpty(Title) ? "Timer Session" : Title,
                        WasCountDown = CountDown,
                        InitialTime = _sessionInitialTime
                    };
                    
                    SessionHistory.AddSession(entry);
                }
            }
        }        private void StartSession()
        {
            _sessionStartTime = DateTime.Now;
            _sessionInitialTime = Timer.TimeOnClock;
        }

        private void EndSession()
        {
            if (_sessionStartTime.HasValue)
            {
                var endTime = DateTime.Now;
                var duration = endTime - _sessionStartTime.Value;
                
                // Only log sessions longer than 5 seconds (reduced for testing)
                if (duration.TotalSeconds >= 5)
                {
                    var session = new SessionEntry
                    {
                        StartTime = _sessionStartTime.Value,
                        EndTime = endTime,
                        Duration = duration,
                        WasCountDown = CountDown,
                        InitialTime = _sessionInitialTime,
                        Activity = !string.IsNullOrEmpty(Title) ? Title : "Timer Session"
                    };

                    SessionHistory.AddSession(session);
                    
                    // Debug: Force property change notification
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionHistory)));
                }
                
                _sessionStartTime = null;
            }
        }

        public Version Version => Assembly.GetExecutingAssembly().GetName().Version!;
        public string VersionString => $"Basic Timer {Version.Major}.{Version.Minor}";

        private double _windowHeight = 70;
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowHeight)));
            }
        }

        private int _fontSize = 36; // Bigger font for bigger window
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
            }
        }

        private int _progressWidthSeconds = 10;
        public int ProgressWidthSeconds
        {
            get => _progressWidthSeconds;
            set
            {
                _progressWidthSeconds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressWidthMax)));
            }
        }

        private double _progressWidthMax;
        public double ProgressWidthMax
        {
            get => _progressWidthMax;
            set
            {
                _progressWidthMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressWidthMax)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressWidth)));
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public double ProgressFraction
        {
            get
            {
                double width = ProgressWidthSeconds;
                double sec = Timer.TimeOnClock.TotalSeconds;

                if (sec == 0)
                    return 0;

                return (sec < 0)
                    ? (width - Math.Abs(sec) % width) / width
                    : (sec % width) / width;
            }
        }
        public double ProgressWidth => ProgressFraction * ProgressWidthMax;

        public void SetColorPreset(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "blue":
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.LightBlue);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Blue);
                    break;
                case "green":
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.LightGreen);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Green);
                    break;
                case "red":
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.Pink);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Red);
                    break;
                case "purple":
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.Lavender);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Purple);
                    break;
                case "orange":
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.PeachPuff);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Orange);
                    break;
                default:
                    ProgressBackgroundBrush = new SolidColorBrush(Colors.White);
                    ProgressForegroundBrush = new SolidColorBrush(Colors.Blue);
                    break;
            }
        }


    }
}
