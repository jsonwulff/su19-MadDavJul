using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using SpaceTaxi;
using SpaceTaxi.States;
using Image = DIKUArcade.Graphics.Image;

namespace Galaga_Exercise_3.GalagaStates {
    public class MainMenu : IGameState {
        private static MainMenu instance = null;

        private Entity backGroundImage;
        private Text[] menuButtons;
        private Text newGame;
        private Text levelSelect;
        private Text quit;
        private int activeMenuButton;
        private int maxMenuButtons = 3;
        private Vec3I activeColor;
        private Vec3I inactiveColor;

        public MainMenu() {
            InitializeGameState();
        }
        
        public static MainMenu GetInstance() {
            return MainMenu.instance ?? (MainMenu.instance = new MainMenu());
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }

        public void InitializeGameState() {
            
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            menuButtons = new Text[maxMenuButtons];
            activeMenuButton = 0;
            newGame = new Text("New Game",
                new Vec2F(0.25f, 0.4f), 
                new Vec2F(0.5f, 0.5f));
            levelSelect = new Text("Select level",
                new Vec2F(0.25f, 0.3f), 
                new Vec2F(0.5f, 0.5f));
            quit = new Text("Quit",
                new Vec2F(0.25f, 0.2f), 
                new Vec2F(0.5f, 0.5f));
            menuButtons[0] = newGame;
            menuButtons[1] = levelSelect;
            menuButtons[2] = quit;
            
            activeColor = new Vec3I(255,255,255);
            inactiveColor = new Vec3I(100,100,100);
            
            HandleButtons();
        }

        public void UpdateGameLogic() {
            MainMenu.GetInstance();
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
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.StatusEvent, this, "RESET_SCORE","", ""));
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_LEVEL",
                        MapCreator.GetInstance().levelsInFolder[0], ""));
                Player.GetInstance().pickedUpCustomer = null;
                break;

            case 1:
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent, this, "CHANGE_STATE", "LEVEL_SELECT", ""));
                break;
            case 2:
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
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