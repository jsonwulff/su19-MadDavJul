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
        public Dictionary<char, Platform> PlatformContainer;
        public string LevelName;
        public (float x, float y) PlayerPosition;
        public string[] CustomerData;
        private Player player;
        public char[] Platforms;
        private string FileName;
        private int LevelNumber;
        

        private int explosionLength = 500;

        public Map(EntityContainer<Entity> mapContainer, string levelName, String fileName, int levelNumber,
            (float x, float y) playerPosition, string[] customerData, Dictionary<char, Platform> platformContainer) {
            MapContainer = mapContainer;
            LevelName = levelName;
            PlayerPosition = playerPosition;
            CustomerData = customerData;
            PlatformContainer = platformContainer;
            player = Player.GetInstance();
            FileName = fileName;
            LevelNumber = levelNumber;
        }
        
        /// <summary>
        /// CollisionLogic checks for collisions with MapContainer and PlatformContainer. Kills if obstacle
        /// collision, sets player.onPlatform false if platform collision.
        /// </summary>
        public void CollisionLogic() {
            
            foreach (var platform in PlatformContainer) {
                platform.Value.PlatformCollision();
            }
            
            // Logic for collision with obstacles
            foreach (Entity entity in MapContainer) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
  
                if (collsion.Collision) {
                    player.AddExplosion();
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

        public void RenderMap() {
            MapContainer.Iterate(entity => entity.RenderEntity());
            foreach (var elem in PlatformContainer) {
                elem.Value.RenderPlatform();
            }
        }
    }
}