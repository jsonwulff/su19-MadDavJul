using DIKUArcade.Math;

namespace SpaceTaxi {
    public class Physics {
        private Vec2F gravity = new Vec2F(0f,-0.00004f);

        /// <summary>
        /// Updates the force acted on the player
        /// </summary>
        public void ManagePhysics() {
            Vec2F force = Player.GetInstance().Acceleration;

            if (!Player.GetInstance().onPlatform) {
                force += gravity;
            }
            
            Player.GetInstance().UpdateVelocity(force);
        }
    }
}