using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceTaxi.Customers;

namespace SpaceTaxi {
    public class MapCreator {
        private static MapCreator instance = null;
        
        public string[] levelsInFolder; // As filenames
        public Dictionary<string, Map> mapDictionary;

        private ASCIIReader asciiReader;
        private Translator translator;
        private string[] levelNames;
        private CustomerCreator customerCreator;

        public MapCreator() {
            asciiReader = new ASCIIReader();
            translator = new Translator();
            customerCreator = new CustomerCreator();
            levelsInFolder = GetLevels(); // Levels as filenames
            mapDictionary = makeLevels();         
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns>Instance of MapCreator</returns>
        public static MapCreator GetInstance() {
            return MapCreator.instance ?? (MapCreator.instance = new MapCreator());
        }
        /// <summary>
        /// Get all levels in the Levels folder
        /// </summary>
        /// <returns>An array of filenames in the Levels folder</returns>
        private string[] GetLevels() {
            // Find base path.
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));

            while (dir.Name != "bin") {
                dir = dir.Parent;
            }
            dir = dir.Parent;

            // Find level file.
            string path = Path.Combine(dir.FullName, "Levels");

            return Directory.GetFiles(path, "*.txt").Select(f => Path.GetFileName(f)).ToArray();
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
        /// Populates the mapDictionary, for all the maps.
        /// </summary>
        /// <returns>Dictionary of all map objects.</returns>
        private Dictionary<string, Map> makeLevels() {
            var retval =  new Dictionary<string, Map>();
            int levelNumber = 0;
            foreach (var levelFile in levelsInFolder) {
                asciiReader.ReadFile(levelFile);
                translator.CreateImageDictionary(asciiReader.KeyContainer);
                retval.Add(levelFile, new Map(
                    translator.CreateMapEntities(asciiReader.MapContainer, asciiReader.Platforms),
                    GetMapName(asciiReader.MetaContainer), 
                    levelFile, 
                    levelNumber,
                    translator.PlayerPostiotion,
                    asciiReader.CustomerContainer,
                    translator.CreatePlatformEntities(asciiReader.MapContainer, asciiReader.Platforms),
                    customerCreator));
                levelNumber++;
            }
            return retval;
        }
    }
}