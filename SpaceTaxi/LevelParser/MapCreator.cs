using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace SpaceTaxi {
    public class MapCreator {
        private static MapCreator instance = null;
        
        private ASCIIReader asciiReader;
        private Translator translator;
        public string[] levelsInFolder;
        public Dictionary<string, Map> mapDictionary;
        private string[] levelNames;

        public MapCreator() {
            asciiReader = new ASCIIReader();
            translator = new Translator();
            levelsInFolder = GetLevels();
            mapDictionary = makeLevels();         
        }

        public static MapCreator GetInstance() {
            return MapCreator.instance ?? (MapCreator.instance = new MapCreator());
        }

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

        private Dictionary<string, Map> makeLevels() {
            var retval =  new Dictionary<string, Map>();
            
            foreach (var levelFile in levelsInFolder) {
                asciiReader.ReadFile(levelFile);
                translator.CreateImageDictionary(asciiReader.KeyContainer);
                retval.Add(levelFile, new Map(translator.CreateEntities(asciiReader.MapContainer, asciiReader.Platforms),
                    GetMapName(asciiReader.MetaContainer),
                    translator.PlayerPostiotion,
                    asciiReader.CustomerContainer,
                    asciiReader.Platforms));
            }

            return retval;
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
        
    }
}