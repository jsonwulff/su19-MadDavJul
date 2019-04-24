using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpaceTaxi.LevelParser {
    public class ASCIIReader {

        public string[] MapContainer;
        public string[] MetaContainer;
        public string[] KeyContainer;
        public string[] CustomerContainer;
        
        
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

        public void ReadFile(string filename) {
            var path = GetLevelFilePath(filename);
            MapContainer = GetMapData(path);
            MetaContainer = GetMetaData(path);
            KeyContainer = GetKeyLegendData(path);
            CustomerContainer = GetCustomerData(path);
        }


        private string[] GetMapData(string path) {
            return File.ReadLines(path).Take(23).ToArray();
        }

        private string[] GetMetaData(string path) {
            var metaContent = File.ReadLines(path).Skip(24).Where(line => line != "");
            List<string> metaData = new List<string>(); 
            Regex keyLegendRegex = new Regex(@"\S\)\s");
            Regex customerRegex = new Regex(@"Customer");
            foreach (var line in metaContent) {
                if (!keyLegendRegex.IsMatch(line) && !customerRegex.IsMatch(line)) {
                    metaData.Add(line);
                }
                
                
            }
            return metaData.ToArray();
        }

        private string[] GetKeyLegendData(string path) {
            var keyLegendContent = File.ReadLines(path).Skip(24);
            List<string> keyLegendData = new List<string>(); 
            Regex keyLegendRegex = new Regex(@"\S\)\s");
            foreach (string line in keyLegendContent) {
                if (keyLegendRegex.IsMatch(line)) {
                    keyLegendData.Add(line);
                }
            }
            return keyLegendData.ToArray();
        }
        
        private string[] GetCustomerData(string path) {
            Regex keyLegendRegex = new Regex(@"Customer");
            return File.ReadLines(path).Skip(24).Where(line => keyLegendRegex.IsMatch(line)).ToArray();
        }
    }
}