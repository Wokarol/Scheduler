using Wokarol.Clocks;

namespace Wokarol.ScheduleSystem
{
    /// <summary>
    /// Handles scheduling actions for static instance of ScheduleHandler with PlayerLoopClock
    /// </summary>
    public class Scheduler
    {
        // Static Handle
        static ScheduleHandler instance;
        static ScheduleHandler Instance {
            get {
                if (instance == null) instance = new ScheduleHandler(PlayerLoopClock.Instance);
                return instance;
            }
        }

        /// <summary>
        /// Invokes Action with given delay
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="time">Delay in seconds</param>
        public static void Delay(System.Action action, float time) {
            Instance.Delay(action, time);
        }

        /// <summary>
        /// Repeats action infinitely
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        public static void Repeat(System.Action action, float interval) {
            Instance.Repeat(action, interval);
        }

        /// <summary>
        /// Repeats action given number of times
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        /// <param name="count">Amount of repeats</param>
        public static void Repeat(System.Action action, float interval, int count) {
            Instance.Repeat(action, interval, count);
        }
    } 
}
