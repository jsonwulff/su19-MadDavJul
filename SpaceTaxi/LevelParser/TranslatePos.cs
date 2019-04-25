using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DIKUArcade.Math;

namespace SpaceTaxi.LevelParser {
    public class TranslatePos {
        private ASCIIReader asciiReader;
        
        public TranslatePos() {
            var asciiReader = new ASCIIReader();    
        }

        public Dictionary<char, List<Vec2F>> TranslatePosition(List<string> mapText) {
            var mapDictionary = new Dictionary<char, List<Vec2F>>();
            for (int i = 0; i < 23; i++)  {
                for (int j = 0; j < 40; j++) {
                    if (mapText[i][j] == ' ') {
                        //nothing
                    } else if (mapDictionary.ContainsKey(mapText[i][j])) {
                        mapDictionary[mapText[i][j]].Add(new Vec2F((float)j/40, 1-(float)i/23));
                    } else {
                        mapDictionary.Add(mapText[i][j], new List<Vec2F>());
                        mapDictionary[mapText[i][j]].Add(new Vec2F((float)j/40, 1-(float)i/23));
                    }
                     
                }                
            }

            return mapDictionary;
        }
        
    }
}