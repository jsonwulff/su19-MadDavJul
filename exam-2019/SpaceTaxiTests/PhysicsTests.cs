using System;
using System.Collections.Generic;
using DIKUArcade.EventBus;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.Customers;
using SpaceTaxi.States;

namespace SpaceTaxiTests {
    public class PhysicsTests {
        private Player player;
        private GameEventBus<object> eventBus;
        private Physics taxiPhysics;
        
        [SetUp]
        public void CreateObjects() {
            player = Player.GetInstance();
            eventBus = new GameEventBus<object>();
            
            eventBus = SpaceTaxiBus.GetBus();
            
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.ControlEvent,
                GameEventType.GameStateEvent,
                GameEventType.InputEvent,
                GameEventType.PlayerEvent,
                GameEventType.WindowEvent
            });
            taxiPhysics = new Physics();
        }
        /// <summary>
        /// Tests som er tydeligt sande aborter på grund af mulige evige loops(?) kan ikke tænke på andre muligheder
        /// </summary>
        [Test]
        public void istrue() {
            taxiPhysics.ManagePhysics();
            Assert.IsTrue(true);
        }
    }
}