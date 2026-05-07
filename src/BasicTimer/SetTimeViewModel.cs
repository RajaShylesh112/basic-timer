using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTimer
{
    internal class SetTimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int TotalSeconds => Hours * 3600 + Minutes * 60 + Seconds;

        private int _hours;
        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                if (_hours < 0)
                    _hours = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hours)));
            }
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                while (_minutes >= 60)
                {
                    Hours += 1;
                    _minutes -= 60;
                }
                while (_minutes < 0 && Hours > 0)
                {
                    Hours -= 1;
                    _minutes += 60;
                }
                
                if (_minutes < 0)
                    _minutes = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Minutes)));
            }
        }

        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                while (_seconds >= 60)
                {
                    Minutes += 1;
                    _seconds -= 60;
                }
                while (_seconds < 0 && Minutes > 0)
                {
                    Minutes -= 1;
                    _seconds += 60;
                }

                if (_seconds < 0)
                    _seconds = 0;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seconds)));
            }
        }
    }
}
