using System;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;

namespace SpaceTaxi {
    public class Platform {
        private Player player;
        private EntityContainer<Entity> PlatformEntities;
        private char PlatformChar;

        public Platform(char platformChar) {
            PlatformChar = platformChar;
            PlatformEntities = new EntityContainer<Entity>();
            player = Player.GetInstance();
        }

        public void RenderPlatform() {
            PlatformEntities.Iterate(entity => entity.RenderEntity());
        }

        public void AddEntity(Entity entity) {
            PlatformEntities.AddStationaryEntity(entity);
        }

        public void PlatformCollision() {
            // Logic for collision with platforms
            foreach (Entity entity in PlatformEntities) {
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
        }

    }
}