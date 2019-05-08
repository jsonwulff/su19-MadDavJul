using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;

namespace SpaceTaxi {
    public class Player : IGameEventProcessor<object> {
        private readonly Image taxiBoosterOffImageLeft;
        private readonly Image taxiBoosterOffImageRight;
        private readonly DynamicShape shape;
        private Orientation taxiOrientation;

        
        private Vec2F acceleration;
        public Vec2F Velocity;
        double time = 0;
        public double Speed;

        public Player() {
            shape = new DynamicShape(new Vec2F(), new Vec2F());
            taxiBoosterOffImageLeft =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
            taxiBoosterOffImageRight =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));

            Entity = new Entity(shape, taxiBoosterOffImageLeft);
            Velocity = new Vec2F(0,0);
            acceleration = new Vec2F(0,0);
            
        }

        public Entity Entity { get; }

        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        public void SetExtent(float width, float height) {
            shape.Extent.X = width;
            shape.Extent.Y = height;
        }

        public void RenderPlayer() {
            //TODO: Next version needs animation. Skipped for clarity.
            Entity.Image = taxiOrientation == Orientation.Left
                ? taxiBoosterOffImageLeft
                : taxiBoosterOffImageRight;
            Entity.RenderEntity();
        }

        public void Move() {
            Vec2F newPos = shape.Direction + shape.Position;
            if (!(newPos.X < 0.0f ||
                  newPos.X + shape.Extent.X > 1.0f ||
                  newPos.Y + shape.Extent.Y < 0.0f ||
                  newPos.Y > 1.0f)) {
                shape.Position = newPos;
            }
        }
        
        public void ManagePhysics() {

            Speed = Math.Abs(Velocity.Length());
            Vec2F gravity = new Vec2F(0f,-0.004f);
            
            Velocity += (gravity + acceleration) * (float) (StaticTimer.GetElapsedSeconds() - time);
            time = StaticTimer.GetElapsedSeconds();
            Console.WriteLine(Velocity);
            shape.Direction = Velocity;
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message) {
                    case "BOOSTER_TO_LEFT":
                        acceleration = acceleration + new Vec2F(-0.01f, 0);
                        break;
                    case "BOOSTER_TO_RIGHT":
                        acceleration = acceleration + new Vec2F(0.01f, 0);
                        break;
                    case "BOOSTER_UPWARDS":
                        acceleration = acceleration + new Vec2F(0, 0.015f);
                        break;
                    case "STOP_ACCELERATE_LEFT":
                        acceleration = new Vec2F(0, acceleration.Y);
                        break;
                    case "STOP_ACCELERATE_RIGHT":
                        acceleration = new Vec2F(0, acceleration.Y);
                        break;
                    case "STOP_ACCELERATE_UP":
                        acceleration = new Vec2F(acceleration.X, 0);
                        break;
                    
                }
            }
        }
    }
}