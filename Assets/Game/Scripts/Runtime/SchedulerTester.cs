using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.ScheduleSystem;

public class SchedulerTester : MonoBehaviour
{
    [SerializeField] BubbleOutput output = default;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            output.AddBubble("A");
            Scheduler.Delay(() => output.AddBubble("B"), 2f);
            Scheduler.Delay(() => output.AddBubble("C"), 3f);
            Scheduler.Repeat(() => output.AddBubble("D"), 1.5f);
            Scheduler.Repeat(() => output.AddBubble("E"), 1.5f, 2);
        }
    }
}
