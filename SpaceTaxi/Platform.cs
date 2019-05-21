using System;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace SpaceTaxi {
    public class Platform {
        private EntityContainer<Entity> PlatformEntities;
        private char PlatformChar;

        public Platform(char platformChar) {
            PlatformChar = platformChar;
            PlatformEntities = new EntityContainer<Entity>();
        }

        public void RenderPlatform() {
            PlatformEntities.Iterate(entity => entity.RenderEntity());
        }

        public void AddEntity(Entity entity) {
            PlatformEntities.AddStationaryEntity(entity);
        }

        public void PlatformCollision() {
            throw new NotImplementedException();
        }

    }
}