using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;
using DIKUArcade.Timers;

namespace SpaceTaxi.States {
    public class LevelSelect : IGameState {
        private static LevelSelect instance = null;
        
        private Entity backGroundImage;
        private Text[] menuButtons;
        private int activeMenuButton;
        private Vec3I activeColor;
        private Vec3I inactiveColor;
        
        public LevelSelect() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
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
            foreach(KeyValuePair<string, Map> entry in MapCreator.GetInstance().mapDictionary) {
                var menubotton = new Text(entry.Value.LevelName,
                    new Vec2F(0.35f, 0.4f - i * 0.1f),
                    new Vec2F(0.5f, 0.5f));
                menubotton.SetFontSize(30);
                retval.Add(menubotton);
                i++;
            }
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

        }
        
        public void HandleButtons() {
            foreach (var button in menuButtons) {
                button.SetColor(inactiveColor);    
            }
            menuButtons[Math.Abs(activeMenuButton % menuButtons.Length)].SetColor(activeColor);
            RenderState();
        }
        
        public void ActivateButton() {
            //Player.GetInstance().ResetPlayer();
            SpaceTaxiBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent, this, "RESET_SCORE","", ""));
            SpaceTaxiBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent, this, "CHANGE_LEVEL",
                    MapCreator.GetInstance().levelsInFolder[Math.Abs(activeMenuButton % menuButtons.Length)], ""));
            Player.GetInstance().pickedUpCustomer = null;
            StaticTimer.RestartTimer();
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