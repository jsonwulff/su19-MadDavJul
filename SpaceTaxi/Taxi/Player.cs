using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using SpaceTaxi.Customers;

namespace SpaceTaxi {
    public class Player : IGameEventProcessor<object> {
        private static Player instance = null;
        
        private readonly Image taxiBoosterOffImageLeft;
        private readonly Image taxiBoosterOffImageRight;
        private readonly ImageStride taxiBoosterOnLeft;
        private readonly ImageStride taxiBoosterOnRight;
        private readonly ImageStride taxiBoosterOnBottomLeft;
        private readonly ImageStride taxiBoosterOnBottomOnLeft;
        private readonly ImageStride taxiBoosterOnBottomOnRight;
        private readonly ImageStride taxiBoosterOnBottomRight;

        private readonly List<Image> explsionStrides;
        private int explosionLength = 500;
        public AnimationContainer explosion;
        
        private readonly DynamicShape shape;
        private Orientation taxiOrientation;
        public Entity Entity { get; }

        public bool onPlatform;
        public bool alive = true;
        public double deathTime { get; private set; }

        public Customer pickedUpCustomer;
        public bool DeliveryOnTime;
        
        private Vec2F gravity = new Vec2F(0f,-0.004f);
        private Vec2F acceleration;
        private Vec2F velocity;
        private double time;
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

            explsionStrides = ImageStride.CreateStrides(8,Path.Combine("Assets", "Images", "Explosion.png"));
            explosion = new AnimationContainer(1);
            
            Entity = new Entity(shape, taxiBoosterOffImageLeft);
            velocity = new Vec2F(0,0);
            acceleration = new Vec2F(0,0);
            
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.PlayerEvent, this);
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.TimedEvent, this);
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns>Instance of Player</returns>
        public static Player GetInstance() {
            return Player.instance ?? (Player.instance = new Player());
        }
        
        /// <summary>
        /// Set the players position
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }
        
        /// <summary>
        /// Render the player according to the acceleration/velocity
        /// </summary>
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

            if (alive) {
                Entity.RenderEntity();
            }
        }

        /// <summary>
        /// Plays an animation on the player position.
        /// </summary>
        public void KillPlayer() {
            alive = false;
            deathTime = StaticTimer.GetElapsedSeconds() + 1.0;
            explosion.AddAnimation(
                new StationaryShape(shape.Position, 
                    new Vec2F(shape.Extent.X, shape.Extent.X)), explosionLength, 
                new ImageStride(explosionLength / 8, explsionStrides));
        }
        
        /// <summary>
        /// Simulates pseudo-physics for the Player object
        /// </summary>
        public void ManagePhysics() {
            Speed = Math.Abs(velocity.Length());
            velocity += (gravity + acceleration) * (float) (StaticTimer.GetElapsedSeconds() - time);
            time = StaticTimer.GetElapsedSeconds();
            shape.Direction = velocity;
        }
        
        /// <summary>
        /// Resets players properties
        /// </summary>
        public void ResetPlayer() {
            alive = true;
            velocity = new Vec2F(0,0);
            acceleration = new Vec2F(0,0);
            time = 0;
            onPlatform = false;
            shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            StaticTimer.RestartTimer();
        }
        
        /// <summary>
        /// Updates the movement of player object.
        /// </summary>
        public void Move() {
            shape.Move();
            if (onPlatform || !alive) {
                shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            } else {
                ManagePhysics();
            }
        }

        /// <summary>
        /// Listens to events from the gameBus
        /// </summary>
        /// <param name="eventType">Type of event received</param>
        /// <param name="gameEvent">The Event</param>
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message) {
                case "BOOSTER_UPWARDS":
                    if (onPlatform) {
                        onPlatform = false;
                        velocity = new Vec2F(0.0f, 0.0f);
                        acceleration = new Vec2F(0, 0.015f);
                        time = StaticTimer.GetElapsedSeconds();
                    }

                    acceleration = acceleration + new Vec2F(0, 0.015f);
                    break;
                case "BOOSTER_TO_RIGHT":
                    acceleration = acceleration + new Vec2F(0.01f, 0);
                    break;
                case "BOOSTER_TO_LEFT":
                    acceleration = acceleration + new Vec2F(-0.01f, 0);
                    break;
                case "STOP_ACCELERATE_SIDEWAYS":
                    acceleration = new Vec2F(0, acceleration.Y);
                    break;
                case "STOP_ACCELERATE_UP":
                    acceleration = new Vec2F(acceleration.X, 0);
                    break;
                case "CUSTOMER_PICKED_UP":
                    DeliveryOnTime = true;
                    break;
                }
            } else if (eventType == GameEventType.TimedEvent) {
                switch (gameEvent.Message) {
                case "DELIVERY_TIME_EXCEEDED":
                    DeliveryOnTime = false;
                    break;
                }
            }
        }
    }
}