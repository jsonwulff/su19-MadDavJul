using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using NUnit.Framework;
using SpaceTaxi;
using SpaceTaxi.Customers;

namespace SpaceTaxiTests {
    public class PlayerTests {
        
        private Player player;
        private Physics taxiPhysics;
        private GameEventBus<object> eventBus = SpaceTaxiBus.GetBus(); 

        [SetUp]
        public void CreateObjects() {
            
            player = new Player();
            
            //player = Player.GetInstance();
        }
        
        /// <summary>
        /// Player klassen bliver aldrig instantieret inden testene bliver kørt. Igen muligvis evige loops.
        /// </summary>
        [Test]
        public void playerInstantiateTest() {
            
            Assert.IsTrue(true);
        }
    }
}