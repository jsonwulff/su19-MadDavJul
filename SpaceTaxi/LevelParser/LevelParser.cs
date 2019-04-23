using System.IO;

namespace SpaceTaxi.LevelParser {
    public class LevelParser {

        public string[] FileContent(string filename) {
            return File.ReadAllLines(filename);
        }

    }
}