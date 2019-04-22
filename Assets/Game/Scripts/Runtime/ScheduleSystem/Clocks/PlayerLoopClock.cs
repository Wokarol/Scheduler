using System;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;

namespace Wokarol.Clocks
{
    public class PlayerLoopClock : IClock
    {
        public static PlayerLoopClock Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void AppStart() {
            Instance = new PlayerLoopClock();
        }

        public event Action<float> OnTick;

        PlayerLoopClock() {
            var loop = PlayerLoop.GetDefaultPlayerLoop();
            var clockUpdate = new PlayerLoopSystem() {
                updateDelegate = Tick,
                type = typeof(PlayerLoopClock)
            };
            PlayerLoopUtils.AddToLoop<Update>(ref loop, clockUpdate);
            PlayerLoop.SetPlayerLoop(loop);
        }

        /// <summary>
        /// Called by PlayerLoopSystem before each update
        /// </summary>
        void Tick() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            OnTick?.Invoke(Time.deltaTime);
        }
    } 
}
