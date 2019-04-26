using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class MapCreator {
        private ASCIIReader asciiReader;
        private Translator translator;

        public MapCreator() {
            asciiReader = new ASCIIReader();
            translator = new Translator();
        }

        public Map CreateMap(string filename) {
            asciiReader.ReadFile(filename);
            translator.CreateImageDictionary(asciiReader.KeyContainer);
            Map map = new Map(translator.CreateEntities(asciiReader.MapContainer),"test",translator.PlayerPostiotion,asciiReader.CustomerContainer);
            return map;
        }
    }
}