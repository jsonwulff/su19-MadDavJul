using System;
using System.IO;
using NUnit.Framework;
using SpaceTaxi;

namespace SpaceTaxiTests {
    
    [TestFixture]
    public class TestLevelParser {
        
        private LevelReader levelReader;
        
        [SetUp]
        public void CreateObjects() {
            levelReader = new LevelReader();
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
            levelReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in levelReader.LevelMapContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestMetaContainer(string filename) {
            levelReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in levelReader.MetaContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestKeyContainer(string filename) {
            levelReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in levelReader.KeyContainer) {
                Assert.IsTrue(Array.Exists(file, element => element == line));
            }
        }
        
        [TestCase("the-beach.txt")]
        [TestCase("short-n-sweet.txt")]
        public void TestCustomerContainer(string filename) {
            levelReader.ReadFile(filename);
            string[] file = GetLevelFile(filename);
            foreach (var line in levelReader.CustomerContainer) {
                Assert.IsTrue(Array.Exists(file, line.EndsWith));
            }
        }
        
        
    }
}