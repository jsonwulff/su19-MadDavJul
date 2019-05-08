using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;

namespace SpaceTaxi {
    public class Player : IGameEventProcessor<object> {
        private readonly Image taxiBoosterOffImageLeft;
        private readonly Image taxiBoosterOffImageRight;
        private readonly ImageStride taxiBoosterOnLeft;
        private readonly ImageStride taxiBoosterOnRight;
        private readonly ImageStride taxiBoosterOnBottomLeft;
        private readonly ImageStride taxiBoosterOnBottomOnLeft;
        private readonly ImageStride taxiBoosterOnBottomOnRight;
        private readonly ImageStride taxiBoosterOnBottomRight;
        

        
        
        private readonly DynamicShape shape;
        private Orientation taxiOrientation;

        public Vec2F acceleration;
        public Vec2F Velocity;
        double time = 0;
        public double Speed;
        

        public Player() {
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.0575f, 0.03f));
            taxiBoosterOffImageLeft =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
            taxiBoosterOffImageRight =
                new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));
            taxiBoosterOnLeft = 
                new ImageStride(4, ImageStride.CreateStrides(2,Path.Combine("Assets", "Images", "Taxi_Thrust_Back.png")));
            taxiBoosterOnRight = 
                new ImageStride(4, ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Back_Right.png")));
            taxiBoosterOnBottomLeft = 
                new ImageStride(4, ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom.png")));
            taxiBoosterOnBottomOnLeft = 
                new ImageStride(4, ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back.png")));
            taxiBoosterOnBottomOnRight = 
                new ImageStride(4, ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back_Right.png")));
            taxiBoosterOnBottomRight = 
                new ImageStride(4, ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Right.png")));

            
            Entity = new Entity(shape, taxiBoosterOffImageLeft);
            Velocity = new Vec2F(0,0);
            acceleration = new Vec2F(0,0);
            
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, this);
        }

        public Entity Entity { get; }
        
        private void Direction(Vec2F direction) {
            var shape = Entity.Shape.AsDynamicShape();
            shape.ChangeDirection(direction);
        }

        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        public void SetExtent(float width, float height) {
            shape.Extent.X = width;
            shape.Extent.Y = height;
        }

        public void RenderPlayer() {
            if (acceleration.Y > 0 && acceleration.X < 0) { //Flyver op og venstre
                taxiOrientation = Orientation.Left;
                Entity.Image = taxiBoosterOnBottomOnLeft; 
            } else if (acceleration.Y > 0 && acceleration.X > 0) { //Flyver op og højre
                taxiOrientation = Orientation.Right;
                Entity.Image = taxiBoosterOnBottomOnRight;
            } else if (acceleration.X < 0) { //Flyver venstre
                taxiOrientation = Orientation.Left;
                Entity.Image = taxiBoosterOnLeft;
            } else if (acceleration.Y > 0) { //Flyver op
                Entity.Image = taxiOrientation == Orientation.Left
                    ? taxiBoosterOnBottomLeft
                    : taxiBoosterOnBottomRight;
            }  else if (acceleration.X > 0) { //Flyver højre
                taxiOrientation = Orientation.Right;
                Entity.Image = taxiBoosterOnRight;
            }  else { // Flyver ikke
                Entity.Image = taxiOrientation == Orientation.Left
                    ? taxiBoosterOffImageLeft
                    : taxiBoosterOffImageRight; 
            }
            Entity.RenderEntity();
            
        }
        
        public void ManagePhysics() {
            Console.WriteLine(Velocity);
            Speed = Math.Abs(Velocity.Length());
            Vec2F gravity = new Vec2F(0f,-0.004f);
            
            Velocity += (gravity + acceleration) * (float) (StaticTimer.GetElapsedSeconds() - time);
            time = StaticTimer.GetElapsedSeconds();
            
            shape.Direction = Velocity;
        }
        
        /// <summary>
        /// Updates the movement of player object.
        /// </summary>
        public void Move() {
            Vec2F newPos = Entity.Shape.AsDynamicShape().Direction + Entity.Shape.Position;
            if (!(newPos.X < 0.0f ||
                  newPos.X + Entity.Shape.Extent.X > 1.0f ||
                  newPos.Y + Entity.Shape.Extent.Y < 0.0f ||
                  newPos.Y > 1.0f)) {
                Entity.Shape.Move();
            }
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType != GameEventType.PlayerEvent) {
                return;
            }
            switch (gameEvent.Message) {
            case "BOOSTER_UPWARDS":
                acceleration = acceleration + new Vec2F(0, 0.015f);
                break;
            case "BOOSTER_TO_RIGHT":
                acceleration = acceleration + new Vec2F(0.01f, 0);
                break;
            case "BOOSTER_TO_LEFT":
                acceleration = acceleration + new Vec2F(-0.01f, 0);
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