using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
{
    public class TimeMeasureService
    {
        private class TimerData 
        {
            public TimeSpan Total { get; set; }
            public DateTime? Last { get; set; }
        }

        private readonly ILogger m_Logger;
        private readonly Dictionary<string, TimerData> m_Timers;
        private readonly string m_Format;

        public TimeMeasureService(ILogger logger, string format = "c")
        {
            m_Logger = logger;
            m_Format = format;
            m_Timers = new Dictionary<string, TimerData>();
        }

        public void Start(string name)
        {
            m_Timers.Add(name, new TimerData() 
            {
                Total = TimeSpan.Zero, 
                Last = DateTime.Now 
            });
        }

        public void Pause(string name) 
        {
            var timer = m_Timers[name];
            timer.Total = timer.Total + (DateTime.Now - timer.Last.Value);
            timer.Last = null;
        }

        public void Resume(string name)
        {
            var timer = m_Timers[name];
            timer.Last = DateTime.Now;
        }

        public void Stop(string name) 
        {
            var timer = m_Timers[name];

            if (timer.Last.HasValue)
            {
                timer.Total = timer.Total + (DateTime.Now - timer.Last.Value);
            }
            
            m_Timers.Remove(name);

            m_Logger.LogInformation($"Total {name} time: {timer.Total.ToString(m_Format)}");
        }
    }
}
