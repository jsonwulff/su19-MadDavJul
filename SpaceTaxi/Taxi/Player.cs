﻿using System;
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
        private static Player instance = null;
        
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

        public bool onPlatform = false;
        public bool alive = true;
        
        public Vec2F acceleration;
        public Vec2F Velocity;
        public double time = 0;
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
        
        public static Player GetInstance() {
            return Player.instance ?? (Player.instance = new Player());
        }

        public void SetPosition(float x, float y) {
            shape.Position.X = x;
            shape.Position.Y = y;
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
            Entity.Shape.Move();
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType != GameEventType.PlayerEvent) {
                return;
            }
            switch (gameEvent.Message) {
            case "BOOSTER_UPWARDS":
                if (onPlatform) {
                    onPlatform = false;
                    time = 0;
                    Velocity = new Vec2F(0.0f,0.0f);
                    acceleration = new Vec2F(0,0);
                    StaticTimer.RestartTimer();
                }
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