using System;
using System.IO;
using NUnit.Framework;
using SpaceTaxi;

namespace SpaceTaxiTests {
    
    [TestFixture]
    public class TestLevelParser {
        
        private MapCreator mapCreator;
        private ASCIIReader asciiReader;
        private Translator translator;
        
        [SetUp]
        public void CreateObjects() {
            mapCreator = new MapCreator();
            asciiReader = new ASCIIReader();
            translator = new Translator();
        }


        private string[] GetLevelFile(string filename) {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));

            while (dir.Name != "bin") {
                dir = dir.Parent;
            }
            dir = dir.Parent;
            
            string path = Path.Combine(dir.FullName.ToString(), "Levels", filename);
            return File.ReadAllLines(path);
        }

        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestMapContainer(string filename) {
            asciiReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in asciiReader.MapContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestMetaContainer(string filename) {
            asciiReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in asciiReader.MetaContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestKeyContainer(string filename) {
            asciiReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in asciiReader.KeyContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestCustomerContainer(string filename) {
            asciiReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in asciiReader.CustomerContainer) {
                Assert.IsTrue(Array.Exists(file, line.EndsWith));
            }
        }
        
        
    }
}