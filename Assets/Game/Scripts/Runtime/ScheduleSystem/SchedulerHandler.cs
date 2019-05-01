using System;
using System.Collections.Generic;
using Wokarol.Clocks;

namespace Wokarol.ScheduleSystem
{
    /// <summary>
    /// Handles delaying and repeating of actions
    /// </summary>
    public class ScheduleHandler
    {
        Random rng;
        Random RNG {
            get {
                if(rng == null) {
                    rng = new Random();
                }
                return rng;
            }
        }

        /// <summary>
        /// All currently tracked delayed actions
        /// </summary>
        private List<DelayedAction> delayedActions = new List<DelayedAction>();
        private HashSet<int> delayedActionIDs = new HashSet<int>();
        /// <summary>
        /// All currently tracked repeated actions
        /// </summary>
        private List<RepeatedAction> repeatedActions = new List<RepeatedAction>();
        private HashSet<int> repeatedActionIDs = new HashSet<int>();

        private HashSet<int> delayedActionsIDsToDelete = new HashSet<int>();
        private HashSet<int> repeatedActionsIDsToDelete = new HashSet<int>();

        public ScheduleHandler(IClock clock) {
            clock.OnTick += Tick;
        }

        // Per Tick
        void Tick(float delta) {

            // Loop through every delayed action
            for (int i = delayedActions.Count - 1; i >= 0; i--) {
                EvaluateDelayedAction(delta, i);
            }

            // Loop through every repeated action
            for (int i = repeatedActions.Count - 1; i >= 0; i--) {
                EvaluateRepeatedAction(delta, i);
            }

            delayedActionsIDsToDelete.Clear();
            repeatedActionsIDsToDelete.Clear();
        }

        private void EvaluateDelayedAction(float delta, int i) {

            var delayedAction = delayedActions[i];

            // Delete action if it exist in toDelete hash
            if (delayedActionsIDsToDelete.Contains(delayedAction.ID)) {
                RemoveDelayedAction(i);
                return;
            }

            delayedAction.Countdown -= delta;

            // Invoke action if countdown reached 0 or below
            if (delayedAction.Countdown <= 0) {
                delayedAction.Action.Invoke();
                RemoveDelayedAction(i);
            } else {
                delayedActions[i] = delayedAction;
            }
        }

        private void EvaluateRepeatedAction(float delta, int i) {

            var repeatedAction = repeatedActions[i];

            // Delete action if it exist in toDelete hash
            if (repeatedActionsIDsToDelete.Contains(repeatedAction.ID)) {
                RemoveRepeatedAction(i);
                return;
            }

            repeatedAction.Countdown -= delta;

            // Invoke action if countdown reached 0 or below
            if (repeatedAction.Countdown <= 0) {
                repeatedAction.Action.Invoke();
                repeatedAction.Countdown += repeatedAction.Interval;
                if (repeatedAction.Count != -1) repeatedAction.Count -= 1;
            }
            // Remove action if count reached 0 (it will never happen if action count with -1)
            if (repeatedAction.Count == 0) {
                RemoveRepeatedAction(i);
            } else {
                repeatedActions[i] = repeatedAction;
            }
        }

        // Creating new Actions
        /// <summary>
        /// Invokes Action with given delay
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="time">Delay in seconds</param>
        public ActionHandle Delay(Action action, float time) {
            int id = AddDelayedAction(action, time);
            return new ActionHandle(id, ActionType.Delayed, this);
        }

        /// <summary>
        /// Repeats action infinitely
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        public ActionHandle Repeat(Action action, float interval) {
            return Repeat(action, interval, -1);
        }

        /// <summary>
        /// Repeats action given number of times
        /// </summary>
        /// <param name="action">Action to repeat</param>
        /// <param name="interval">Interval in seconds</param>
        /// <param name="count">Amount of repeats</param>
        public ActionHandle Repeat(Action action, float interval, int count) {
            int id = AddRepeatedAction(action, interval, count);
            return new ActionHandle(id, ActionType.Repeat, this);
        }

        // Methods for Handles
        /// <summary>
        /// Deletes action
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        internal bool DeleteAction (ActionHandle handle) {

            // Check if action with given ID exist, and if so add it to toDelete hash
            switch (handle.Type) {
                case ActionType.Delayed:
                    if (!delayedActionIDs.Contains(handle.ID))
                        return false;
                    delayedActionsIDsToDelete.Add(handle.ID);
                    return true;

                case ActionType.Repeat:
                    if (!repeatedActionIDs.Contains(handle.ID))
                        return false;
                    repeatedActionsIDsToDelete.Add(handle.ID);
                    return true;

                default:
                    return false;
            }
        }

        // Private Utils
        /// <summary>
        /// Gets free id
        /// </summary>
        /// <param name="hashSet"></param>
        /// <returns></returns>
        private int GetFreeID(HashSet<int> hashSet) {
            int id = RNG.Next();
            int startID = id;

            while (hashSet.Contains(id)) {
                id += 1;
                if (id == startID) throw new OutOfMemoryException("Unable to find free id");
            }

            return id;
        }

        // Private Adding/Removing
        private int AddDelayedAction(Action action, float time) {
            int id = GetFreeID(delayedActionIDs);
            delayedActions.Add(new DelayedAction(id, action, time));
            delayedActionIDs.Add(id);
            return id;
        }
        private int AddRepeatedAction(Action action, float interval, int count) {
            int id = GetFreeID(repeatedActionIDs);
            repeatedActions.Add(new RepeatedAction(id, action, interval, count));
            repeatedActionIDs.Add(id);
            return id;
        }

        private bool RemoveDelayedAction(int i) {
            delayedActionIDs.Remove(delayedActions[i].ID);
            delayedActions.RemoveAt(i);
            return false;
        }
        private bool RemoveRepeatedAction(int i) {
            repeatedActionIDs.Remove(repeatedActions[i].ID);
            repeatedActions.RemoveAt(i);
            return false;
        }

        // Structs
        /// <summary>
        /// Data of action that is delayed
        /// </summary>
        private struct DelayedAction
        {
            public readonly Action Action;
            public readonly int ID;
            public float Countdown { get; set; }

            public DelayedAction(int id, Action action, float countdown) {
                ID = id;
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
            public readonly int ID;
            public float Countdown { get; set; }
            public float Interval { get; set; }
            public int Count { get; set; }

            public RepeatedAction(int id, Action action, float interval, int count) {
                ID = id;
                Action = action;
                Countdown = interval;
                Interval = interval;
                Count = count;
            }
        }

    } 

    public struct ActionHandle
    {
        internal readonly int ID;
        internal readonly ActionType Type;
        internal readonly ScheduleHandler Source;

        internal ActionHandle(int id, ActionType type, ScheduleHandler source) {
            ID = id;
            Type = type;
            Source = source;
        }

        public bool Delete() {
            return Source.DeleteAction(this);
        }
    }

    public enum ActionType : byte { Delayed, Repeat }
}