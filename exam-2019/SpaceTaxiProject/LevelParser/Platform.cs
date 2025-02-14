using System;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Physics;

namespace SpaceTaxi {
    public class Platform {
        public (float x1, float x2, float y) PlatformExtent;
        private Player player;
        private EntityContainer<Entity> platformEntities;
        private char platformChar;
        private int inLevel;

        public Platform(char platformChar) {
            this.platformChar = platformChar;
            platformEntities = new EntityContainer<Entity>();
            player = Player.GetInstance();
        }
            
        /// <summary>
        /// Gets the extent of the whole platform
        /// </summary>
        /// <returns>floats according to the extent of the platform</returns>
        public (float x1, float x2, float y) GetPlatformExtent() {
            var left = -1.0f;
            var right = -1.0f;
            var top = -1.0f;
            foreach (Entity platformEntity in platformEntities) {
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

        /// <summary>
        /// Renders platform entities
        /// </summary>
        public void RenderPlatform() {
            platformEntities.Iterate(entity => entity.RenderEntity());
        }

        /// <summary>
        /// Adds an entity to the platform
        /// </summary>
        /// <param name="entity"> entity to be added</param>
        public void AddEntity(Entity entity) {
            platformEntities.AddStationaryEntity(entity);
        }

        /// <summary>
        /// Increments the level number
        /// </summary>
        /// <param name="levelNumber"> the Level Number</param>
        public void AddLevelNumber(int levelNumber) {
            inLevel = levelNumber;
        }

        /// <summary>
        /// Updates for collision with player object
        /// </summary>
        public void PlatformCollision() {
            // Logic for collision with platforms
            foreach (Entity entity in platformEntities) {
                var collsion =
                    CollisionDetection.Aabb(player.Entity.Shape.AsDynamicShape(), entity.Shape);
                if (collsion.Collision) {
                    player.onPlatform = true;
                    var speed = player.Entity.Shape.AsDynamicShape().Direction;
                    if (Math.Abs(speed.X) > 0.0025f || Math.Abs(speed.Y) > 0.0025f) {
                        player.KillPlayer();
                    } else if (player.pickedUpCustomer != null) {
                        if (player.pickedUpCustomer.DestinationPlatform == platformChar && player.pickedUpCustomer.DestinationLevel == inLevel) {
                            if (player.DeliveryOnTime) {
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.StatusEvent, this,"AWARD_POINTS",
                                        player.pickedUpCustomer.DropOffPoints.ToString(), ""));
                            } else {
                                SpaceTaxiBus.GetBus().RegisterEvent(
                                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                                        GameEventType.StatusEvent, this,"AWARD_POINTS",
                                        (player.pickedUpCustomer.DropOffPoints/2).ToString(), ""));
                            }
                            player.pickedUpCustomer = null;
                        }
                    }
                }
            }
        }

    }
}