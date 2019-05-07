using DIKUArcade.EventBus;

namespace SpaceTaxi.States {
    public class SpaceTaxiBus {
        private static GameEventBus<object> eventBus;
        public static GameEventBus<object> GetBus() {
            return SpaceTaxiBus.eventBus ?? (SpaceTaxiBus.eventBus = 
                       new GameEventBus<object>());
        }
    }
}