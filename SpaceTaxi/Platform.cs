using System;
using DIKUArcade.Entities;

namespace SpaceTaxi {
    public class Platform {
        private EntityContainer<Entity> PlatformContainer;
        private char PlatformChar;

        public void RenderPlatform() {
            PlatformContainer.Iterate(entity => entity.RenderEntity());
        }

        public void PlatformCollision() {
            throw new NotImplementedException();
        }
    }
}