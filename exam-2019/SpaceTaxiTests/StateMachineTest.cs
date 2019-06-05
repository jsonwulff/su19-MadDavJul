using System.Collections.Generic;
using DIKUArcade.EventBus;
using DIKUArcade.Timers;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.States;

namespace SpaceTaxiTests {
    [TestFixture]
    public class StateMachineTest {
        private StateMachine stateMachine;
        private GameEventBus<object> eventBus;
        private TimedEventContainer timedEvents;
        
        [SetUp]
        public void InitiateStateMachine() {
            DIKUArcade.Window.CreateOpenGLContext();

            eventBus = SpaceTaxiBus.GetBus();
            
            eventBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.InputEvent,
                GameEventType.WindowEvent,
                GameEventType.PlayerEvent, 
                GameEventType.GameStateEvent,
                GameEventType.StatusEvent,
                GameEventType.TimedEvent
            });
            stateMachine = new StateMachine(); 
            timedEvents = TimedEvents.getTimedEvents();
            timedEvents.AttachEventBus(eventBus);
            eventBus.Subscribe(GameEventType.GameStateEvent, stateMachine);
        }
        
        [Test]
        public void TestInitialState() {
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }
        [Test]
        public void TestEventGamePaused() {
            eventBus.RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "GAME_PAUSED", ""));
            eventBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GamePaused>());
        } 
        
        [Test]
        public void TestEventGameRunning() {
            eventBus.RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "GAME_RUNNING", ""));
            eventBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GameRunning>());
        }
        
        [Test]
        public void TestEventMainMenu() {
            eventBus.RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "MAIN_MENU", ""));
            eventBus.ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }
    }
}