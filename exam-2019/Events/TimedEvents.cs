using DIKUArcade.Timers;

namespace SpaceTaxi {
    public class TimedEvents {
        private static TimedEventContainer timedEvents;

        public static TimedEventContainer getTimedEvents() {
            return TimedEvents.timedEvents ?? (TimedEvents.timedEvents =
                       new TimedEventContainer(10));
        }
    }
}