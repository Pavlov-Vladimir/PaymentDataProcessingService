using System;
using System.Threading.Tasks;

namespace PDPS.Core
{
    public class TimeWatcher
    {
        private bool _enableRaisingEvents = false;

        public DateTime EventTime { get; set; }
        public bool EnableRaisingEvents
        {
            get => _enableRaisingEvents;
            set 
            {
                _enableRaisingEvents = value;
                if (_enableRaisingEvents)
                {
                    _ = Watch();
                }
            }
        }

        public TimeWatcher(int hours = 23, int minutes = 59, int seconds = 59)
        {
            EventTime = new DateTime(1, 1, 1, hours, minutes, seconds);
        }

        public event Action<object> OnTime;

        public async Task Watch()
        {
            DateTime time;
            while (true)
            {
                if (!EnableRaisingEvents)
                    return;

                await Task.Delay(1000);
                time = DateTime.Now;
                if (time.Hour == EventTime.Hour &&
                    time.Minute == EventTime.Minute &&
                    time.Second == EventTime.Second)
                {
                    OnTime?.Invoke(this);
                }             
            }
        }
    }
}
