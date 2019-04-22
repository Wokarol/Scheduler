using System;
using System.Collections.Generic;
using Wokarol.Clocks;

namespace Wokarol.ScheduleSystem
{
    public class ScheduleHandler
    {
        /// <summary>
        /// All currently tracked delayed actions
        /// </summary>
        private List<DelayedAction> delayedActions = new List<DelayedAction>();
        /// <summary>
        /// All currently tracked repeated actions
        /// </summary>
        private List<RepeatedAction> repeatedActions = new List<RepeatedAction>();

        public ScheduleHandler(IClock clock) {
            clock.OnTick += Tick;
        }

        public void Tick(float delta) {
            // Loop through every delayed action
            for (int i = delayedActions.Count - 1; i >= 0; i--) {
                var delayedAction = delayedActions[i];
                delayedAction.Countdown -= delta;

                // Invoke action if countdown reached 0 or below
                if (delayedAction.Countdown <= 0) {
                    delayedAction.Action.Invoke();
                    delayedActions.RemoveAt(i);
                } else {
                    delayedActions[i] = delayedAction;
                }
            }

            // Loop through every repeated action
            for (int i = repeatedActions.Count - 1; i >= 0; i--) {
                var repeatedAction = repeatedActions[i];
                repeatedAction.Countdown -= delta;

                // Invoke action if countdown reached 0 or below
                if (repeatedAction.Countdown <= 0) {
                    repeatedAction.Action.Invoke();
                    repeatedAction.Countdown += repeatedAction.Interval;
                    if (repeatedAction.Count != -1) repeatedAction.Count -= 1;
                }
                // Remove action if count reached 0 (it will never happen if action count with -1)
                if (repeatedAction.Count == 0) {
                    repeatedActions.RemoveAt(i);
                } else {
                    repeatedActions[i] = repeatedAction;
                }
            }
        }

        /// <summary>
        /// Invokes Action with given delay
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="time">Delay in seconds</param>
        public void Delay(Action action, float time) {
            delayedActions.Add(new DelayedAction(action, time));
        }

        /// <summary>
        /// Repeats action infinitely
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        public void Repeat(Action action, float interval) {
            repeatedActions.Add(new RepeatedAction(action, interval, -1));
        }

        /// <summary>
        /// Repeats action given number of times
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        /// <param name="count">Amount of repeats</param>
        public void Repeat(Action action, float interval, int count) {
            if (count <= 0) throw new System.ArgumentOutOfRangeException($"{nameof(count)} need to be positive");
            repeatedActions.Add(new RepeatedAction(action, interval, count));
        }

        /// <summary>
        /// Data of action that is delayed
        /// </summary>
        private struct DelayedAction
        {
            public readonly Action Action;
            public float Countdown { get; set; }

            public DelayedAction(Action action, float countdown) {
                Action = action;
                Countdown = countdown;
            }
        }

        /// <summary>
        /// Data of action that is repeated
        /// </summary>
        private struct RepeatedAction
        {
            public readonly Action Action;
            public float Countdown { get; set; }
            public float Interval { get; set; }
            public int Count { get; set; }

            public RepeatedAction(Action action, float interval, int count) {
                Action = action;
                Countdown = interval;
                Interval = interval;
                Count = count;
            }
        }
    } 
}