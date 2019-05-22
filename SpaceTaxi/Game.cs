using System.Collections.Generic;
using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Timers;


namespace SpaceTaxi {
    public class Game : IGameEventProcessor<object> {
        private Window win;
        private GameTimer gameTimer;

        private StateMachine stateMachine;
        
        private GameEventBus<object> eventBus;
        
        
        public Game() {
            // window
            win = new Window("Space Taxi Game v0.1", 500, AspectRatio.R1X1);

            // game timer
            gameTimer = new GameTimer(60); // 60 UPS, no FPS limit
            
            // event bus
            eventBus = SpaceTaxiBus.GetBus();
            eventBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window, e.g. CloseWindow()
                GameEventType.PlayerEvent, // commands issued to the player object, e.g. move,
                GameEventType.GameStateEvent,
                GameEventType.StatusEvent
            });
            win.RegisterEventBus(eventBus);

            // event delegation
            eventBus.Subscribe(GameEventType.WindowEvent, this);
            stateMachine = new StateMachine();
        }

        public void GameLoop() {
            while (win.IsRunning()) {
                gameTimer.MeasureTime();

                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();
                    eventBus.ProcessEvents();
                    stateMachine.ActiveState.UpdateGameLogic();
                }

                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    stateMachine.ActiveState.RenderState();
                      

                    win.SwapBuffers();
                }

                if (gameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps from the timer
                    win.Title = "Space Taxi | UPS: " + gameTimer.CapturedUpdates + ", FPS: " +
                                 gameTimer.CapturedFrames;
                }
            }
        }


        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                }
            }
        }
    }
}