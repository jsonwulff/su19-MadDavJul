using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class Map {
        public EntityContainer<Entity> MapContainer;
        public EntityContainer<Entity> PlatformContainer;
        public string LevelName;
        public (float x, float y) PlayerPosition;
        public string[] CustomerData;
        public char[] Platforms;

        public Map(EntityContainer<Entity> mapContainer, string levelName,
            (float x, float y) playerPosition, string[] customerData, EntityContainer<Entity> platformContainer) {
            MapContainer = mapContainer;
            LevelName = levelName;
            PlayerPosition = playerPosition;
            CustomerData = customerData;
            PlatformContainer = platformContainer;
        }
    }
}