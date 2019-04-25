using DIKUArcade.Entities;
using DIKUArcade.Math;

namespace SpaceTaxi.LevelParser {
    public class Map {
        public EntityContainer<Entity> MapContainer;
        public string LevelName;
        public Vec2F PlayerPosition;
        public string[] CustomerData;

        public Map(EntityContainer<Entity> mapContainer, string levelName, Vec2F playerPosition, string[] customerData) {
            MapContainer = mapContainer;
            LevelName = levelName;
            PlayerPosition = playerPosition;
            CustomerData = customerData;
        }
    }
}