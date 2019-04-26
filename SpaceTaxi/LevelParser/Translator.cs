using System;
using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class Translator {
        private Dictionary<char, Image> imageDictionary;
        private Vec2F entitySize;
        public (float x, float y) PlayerPostiotion;
        
        public Translator() {
            entitySize = new Vec2F(0.025f,0.025f);
        }

        public void CreateImageDictionary(string[] keyContent) {
            Dictionary<char, Image> dictionary = new Dictionary<char, Image>();
            foreach (var line in keyContent) {
                dictionary.Add(line[0], new Image(Path.Combine("Assets", "Images", line.Substring(3))));
            }

            imageDictionary = dictionary;
        }

        private Vec2F TranslatePos(int x, int y) {
            return new Vec2F(x * 0.025f, 0.975f - y * 0.025f);
        }

        private Entity TranslateToEntity(int x, int y, char c) {
            var entity = new Entity(
                new StationaryShape(
                    TranslatePos(x, y),
                    entitySize),
                imageDictionary[c]
            );
            return entity;
        }

        public EntityContainer<Entity> CreateEntities(string[] mapContainer) {
            var mapentities = new EntityContainer<Entity>();
            for (int y = 0; y < 23; y++) {
                var line = mapContainer[y];
                for (int x = 0; x < 40; x++) {
                    if (line[x] == '>') {
                        PlayerPostiotion = (x * 0.025f, 0.975f - y * 0.025f);
                    } else if (imageDictionary.ContainsKey(line[x])) {
                        mapentities.AddStationaryEntity(TranslateToEntity(x,y,line[x]));    
                    }
                }
            }

            return mapentities;
        }
    }
}