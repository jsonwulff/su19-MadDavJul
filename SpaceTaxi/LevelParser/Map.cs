using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using SpaceTaxi.States;

namespace SpaceTaxi {
    public class Map {
        public EntityContainer<Entity> MapContainer;
        public EntityContainer<Entity> PlatformContainer;
        public string LevelName;
        public (float x, float y) PlayerPosition;
        public string[] CustomerData;
        public AnimationContainer explosions;
        private Player player;
        public char[] Platforms;
        private List<Image> explosionStrides;
        private string FileName;
        private int LevelNumber;
        

        private int explosionLength = 500;

        public Map(EntityContainer<Entity> mapContainer, string levelName, String fileName, int levelNumber,
            (float x, float y) playerPosition, string[] customerData, EntityContainer<Entity> platformContainer) {
            MapContainer = mapContainer;
            LevelName = levelName;
            PlayerPosition = playerPosition;
            CustomerData = customerData;
            PlatformContainer = platformContainer;
            player = Player.GetInstance();
            FileName = fileName;
            LevelNumber = levelNumber;
            
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(4);
        }
        /// <summary>
        /// CollisionLogic checks for collisions with MapContainer and PlatformContainer. Kills if obstacle
        /// collision, sets player.onPlatform false if platform collision.
        /// </summary>
        public void CollisionLogic() {
            if (player.onPlatform) {
                player.Entity.Shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            } else {
                player.ManagePhysics();
            }

            // Logic for collision with platforms
            foreach (Entity entity in PlatformContainer) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
                if (collsion.Collision) {
                    player.onPlatform = true;
                    if (player.Speed > 0.005) {
                        player.alive = false;
                        SpaceTaxiBus.GetBus().RegisterEvent(
                            GameEventFactory<object>.CreateGameEventForAllProcessors(
                                GameEventType.GameStateEvent, this, "CHANGE_STATE", "GAME_OVER", ""));
                    }
                    
                }
            }
            
            // Logic for collision with obstacles
            foreach (Entity entity in MapContainer) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
  
                if (collsion.Collision) {
                    AddExplosion(player.Entity.Shape.Position.X, player.Entity.Shape.Position.Y,
                        player.Entity.Shape.Extent.X, player.Entity.Shape.Extent.X);
                    player.acceleration = new Vec2F(0,0);
                    player.Velocity = new Vec2F(0,0);
                    SpaceTaxiBus.GetBus().RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                            GameEventType.GameStateEvent, this, "CHANGE_STATE", "GAME_OVER", ""));
                }
            }

            if (player.Entity.Shape.Position.Y > 1.0f) {
                if (LevelNumber == MapCreator.GetInstance().levelsInFolder.Length - 1) {
                    GameRunning.GetInstance().SetMap(MapCreator.GetInstance().levelsInFolder[0]);
                } else {
                    GameRunning.GetInstance()
                        .SetMap(MapCreator.GetInstance().levelsInFolder[LevelNumber + 1]);
                }
            }
        }
        
        /// <summary>
        /// Plays an animation on the given position.
        /// </summary>
        /// <param name="posX">Position x</param>
        /// <param name="posY">Position y</param>
        /// <param name="extentX">Extent x</param>
        /// <param name="extentY">Extent y</param>
        public void AddExplosion(float posX, float posY,
            float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
        }
    }
}