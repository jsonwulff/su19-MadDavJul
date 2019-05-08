using DIKUArcade.State;

namespace SpaceTaxi.States {
    public class GameRunning : IGameState {
        private static GameRunning instance = null;
        
        
        private GameRunning() {
            InitializeGameState();
            
        }

        public static GameRunning GetInstance() {
            return GameRunning.instance ?? (GameRunning.instance = new GameRunning());
        }
        
        public static GameRunning NewInstance() {
            return GameRunning.instance = new GameRunning();
        }

        public void GameLoop() {
            throw new System.NotImplementedException();
        }

        public void InitializeGameState() {
        }

        public void UpdateGameLogic() {

        }
        
        public void RenderState() {


        }
        
        
        
        
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_RIGHT":
                break;
            case "KEY_LEFT":
                break;
            case "KEY_SPACE":
                break;
            case "KEY_ESCAPE":
                break;
            }
        }

        public void KeyRelease(string key) {
            switch (key) {
            case "KEY_RIGHT":
            case "KEY_LEFT":
                break;
                
            }
        }

        public void HandleKeyEvent(string keyValue, string keyAction) {
            switch (keyAction) {
            case "KEY_PRESS":
                KeyPress(keyValue);
                break;
            case "KEY_RELEASE":
                KeyRelease(keyValue);
                break;
            }
        }
    }
 
}