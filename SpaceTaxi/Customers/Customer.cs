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
            spawnTime = Convert.ToDouble(spawntime);
            spawnPlatform = spawnplatform;
            deliveryTime = Convert.ToInt32(deliverytime);
            dropOffPoints = Convert.ToInt32(dropoffpoints);
            originLevel = originlevel;
            setDestination(destinationcode);
            
            shape = new DynamicShape(new Vec2F(0.0f,0.0f), new Vec2F(0.018f,0.03f));
            customerStandRight = 
                new Image(Path.Combine("Assets", "Images","CustomerStandRight.png"));
            customerStandLeft = 
                new Image(Path.Combine("Assets", "Images","CustomerStandLeft.png"));
            
            Entity = new Entity(shape, customerStandRight);
        }

        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
        }

        public void RenderCustomer() {
            // TODO: Move Render entity and other stuff when spawn time is met
            Entity.RenderEntity();
            if (StaticTimer.GetElapsedSeconds() > spawnTime) {
            }
        }

        private void setDestination(string destinationCode) {
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

        private void pickUpCustomer() {
            Player.GetInstance().pickedUpCustomer = this;
        }

        public void pickUpCollision() {
            var collision = CollisionDetection.Aabb(Player.GetInstance().Entity.Shape.AsDynamicShape(), shape);
            if (collision.Collision) {
                Console.WriteLine("{0} has been picked up",customerName);
                pickUpCustomer();
            }
        }
    }
}