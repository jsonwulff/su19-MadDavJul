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

        private bool playerAlive;
        
        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private int explosionLength = 500;
        
        private GameRunning() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            player = new Player();
            
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
            map = MapCreator.GetInstance().mapDictionary["the-beach.txt"];
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
        }
        

        public void UpdateGameLogic() {
            player.Move();
            
            
            map.MapContainer.Iterate(entity => 
            {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
                if (!collsion.Collision) {
                    player.ManagePhysics();

                }
                if (collsion.Collision) {
                    AddExplosion(player.Entity.Shape.Position.X, player.Entity.Shape.Position.Y,
                        player.Entity.Shape.Extent.X, player.Entity.Shape.Extent.X);
                    player.acceleration = new Vec2F(0,0);
                    player.Velocity = new Vec2F(0,0);
                    
                    Console.Write("Colision detected");
                    if (collsion.CollisionDir == CollisionDirection.CollisionDirDown) {
                        Console.WriteLine(" from above");
                    } else if (collsion.CollisionDir == CollisionDirection.CollisionDirUp) {
                        Console.WriteLine(" from below");
                    } else {
                        Console.WriteLine("");
                    }
                }
            });
        }
        
        public void RenderState() {
            backGroundImage.RenderEntity();
            map.PlatformContainer.Iterate(entity => entity.RenderEntity());
            map.MapContainer.Iterate(entity => entity.RenderEntity());
            player.RenderPlayer();
            explosions.RenderAnimations();
        }

        public void SetMap(string levelFileName) {
            StaticTimer.RestartTimer();
            map = MapCreator.GetInstance().mapDictionary[levelFileName];
            foreach (var line in map.Platforms) {
                Console.WriteLine(line);
            }
            player.Velocity = new Vec2F(0,0);
            player.acceleration = new Vec2F(0,0);
            player.SetPosition(map.PlayerPosition.x, map.PlayerPosition.y);
        }
        
        public void AddExplosion(float posX, float posY,
            float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
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