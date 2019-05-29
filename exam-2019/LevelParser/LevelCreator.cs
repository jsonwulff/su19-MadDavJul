using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpaceTaxi.Customers;

namespace SpaceTaxi {
    public class LevelCreator {
        private static LevelCreator instance = null;
        
        public string[] levelsInFolder; // As filenames
        public Dictionary<string, Level> llevelDictionary;

        private LevelReader levelReader;
        private LevelTranslator levelTranslator;
        private string[] levelNames;
        private CustomerCreator customerCreator;

        public LevelCreator() {
            levelReader = new LevelReader();
            levelTranslator = new LevelTranslator();
            customerCreator = new CustomerCreator();
            levelsInFolder = GetLevels(); // Levels as filenames
            llevelDictionary = makeLevels();         
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns>Instance of LevelCreator</returns>
        public static LevelCreator GetInstance() {
            return LevelCreator.instance ?? (LevelCreator.instance = new LevelCreator());
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
        private string GetLevelName(string[] metaContainer) {
            Regex levelNameRegex = new Regex(@"Name:");
            foreach (string line in metaContainer) {
                if (levelNameRegex.IsMatch(line)) {
                    return line.Substring(6);
                }
            }

            return "No level name found";
        }
        
        /// <summary>
        /// Populates the levelDictionary, for all the levels.
        /// </summary>
        /// <returns>Dictionary of all level objects.</returns>
        private Dictionary<string, Level> makeLevels() {
            var retval =  new Dictionary<string, Level>();
            int levelNumber = 0;
            foreach (var levelFile in levelsInFolder) {
                levelReader.ReadFile(levelFile);
                levelTranslator.CreateImageDictionary(levelReader.KeyContainer);
                retval.Add(levelFile, new Level(
                    levelTranslator.CreateLevelEntities(levelReader.LevelMapContainer, levelReader.Platforms),
                    GetLevelName(levelReader.MetaContainer), 
                    levelFile, 
                    levelNumber,
                    levelTranslator.PlayerPostiotion,
                    levelReader.CustomerContainer,
                    levelTranslator.CreatePlatformEntities(levelReader.LevelMapContainer, levelReader.Platforms, levelNumber),
                    customerCreator));
                levelNumber++;
            }
            return retval;
        }
    }
}