using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.State;
using DIKUArcade.Timers;

namespace SpaceTaxi.States {
    public class GameRunning : IGameState {
        private static GameRunning instance = null;
        
        private Entity backGroundImage;
        
        private Player player;
        
        private Map map;
        
        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private int explosionLength = 500;
        
        private GameRunning() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            player = Player.GetInstance();
            
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(4);
            
            InitializeGameState();   
        }

        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }
        
        public static GameRunning NewInstance() {
            return GameRunning.instance = new GameRunning();
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }

        public void InitializeGameState() {       
            map = MapCreator.GetInstance().mapDictionary["short-n-sweet.txt"];
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
        }
        

        public void UpdateGameLogic() {
            player.Move();
            map.CollisionLogic();

        }
        
        public void RenderState() {
            backGroundImage.RenderEntity();
            map.PlatformContainer.Iterate(entity => entity.RenderEntity());
            map.MapContainer.Iterate(entity => entity.RenderEntity());
            player.RenderPlayer();
            map.explosions.RenderAnimations();
        }

        public void SetMap(string levelFileName) {
            StaticTimer.RestartTimer();
            map = MapCreator.GetInstance().mapDictionary[levelFileName];
            player.Velocity = new Vec2F(0,0);
            player.acceleration = new Vec2F(0,0);
            player.time = 0;
            player.Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
        }

        public void ResetPlayer() {
            player.Velocity = new Vec2F(0,0);
            player.acceleration = new Vec2F(0,0);
            player.time = 0;
            player.Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
            StaticTimer.RestartTimer();
        }
        

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