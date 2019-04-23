using System;
using System.IO;

namespace SpaceTaxi.LevelParser {
    public class ASCIIReader {
        
        /// <summary>
        /// Finds full directory path of the given level.
        /// </summary>
        /// <remarks>This code is borrowed from Texture.cs in DIKUArcade.</remarks>
        /// <param name="filename">Filename of the level.</param>
        /// <returns>Directory path of the level.</returns>
        /// <exception cref="FileNotFoundException">File does not exist.</exception>
        private string GetLevelFilePath(string filename) {
            // Find base path.
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));

            while (dir.Name != "bin") {
                dir = dir.Parent;
            }
            dir = dir.Parent;

            // Find level file.
            string path = Path.Combine(dir.FullName.ToString(), "Levels", filename);

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Error: The file \"{path}\" does not exist.");
            }

            return path;
        }

        public string[] FileContent() {
            return File.ReadAllLines(GetLevelFilePath("the-beach.txt"));
        }

        public void MapContent() {
            for (int i = 0; i < 23; i++) {
                Console.WriteLine(FileContent()[i]);
            }
        }
        
        
        public void KeyLegendContent() {
            string regexPattern = "\\S\\)\\s";
            foreach (string s in FileContent()) {
                if (System.Text.RegularExpressions.Regex.IsMatch(s, regexPattern)) {
                    Console.WriteLine(s);
                }
            }
        }
        
        public void FindKeyLegendImage(char character) {
            string regexPattern = character + "\\)\\s";
            foreach (string s in FileContent()) {
                if (System.Text.RegularExpressions.Regex.IsMatch(s, regexPattern)) {
                    Console.WriteLine(s.Substring(3));
                }
            }
        }

        public void PrintPos() {
            int i = 0;
            foreach (string line in FileContent()) {
                int j = 0;
                foreach (char c in line) {
                    Console.Write(i+","+j+" ");
                    j++;
                }

                i++;
                Console.Write("\n");
            }
        }


    }
}