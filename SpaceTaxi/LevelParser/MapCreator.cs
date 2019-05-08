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
                retval.Add(levelFile, new Map(translator.CreateEntities(asciiReader.MapContainer),
                    GetMapName(asciiReader.MetaContainer),
                    translator.PlayerPostiotion,
                    asciiReader.CustomerContainer));
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
        
        /// <summary>
        /// Runs all of the necessary methods for retuning a map object. First by
        /// reading the txt file, splitting the data and translating the data into
        /// a map object
        /// </summary>
        /// <param name="filename">
        /// String with the name of the level file, including .txt extension.
        /// </param>
        /// <returns>A Map object</returns>
//        public Map CreateMap(string filename) {
//            asciiReader.ReadFile(filename);
//            translator.CreateImageDictionary(asciiReader.KeyContainer);
//            return new Map(translator.CreateEntities(asciiReader.MapContainer),
//                GetMapName(asciiReader.MetaContainer),
//                translator.PlayerPostiotion,
//                asciiReader.CustomerContainer);
//        }
    }
}