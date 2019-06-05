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
        
        public Entity Entity { get; }
        
        public Customer pickedUpCustomer;
        
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
        private AnimationContainer explosion;
        
        private readonly DynamicShape shape;
        private Orientation taxiOrientation;

        public bool onPlatform;
        public bool alive = true;

        public bool DeliveryOnTime;
        
        private Physics physics = new Physics();
        public Vec2F Acceleration;

        public Player() {
            shape = new DynamicShape(new Vec2F(), new Vec2F(0.0575f, 0.03f));
            Entity = new Entity(shape, taxiBoosterOffImageLeft);
            
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
            
            Acceleration = new Vec2F(0,0);

            pickedUpCustomer = null;
            
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
        /// <param name="position">Vec2F Position</param>
        public void SetPosition(Vec2F position) {
            shape.Position = position;
        }
        
        /// <summary>
        /// Render the player according to the acceleration/velocity
        /// </summary>
        public void RenderPlayer() {
            if (Acceleration.Y > 0 && Acceleration.X < 0) {            //Flyver op og venstre
                taxiOrientation = Orientation.Left;
                Entity.Image = taxiBoosterOnBottomOnLeft; 
            } else if (Acceleration.Y > 0 && Acceleration.X > 0) {     //Flyver op og højre
                taxiOrientation = Orientation.Right;
                Entity.Image = taxiBoosterOnBottomOnRight;
            } else if (Acceleration.X < 0) {                           //Flyver venstre
                taxiOrientation = Orientation.Left;
                Entity.Image = taxiBoosterOnLeft;
            } else if (Acceleration.Y > 0) {                           //Flyver op
                Entity.Image = taxiOrientation == Orientation.Left
                    ? taxiBoosterOnBottomLeft
                    : taxiBoosterOnBottomRight;
            }  else if (Acceleration.X > 0) {                          //Flyver højre
                taxiOrientation = Orientation.Right;
                Entity.Image = taxiBoosterOnRight;
            }  else {                                                  // Flyver ikke
                Entity.Image = taxiOrientation == Orientation.Left
                    ? taxiBoosterOffImageLeft
                    : taxiBoosterOffImageRight; 
            }

            if (alive) {
                Entity.RenderEntity();
            }
            explosion.RenderAnimations();
        }

        /// <summary>
        /// Plays an animation on the player position.
        /// </summary>
        public void KillPlayer() {
            alive = false;
            TimedEvents.getTimedEvents().AddTimedEvent(TimeSpanType.Seconds, 1, 
                "GAME_OVER", "", "");
            explosion.AddAnimation(
                new StationaryShape(shape.Position, 
                    new Vec2F(shape.Extent.X, shape.Extent.X)), explosionLength, 
                new ImageStride(explosionLength / 8, explsionStrides));
        }
        

        public void UpdateVelocity(Vec2F force) {
            shape.Direction += force;
        }
        
        public void SetDirection(Vec2F direction) {
            shape.Direction = direction;
        }

        /// <summary>
        /// Resets players properties
        /// </summary>
        public void ResetPlayer() {
            alive = true;
            Acceleration = new Vec2F(0,0);
            onPlatform = false;
            SetDirection( new Vec2F(0.0f, 0.0f));
        }
        
        /// <summary>
        /// Updates the movement of player object.
        /// </summary>
        public void Move() {
            shape.Move();
            if (onPlatform || !alive) {
                shape.AsDynamicShape().Direction = new Vec2F(0.0f, 0.0f);
            } else {
                physics.ManagePhysics();
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
                    }
                    Acceleration = Acceleration + new Vec2F(0.0f, 0.0001f);
                    break;
                case "BOOSTER_TO_RIGHT":
                    Acceleration = Acceleration + new Vec2F(0.0001f, 0);
                    break;
                case "BOOSTER_TO_LEFT":
                    Acceleration = Acceleration + new Vec2F(-0.0001f, 0);
                    break;
                case "STOP_ACCELERATE_SIDEWAYS":
                    Acceleration = new Vec2F(0, Acceleration.Y);
                    break;
                case "STOP_ACCELERATE_UP":
                    Acceleration = new Vec2F(Acceleration.X, 0);
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