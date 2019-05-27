using System;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;

namespace SpaceTaxi {
    public class Platform {
        public (float x1, float x2, float y) PlatformExtent;
        private Player player;
        private EntityContainer<Entity> PlatformEntities;
        private char PlatformChar;
        private int inLevel;

        public Platform(char platformChar) {
            PlatformChar = platformChar;
            PlatformEntities = new EntityContainer<Entity>();
            player = Player.GetInstance();
        }

        public (float x1, float x2, float y) platformExtent() {
            var left = -1.0f;
            var right = -1.0f;
            var top = -1.0f;
            foreach (Entity platformEntity in PlatformEntities) {
                if (left < 0.0 && right < 0.0) {
                    left = platformEntity.Shape.Position.X;
                    right = platformEntity.Shape.Position.X + 0.025f;
                    top = platformEntity.Shape.Position.Y + 0.025f;
                } else {
                    left = Math.Min(platformEntity.Shape.Position.X, left);
                    right = Math.Max(platformEntity.Shape.Position.X, right);

                }
            }
            return (left, right, top);
        }

        public void RenderPlatform() {
            PlatformEntities.Iterate(entity => entity.RenderEntity());
        }

        public void AddEntity(Entity entity) {
            PlatformEntities.AddStationaryEntity(entity);
        }

        public void AddLevelNumber(int levelNumber) {
            inLevel = levelNumber;
        }

        public void PlatformCollision() {
            // Logic for collision with platforms
            foreach (Entity entity in PlatformEntities) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
                if (collsion.Collision) {
                    player.onPlatform = true;
                    var speed = player.Entity.Shape.AsDynamicShape().Direction;
                    if (Math.Abs(speed.X) > 0.004f || Math.Abs(speed.Y) > 0.004f) {
                        player.KillPlayer();
                    } else if (player.pickedUpCustomer != null) {
                        if (player.pickedUpCustomer.destinationPlatform == PlatformChar && player.pickedUpCustomer.destinationLevel == inLevel) {
                            if (player.DeliveryOnTime) {
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.StatusEvent, this,"AWARD_POINTS",
                                        player.pickedUpCustomer.dropOffPoints.ToString(), ""));
                            } else {
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.StatusEvent, this,"AWARD_POINTS",
                                        (player.pickedUpCustomer.dropOffPoints/2).ToString(), ""));
                            }
                            player.pickedUpCustomer = null;
                        }
                    }
                }
            }
        }

    }
}