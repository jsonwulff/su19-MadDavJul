using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.State;

namespace SpaceTaxi.States {
    public class GameOver : IGameState {
        private static GameOver instance = null;
        
        private Entity backGroundImage;
        private Text gameOver;
        private Text toMainMenu;
        private Vec3I textColor;

        public GameOver() {
            backGroundImage = new Entity(
                new StationaryShape(new Vec2F(0,0), new Vec2F(1,1) ), 
                new Image(Path.Combine( "Assets",  "Images", "SpaceBackground.png")));
            textColor = new Vec3I(255,255,255);
            gameOver = new Text("Game Over", new Vec2F(0.0f, -0.3f), new Vec2F(1.0f, 1.0f) );
            gameOver.SetColor(textColor);
            gameOver.SetFontSize(30);
            toMainMenu = new Text("Press enter to continue to main menu",new Vec2F(0.0f, -0.5f), new Vec2F(1.0f, 1.0f) );
            toMainMenu.SetColor(textColor);
            toMainMenu.SetFontSize(16);
        }
        
        public static GameOver GetInstance() {
            return GameOver.instance ?? (GameOver.instance = new GameOver());
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }

        public void InitializeGameState() {
            throw new System.NotImplementedException();
        }

        public void UpdateGameLogic() {
            
        }

        public void RenderState() {
            backGroundImage.RenderEntity();
            gameOver.RenderText();
            toMainMenu.RenderText();
        }
        
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_ENTER":
                SpaceTaxiBus.GetBus().RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.GameStateEvent,this,"CHANGE_STATE", "MAIN_MENU", ""));
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