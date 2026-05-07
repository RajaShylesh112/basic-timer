using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTimer;

internal class TimeTracker
{
    public bool ShowHours { get; set; } = false;

    private DateTime TimeStart = DateTime.Now; // to show a rising time FROM a start point
    private DateTime TimeNow = DateTime.Now; // changes as timer runs
    private DateTime TimeEnd = DateTime.Now; // Default to no time (00:00)
    public TimeSpan TimeOnClock => CountingUpward ? TimeNow - TimeStart : TimeEnd - TimeNow;
    private bool _isRunning = false; // Don't auto-start
    public bool IsRunning => _isRunning;

    private bool _upward = true; // Start in count up mode by default
    public bool CountingUpward
    {
        get => _upward;
        set
        {
            _upward = value;
            if (_upward)
            {
                // used to be counting down (toward a future date)
                // keep the same time on the clock
                TimeSpan timeOnClock = TimeEnd - TimeNow;
                TimeStart = TimeNow - timeOnClock;
            }
            else
            {
                // used to be counting up (from a past date)
                TimeSpan timeOnClock = TimeNow - TimeStart;
                TimeEnd = TimeNow + timeOnClock;
            }
        }
    }

    public int Minutes => (int)TimeOnClock.TotalMinutes;
    public int Seconds => TimeOnClock.Seconds;
    public int Hours => (int)TimeOnClock.TotalHours;

    public void Set(int seconds)
    {
        if (CountingUpward)
            TimeStart = TimeNow.AddSeconds(-seconds);
        else
            TimeEnd = TimeNow.AddSeconds(seconds);
    }

    public void Set(int hours, int minutes, int seconds)
    {
        int totalSeconds = hours * 3600 + minutes * 60 + seconds;
        Set(totalSeconds);
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Start()
    {
        if (CountingUpward)
        {
            TimeStart = DateTime.Now - TimeOnClock;
            TimeNow = DateTime.Now;
        }
        else
        {
            TimeEnd = DateTime.Now + TimeOnClock;
            TimeNow = DateTime.Now;
        }
        _isRunning = true;
    }

    public void Pause()
    {
        if (_isRunning)
            Stop();
        else
            Start();
    }

    public void Reset()
    {
        _isRunning = false;
        TimeNow = DateTime.Now;
        TimeStart = TimeNow;
        TimeEnd = TimeNow;
    }

    public void Restart()
    {
        Reset();
        _isRunning = true;
    }

    public void UpdateTime()
    {
        if (_isRunning)
            TimeNow = DateTime.Now;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        if (TimeOnClock.TotalSeconds < 0)
            sb.Append($"-");
        
        if (ShowHours)
        {
            // Show hours:minutes:seconds format
            int hours = (int)Math.Abs(TimeOnClock.TotalHours);
            int minutes = Math.Abs(TimeOnClock.Minutes);
            int seconds = Math.Abs(TimeOnClock.Seconds);
            
            sb.Append($"{hours:00}:");
            sb.Append($"{minutes:00}:");
            sb.Append($"{seconds:00}");
        }
        else
        {
            // Show original minutes:seconds format
            int totalMinutes = (int)Math.Abs(TimeOnClock.TotalMinutes);
            int seconds = Math.Abs(TimeOnClock.Seconds);
            
            sb.Append($"{totalMinutes:00}:");
            sb.Append($"{seconds:00}");
        }
        
        return sb.ToString();
    }
}
