using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.Clocks;
using Wokarol.ScheduleSystem;

namespace Wokarol.Tests
{
    [TestFixture]
    public class SchedulerTests
    {
        private TestClock clock;
        private ScheduleHandler scheduler;

        [SetUp]
        public void Setup () {
            clock = new TestClock();
            scheduler = new ScheduleHandler(clock);
        }

        [Test]
        public void _01_Can_Add_Actions_And_Tick() {
            scheduler.Delay(() => { }, 1);
            scheduler.Repeat(() => { }, 1);
            clock.Tick(1);
        }

        [Test]
        public void _02_Delayed_Action_Is_Called_After_Time_Passed() {
            List<string> callHistory = new List<string>();
            scheduler.Delay(() => callHistory.Add("A"), 2);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A" );
            TickAndCheckHistory(callHistory, "A" );
            TickAndCheckHistory(callHistory, "A" );
        }

        [Test]
        public void _03_Multiple_Delayed_Action_Are_Called() {
            List<string> callHistory = new List<string>();
            scheduler.Delay(() => callHistory.Add("A"), 2);
            scheduler.Delay(() => callHistory.Add("B"), 3);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A", "B");
            TickAndCheckHistory(callHistory, "A", "B");
        }

        [Test]
        public void _04_Repeated_Action_Is_Called_After_Time_Passed_Repeatedly() {
            List<string> callHistory = new List<string>();
            scheduler.Repeat(() => callHistory.Add("A"), 2);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A", "A");
        }

        [Test]
        public void _05_Multiple_Repeated_Action_Is_Called_After_Time_Passed_Repeatedly() {
            List<string> callHistory = new List<string>();
            scheduler.Repeat(() => callHistory.Add("A"), 2);
            scheduler.Repeat(() => callHistory.Add("B"), 3);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A", "B");
            TickAndCheckHistory(callHistory, "A", "B", "A");
            TickAndCheckHistory(callHistory, "A", "B", "A");
        }

        [Test]
        public void _06_Delayed_And_Repeated_Actions_Are_Called () {
            List<string> callHistory = new List<string>();
            scheduler.Repeat(() => callHistory.Add("A"), 2);
            scheduler.Delay(() => callHistory.Add("B"), 3);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A", "B");
            TickAndCheckHistory(callHistory, "A", "B", "A");
            TickAndCheckHistory(callHistory, "A", "B", "A");
            TickAndCheckHistory(callHistory, "A", "B", "A", "A");
        }

        [Test]
        public void _07_Delayed_Action_Can_Be_Deleted() {
            List<string> callHistory = new List<string>();
            var handle = scheduler.Delay(() => callHistory.Add("A"), 2);

            bool deleteResult = handle.Delete();
            Assert.That(deleteResult, Is.EqualTo(true));

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
        }

        [Test]
        public void _08_Deleting_Executed_Delayed_Action_Returs_False() {
            List<string> callHistory = new List<string>();
            var handle = scheduler.Delay(() => callHistory.Add("A"), 2);

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");

            bool deleteResult = handle.Delete();
            Assert.That(deleteResult, Is.EqualTo(false));

            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A");
        }

        [Test]
        public void _09_Repeated_Action_Can_Be_Deleted() {
            List<string> callHistory = new List<string>();
            var handle = scheduler.Repeat(() => callHistory.Add("A"), 2);

            bool deleteResult = handle.Delete();
            Assert.That(deleteResult, Is.EqualTo(true));

            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory);
        }

        [Test]
        public void _10_Repeated_Action_Can_Be_Deleted_After_First_Call() {
            List<string> callHistory = new List<string>();
            var handle = scheduler.Repeat(() => callHistory.Add("A"), 2);


            TickAndCheckHistory(callHistory);
            TickAndCheckHistory(callHistory, "A");

            bool deleteResult = handle.Delete();
            Assert.That(deleteResult, Is.EqualTo(true));

            TickAndCheckHistory(callHistory, "A");
            TickAndCheckHistory(callHistory, "A");
        }

        private void TickAndCheckHistory(List<string> callHistory, params string[] expectedCalls) {
            clock.Tick(1);
            Assert.That(callHistory, Is.EqualTo(expectedCalls), $"Incorrect call history, expected [{string.Join(", ", expectedCalls)}]");
        }
    }

    class TestClock : IClock
    {
        public event Action<float> OnTick;
        public void Tick(float delta) {
            OnTick?.Invoke(delta);
        }
    }
}
