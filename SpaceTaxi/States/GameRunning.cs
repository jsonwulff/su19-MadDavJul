using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using DIKUArcade.Timers;
using SpaceTaxi.Customers;

namespace SpaceTaxi.States {
    public class GameRunning : IGameState {
        private static GameRunning instance = null;
        private Entity backGroundImage;
        private Player player;
        private Score score;
        private Customer customer;
        private Map map;
        
        // Added for customerCreator
        private CustomerCreator customerCreator;
        private List<Customer> customerList;
        
        
        private GameRunning() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            player = Player.GetInstance();
            score = new Score();
            customer = new Customer();
            
            // Added for customerCreator
            customerCreator = new CustomerCreator();

            InitializeGameState();   
        }
        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns>Instance of GameRunning</returns>
        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }
        
        public static GameRunning NewInstance() {
            return GameRunning.instance = new GameRunning();
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// Initialize the state of GameRunning
        /// </summary>
        public void InitializeGameState() {       

        }
        
        /// <summary>
        /// Continous update all logic
        /// </summary>
        public void UpdateGameLogic() {
            player.Move();
            map.CollisionLogic();
        }
        
        /// <summary>
        /// Render the objects in the scene
        /// </summary>
        public void RenderState() {
            backGroundImage.RenderEntity();
            map.RenderMap();
            player.RenderPlayer();
            score.RenderScore();
            player.explosion.RenderAnimations();
            customer.RenderCustomer();
            
        }

        /// <summary>
        /// Set the map of the current level
        /// </summary>
        /// <param name="levelFileName">Filename of the map</param>
        public void SetMap(string levelFileName) {
            map = MapCreator.GetInstance().mapDictionary[levelFileName];
            player.ResetPlayer();
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
            customerList = customerCreator.CreateCustomers(map.CustomerData);
        }
        
        /// <summary>
        /// Keypress handling
        /// </summary>
        /// <param name="key">The key pressed</param>
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_UP":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this,"BOOSTER_UPWARDS","",""));
                break;
            case "KEY_RIGHT":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_RIGHT", "", ""));
                break;
            case "KEY_LEFT":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "BOOSTER_TO_LEFT", "", ""));
                break;
            case "KEY_ESCAPE":
                StaticTimer.PauseTimer();
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE", "GAME_PAUSED", ""));
                break;
            
            }
        }
        /// <summary>
        /// Key release handling
        /// </summary>
        /// <param name="key">The key pressed</param>
        public void KeyRelease(string key) {
            switch (key) {
            case "KEY_UP":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "STOP_ACCELERATE_UP", "", ""));
                break;
            case "KEY_RIGHT":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "STOP_ACCELERATE_RIGHT", "", ""));
                break;
            case "KEY_LEFT":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "STOP_ACCELERATE_RIGHT", "", ""));
                break;
            }
        }
        
        public void HandleKeyEvent(string keyValue, string keyAction) {
            switch (keyAction) {
            case "KEY_PRESS":
                KeyPress(keyValue);
                break;
            case "KEY_RELEASE":
                KeyRelease(keyValue);
                break;
            }
        }
    }
 
}