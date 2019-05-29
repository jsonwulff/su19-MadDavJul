using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using SpaceTaxi.Customers;
using SpaceTaxi.States;

namespace SpaceTaxi {
    public class Level {
        public EntityContainer<Entity> LevelMapContainer;
        public Dictionary<char, Platform> PlatformContainer;
        public string LevelName;
        public Vec2F PlayerPosition;
        public string[] CustomerData;
        private Player player;
        public char[] Platforms;
        private string FileName;
        private int LevelNumber;
        private List<Customer> customers;
        private CustomerCreator customerCreator;

        public Level(EntityContainer<Entity> levelMapContainer, string levelName, String fileName, int levelNumber,
            Vec2F playerPosition, string[] customerData, Dictionary<char, Platform> platformContainer, CustomerCreator customercreator) {
            LevelMapContainer = levelMapContainer;
            LevelName = levelName;
            PlayerPosition = playerPosition;
            CustomerData = customerData;
            PlatformContainer = platformContainer;
            player = Player.GetInstance();
            FileName = fileName;
            LevelNumber = levelNumber;
            customers = new List<Customer>();
            customerCreator = customercreator;
        }
        
        /// <summary>
        /// CollisionLogic checks for collisions with LevelMapContainer and PlatformContainer. Kills if obstacle
        /// collision, sets player.onPlatform false if platform collision.
        /// </summary>
        public void CollisionLogic() {
            
            // Logic for collision with platforms
            foreach (var platform in PlatformContainer) {
                platform.Value.PlatformCollision();
            }

            foreach (var customer in customers) {
                customer.PickUpCollision();
            }
            
            // Logic for collision with obstacles
            foreach (Entity entity in LevelMapContainer) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
                if (collsion.Collision) {
                    player.KillPlayer();
                }
            }
        }

        public void MoveToNextLevel() {
            if (player.Entity.Shape.Position.Y > 1.0f) {
                if (LevelNumber == LevelCreator.GetInstance().levelsInFolder.Length - 1) {
                    GameRunning.GetInstance().SetLevel(LevelCreator.GetInstance().levelsInFolder[0]);
                } else {
                    GameRunning.GetInstance()
                        .SetLevel(LevelCreator.GetInstance().levelsInFolder[LevelNumber + 1]);
                }
            }
        }

        public void RenderLevel() {
            LevelMapContainer.Iterate(entity => entity.RenderEntity());
            foreach (var elem in PlatformContainer) {
                elem.Value.RenderPlatform();
            }

            foreach (var customer in customers) {
                customer.RenderCustomer();
            }
        }

        public void GetCustomers() {
            customers = customerCreator.CreateCustomers(CustomerData, LevelNumber);
            foreach (var customer in customers) {
                var spawnPlatform = customer.SpawnPlatform;
                var platoformExtent = PlatformContainer[spawnPlatform].GetPlatformExtent();
                customer.SetPosition((platoformExtent.x1 + platoformExtent.x2) / 2, platoformExtent.y);
            }
        }
    }
}