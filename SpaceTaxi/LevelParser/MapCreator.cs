using System.IO;
using System.Text.RegularExpressions;
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
        
        /// <summary>
        /// Extracts the level name from the meta data
        /// </summary>
        /// <param name="metaContainer">string array containing meta data from level file</param>
        /// <returns>String with name of game</returns>
        private string GetMapName(string[] metaContainer) {
            Regex levelNameRegex = new Regex(@"Name:");
            foreach (string line in metaContainer) {
                if (levelNameRegex.IsMatch(line)) {
                    return line.Substring(6);
                }
            }

            return "No level name found";
        }
        
        /// <summary>
        /// Runs all of the necessary methods for retuning a map object. First by
        /// reading the txt file, splitting the data and translating the data into
        /// a map object
        /// </summary>
        /// <param name="filename">
        /// String with the name of the level file, including .txt extension.
        /// </param>
        /// <returns>A Map object</returns>
        public Map CreateMap(string filename) {
            asciiReader.ReadFile(filename);
            translator.CreateImageDictionary(asciiReader.KeyContainer);
            return new Map(translator.CreateEntities(asciiReader.MapContainer),
                GetMapName(asciiReader.MetaContainer),
                translator.PlayerPostiotion,
                asciiReader.CustomerContainer);
        }
    }
}