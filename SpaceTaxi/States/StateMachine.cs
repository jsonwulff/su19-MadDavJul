using DIKUArcade.EventBus;
using DIKUArcade.State;
using Galaga_Exercise_3.GalagaStates;
using SpaceTaxi.States;

namespace SpaceTaxi {
    public class StateMachine : IGameEventProcessor<object>{
        public IGameState ActiveState { get; private set; }
        
        public StateMachine() {
            ActiveState = MainMenu.GetInstance();
            
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.InputEvent, this);
        }



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
            }
        }
        
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            throw new System.NotImplementedException();
        }
    }
}