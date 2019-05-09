using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using DIKUArcade.Timers;

namespace SpaceTaxi.States {
    public class GamePaused : IGameState {
        private static GamePaused instance = null;

        private Entity backGroundImage;
        private Text[] menuButtons;
        private Text continueButton;
        private Text mainMenu;
        private int activeMenuButton;
        private int maxMenuButtons = 2;
        private Vec3I activeColor;
        private Vec3I inactiveColor;

        public GamePaused() {
            InitializeGameState();
        }
        
        public static GamePaused GetInstance() {
            return GamePaused.instance ?? (GamePaused.instance = new GamePaused());
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }

        public void InitializeGameState() {
            activeMenuButton = 0;
            backGroundImage = new Entity(new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            menuButtons = new Text[maxMenuButtons];
            continueButton = new Text("Continue", new Vec2F(0.5f, 0.5f), new Vec2F(0.2f, 0.2f));
            mainMenu = new Text("Main Menu", new Vec2F(0.5f, 0.3f), new Vec2F(0.2f, 0.2f));
            menuButtons[0] = continueButton;
            menuButtons[1] = mainMenu;
            
            activeColor = new Vec3I(255,255,255);
            inactiveColor = new Vec3I(190,190,190);
            HandleButtons();

        }

        public void UpdateGameLogic() {
            GamePaused.GetInstance();
        }

        public void RenderState() {
            backGroundImage.RenderEntity();
            foreach (var button in menuButtons) {
                button.RenderText();
            }

        }

        public void HandleButtons() {
            foreach (var button in menuButtons) {
                button.SetColor(inactiveColor);                
            }
            menuButtons[Math.Abs(activeMenuButton % maxMenuButtons)].SetColor(activeColor);
            RenderState();
        }

        public void ActivateButton() {
            switch (Math.Abs(activeMenuButton % maxMenuButtons)) {
            case 0:
                StaticTimer.ResumeTimer();
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE", "GAME_RUNNING", ""));
                break;
            case 1:
                GameRunning.NewInstance();
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE", "MAIN_MENU", ""));
                break;
            }
        }
        
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_UP":
                activeMenuButton -= 1;
                HandleButtons();
                break;
            case "KEY_DOWN":
                activeMenuButton += 1;
                HandleButtons();
                break;
            case "KEY_ENTER":
                ActivateButton();
                HandleButtons();
                break;
            }
        }
        
        public void HandleKeyEvent(string keyValue, string keyAction) {
            switch (keyAction) {
                case "KEY_PRESS":
                    KeyPress(keyValue);
                    break;
            }
        }
    }
}