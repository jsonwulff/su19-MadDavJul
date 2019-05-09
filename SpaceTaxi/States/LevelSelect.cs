using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;

namespace SpaceTaxi.States {
    public class LevelSelect : IGameState {
        private static LevelSelect instance = null;
        
        private Entity backGroundImage;
        private Text testText;
        private Text[] menuButtons;
        private int activeMenuButton;
        private Vec3I activeColor;
        private Vec3I inactiveColor;
        
        public LevelSelect() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "spaceBackground.png")));
            activeColor = new Vec3I(255,255,255);
            inactiveColor = new Vec3I(100,100,100);
            
            InitializeGameState();
        }
        
        public static LevelSelect GetInstance() {
            return LevelSelect.instance ?? (LevelSelect.instance = new LevelSelect());
        }
        
        public void GameLoop() {
            throw new System.NotImplementedException();
            
        }

        public void InitializeGameState() {
            var retval = new List<Text>();           
            var i = 0;
            foreach(KeyValuePair<string, Map> entry in MapCreator.GetInstance().mapDictionary)
            {
                retval.Add(new Text(entry.Value.LevelName,
                    new Vec2F(0.35f, 0.4f - i * 0.1f), 
                    new Vec2F(0.5f, 0.5f)));
                    i++;
            }
            testText = new Text("TESTSETSTESTETASET",
                new Vec2F(0.0f,-0.5f),
                new Vec2F(1.0f,1.0f));
            testText.SetColor(inactiveColor);
            testText.SetFontSize(20);
            menuButtons = retval.ToArray();
            
            HandleButtons();

        }

        public void UpdateGameLogic() {
            LevelSelect.GetInstance();
        }

        public void RenderState() {
            backGroundImage.RenderEntity();
            foreach (var button in menuButtons) {
                button.RenderText();
            }

            testText.RenderText();
        }
        
        public void HandleButtons() {
            foreach (var button in menuButtons) {
                button.SetColor(inactiveColor);    
            }
            menuButtons[Math.Abs(activeMenuButton % menuButtons.Length)].SetColor(activeColor);
            RenderState();
        }
        
        public void ActivateButton() {
            SpaceTaxiBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent, this, "CHANGE_LEVEL",
                    MapCreator.GetInstance().levelsInFolder[Math.Abs(activeMenuButton % menuButtons.Length)], ""));
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