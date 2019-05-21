using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi.Customers {
    public class Customer {
        private readonly Image customerStandRight;
        private readonly Image customerStandLeft;
        private readonly ImageStride customerWalkRight;
        private readonly ImageStride customerWalkLeft;

        private DynamicShape shape;
        private Orientation customerOrientation;

        private string customerName;
        private int spawnTime;
        private char spawnPlatform;
        private string destinationPlatform;
        private int deliveryTime;
        private int dropOffPoints;
        
        public Entity Entity { get; }

        public Customer() {
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
            Entity.RenderEntity();
        }

        public void AddPointToScores() {
            throw new NotImplementedException();
        }
    }
}