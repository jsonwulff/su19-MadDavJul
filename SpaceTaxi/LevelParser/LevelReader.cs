using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class LevelReader {
        private string[] levelData;

        public string[] LevelMapContainer;
        public string[] MetaContainer;
        public string[] KeyContainer;
        public string[] CustomerContainer;
        public char[] Platforms;
        
        
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

        /// <summary>
        /// Initializes the parsing process, by getting the file path and splitting the
        /// data into LevelMapContainer, MetaContainer, keyContainer and customerContainer
        /// </summary>
        /// <param name="filename"> filename is the name of the text file to parse</param>
        public void ReadFile(string filename) {
            var path = GetLevelFilePath(filename);
            LevelMapContainer = GetLevelMap(path);
            levelData = GetLevelData(path);
            MetaContainer = GetMetaData(levelData);
            KeyContainer = GetKeyLegendData(levelData);
            CustomerContainer = GetCustomerData(levelData);
            Platforms = GetPlatform(MetaContainer);
        }

        /// <summary>
        /// Sets MapContainer to the text representation of the map
        /// </summary>
        /// <param name="path"> file path of the textfile</param>
        private string[] GetLevelMap(string path) {
            return File.ReadLines(path).Take(23).ToArray();
        }

        private string[] GetLevelData(string path) {
            return File.ReadLines(path).Skip(24).Where(line => line != "").ToArray();
        }
        
        private string[] GetKeyLegendData(string[] leveldata) {
            var retval = new List<string>();
            Regex keyLegendRegex = new Regex(@"\S\)\s");
            foreach (var line in leveldata) {
                if (keyLegendRegex.IsMatch(line)) {
                    retval.Add(line);
                }
            }
            return retval.ToArray();
        }

        private string[] GetCustomerData(string[] leveldata) {
            var retval = new List<string>();
            Regex customerRegex = new Regex(@"Customer");
            foreach (var line in leveldata) {
                if (customerRegex.IsMatch(line)) {
                    retval.Add(line.Substring(10));
                }
            }
            return retval.ToArray();
        }
        
        private string[] GetMetaData(string[] leveldata) {
            var retval = new List<string>();
            Regex keyLegendRegex = new Regex(@"\S\)\s");
            Regex customerRegex = new Regex(@"Customer");
            foreach (var line in leveldata) {
                if (!keyLegendRegex.IsMatch(line) && !customerRegex.IsMatch(line)) {
                    retval.Add(line);
                }
            }
            return retval.ToArray();
        }

        
        private char[] GetPlatform(string[] metaContent) {
            var retval = "";
            var platformRegx = new Regex(@"Platforms:");
            foreach (var line in metaContent) {
                if (platformRegx.IsMatch(line)) {
                    retval = Regex.Replace(line.Substring(11), @"[, ]", "");
                }
            }
            return retval.ToCharArray();
        }
    }
}