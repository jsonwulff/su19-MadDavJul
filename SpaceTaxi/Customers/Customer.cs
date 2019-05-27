using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;

namespace SpaceTaxi.Customers {
    public class Customer {
        private readonly Image customerStandRight;
        private readonly Image customerStandLeft;
        private readonly ImageStride customerWalkRight;
        private readonly ImageStride customerWalkLeft;

        private DynamicShape shape;
        private Orientation customerOrientation;

        private string customerName;
        private double spawnTime;
        private int originLevel;
        public char spawnPlatform;
        public int destinationLevel;
        public char destinationPlatform;
        private int deliveryTime;
        public int dropOffPoints;
        private bool customerSpawned = false;
        private bool pickedUp = false;
        


        public Entity Entity { get; }

        public Customer() {
            shape = new DynamicShape(new Vec2F(0.0f,0.0f), new Vec2F(0.018f,0.03f));
            customerStandRight = 
                new Image(Path.Combine("Assets", "Images","CustomerStandRight.png"));
            customerStandLeft = 
                new Image(Path.Combine("Assets", "Images","CustomerStandLeft.png"));
            
            Entity = new Entity(shape, customerStandRight);
            
        }

        public Customer(string customername, string spawntime, char spawnplatform, 
            string destinationcode, string deliverytime, string dropoffpoints, int originlevel) {
            customerName = customername;
            spawnTime = Convert.ToDouble(spawntime) + StaticTimer.GetElapsedSeconds();
            spawnPlatform = spawnplatform;
            deliveryTime = Convert.ToInt32(deliverytime);
            dropOffPoints = Convert.ToInt32(dropoffpoints);
            originLevel = originlevel;
            SetDestination(destinationcode);
            
            shape = new DynamicShape(new Vec2F(0.0f,0.0f), new Vec2F(0.018f,0.03f));
            customerStandRight = 
                new Image(Path.Combine("Assets", "Images","CustomerStandRight.png"));
            customerStandLeft = 
                new Image(Path.Combine("Assets", "Images","CustomerStandLeft.png"));
            
            Entity = new Entity(shape, customerStandRight);
        }
        /// <summary>
        /// Sets customer position
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        /// <summary>
        /// Renders customer
        /// </summary>
        public void RenderCustomer() {
            if (StaticTimer.GetElapsedSeconds() > spawnTime && !pickedUp) {
                Entity.RenderEntity();
            }
        }

        /// <summary>
        /// Determines the dropoff point for customer.
        /// </summary>
        /// <param name="destinationCode">The string that determines dropoff point </param>
        private void SetDestination(string destinationCode) {
            if (destinationCode == "^") {
                destinationLevel = originLevel + 1;
            } else if (destinationCode.Length == 1) {
                destinationLevel = originLevel;
                destinationPlatform = destinationCode[0];
            } else {
                destinationLevel = originLevel + 1;
                destinationPlatform = destinationCode[1];
            }
        }
        
        /// <summary>
        /// Update method to check for collisions and delivery time
        /// </summary>
        private void PickUpCustomer() {
            Player.GetInstance().pickedUpCustomer = this;
            pickedUp = true;
            SpaceTaxiBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.PlayerEvent, this, "CUSTOMER_PICKED_UP", "", ""));
            TimedEvents.getTimedEvents().AddTimedEvent(
                TimeSpanType.Seconds, deliveryTime,"DELIVERY_TIME_EXCEEDED","","");
        }

        /// <summary>
        /// Checks for collision with player
        /// </summary>
        public void PickUpCollision() {
            if (StaticTimer.GetElapsedSeconds() > spawnTime && !pickedUp) {
                var collision = CollisionDetection.Aabb(Player.GetInstance().Entity.Shape.AsDynamicShape(), shape);
                if (collision.Collision && Player.GetInstance().pickedUpCustomer == null) {
                    PickUpCustomer();
                }
            }
        }
    }
}