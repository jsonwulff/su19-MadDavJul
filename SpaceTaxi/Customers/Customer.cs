using System;
using System.IO;
using DIKUArcade.Entities;
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
        private string spawnTime;
        public char spawnPlatform;
        private string destinationPlatform;
        private string deliveryTime;
        private string dropOffPoints;

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
            string destinationplatform, string deliverytime, string dropoffpoints) {
            customerName = customername;
            spawnTime = spawntime;
            spawnPlatform = spawnplatform;
            destinationPlatform = destinationplatform;
            deliveryTime = deliverytime;
            dropOffPoints = dropoffpoints;
            
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
            if (StaticTimer.GetElapsedSeconds() > Convert.ToDouble(spawnTime)  ) {
               Entity.RenderEntity();
            }
        }

        public void pickUpCollision() {
            var collision = CollisionDetection.Aabb(Player.GetInstance().Entity.Shape.AsDynamicShape(), shape);
            if (collision.Collision) {
                Console.WriteLine("{0} has been picked up",customerName);
            }
        }

        public void AddPointToScores() {
            throw new NotImplementedException();
        }
    }
}