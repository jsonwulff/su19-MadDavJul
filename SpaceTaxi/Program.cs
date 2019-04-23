using System;

namespace SpaceTaxi {
    internal class Program {
        public static void Main(string[] args) {
            var asciiReader = new LevelParser.ASCIIReader();
            asciiReader.MapContent();
            asciiReader.FindKeyLegendImage('A');
            asciiReader.FindKeyLegendImage('B');
            asciiReader.PrintPos();

//            var game = new Game();
//            game.GameLoop();
        }
    }
}