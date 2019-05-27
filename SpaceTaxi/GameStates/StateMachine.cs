using System;
using DIKUArcade.EventBus;
using DIKUArcade.State;
using Galaga_Exercise_3.GalagaStates;
using SpaceTaxi.States;

namespace SpaceTaxi {
    public class StateMachine : IGameEventProcessor<object>{
        public IGameState ActiveState { get; private set; }

        private GameEventBus<object> eventBus = SpaceTaxiBus.GetBus(); 
        
        public StateMachine() {
            ActiveState = MainMenu.GetInstance();
            
            eventBus.Subscribe(GameEventType.GameStateEvent, this);
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.TimedEvent, this);
            
        }

        /// <summary>
        /// Switches the activeState in between the different gameStates
        /// </summary>
        /// <param name="stateType"> Which state to change to</param>
        private void SwitchState(GameStateType stateType) {
            switch (stateType) {
                case (GameStateType.GameRunning):
                    ActiveState = GameRunning.GetInstance();
                    break;
                case (GameStateType.GamePaused):
                    ActiveState = GamePaused.GetInstance();
                    break;
                case (GameStateType.MainMenu):
                    ActiveState = MainMenu.GetInstance();
                    break;
                case (GameStateType.LevelSelect):
                    ActiveState = LevelSelect.GetInstance();
                    break;
                case (GameStateType.GameOver):
                    ActiveState = GameOver.GetInstance();
                    break;
            }
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.InputEvent) {
                ActiveState.HandleKeyEvent(gameEvent.Message, gameEvent.Parameter1);
            } 
            else if (eventType == GameEventType.GameStateEvent) {
                switch (gameEvent.Message) {
                    case "CHANGE_STATE":
                        SwitchState(StateTransformer.TransformStringToState(gameEvent.Parameter1));
                        break;
                    case "CHANGE_LEVEL":
                        var gameRunning = GameRunning.GetInstance();
                        gameRunning.SetMap(gameEvent.Parameter1);
                        ActiveState = gameRunning;
                        break;
                    }
            } 
            else if (eventType == GameEventType.TimedEvent) {
                switch (gameEvent.Message) {
                    case "GAME_OVER":
                        SwitchState(StateTransformer.TransformStringToState(gameEvent.Message));
                        break;
                }
            }
        }
    }
}