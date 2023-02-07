using System;
using System.Timers;

namespace BackendAPI.Web.Core
{


    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimerHelper
    {
        private static System.Timers.Timer aTimer;

        public static void Start()
        {
            SetTimer();

            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            
            
          

        }
        public static void StopTimer()
        {
            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("stop timer");
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }
    }
}
