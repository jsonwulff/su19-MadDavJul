using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class Translator {
        private Dictionary<char, Image> imageDictionary;
        private Vec2F entitySize;
        public Vec2F PlayerPostiotion;
        
        public Translator() {
            entitySize = new Vec2F(0.025f,0.025f);
        }

        /// <summary>
        /// creates dictionary of the key legend extracted from textfile and saves it to imageDictionary.
        /// </summary>
        /// <param name="keyContent"> String array of the key legend extracted from textfile. </param>
        public void CreateImageDictionary(string[] keyContent) {
            Dictionary<char, Image> dictionary = new Dictionary<char, Image>();
            foreach (var line in keyContent) {
                dictionary.Add(line[0], new Image(Path.Combine("Assets", "Images", line.Substring(3))));
            }

            imageDictionary = dictionary;
        }

        /// <summary>
        /// Converts integer coordinate to Vec2F.
        /// </summary>
        /// <param name="x"> x-value of integer coordinate.</param>
        /// <param name="y"> y-value of integer coordinate.</param>
        /// <returns> Vec2F representation of integer coordinate.</returns>
        private Vec2F TranslatePos(int x, int y) {
            return new Vec2F(x * 0.025f, 0.975f - y * 0.025f);
        }
        
        /// <summary>
        /// Help-method that creates entity from integer coordinate and a char.
        /// </summary>
        /// <remarks> This method utilizes imageDictionary and TranslatePos</remarks>
        /// <param name="x"> x-value of integer coordinate.</param>
        /// <param name="y"> y-value of integer coordinate.</param>
        /// <param name="c"> Character that represents image in imageDictionary.</param>
        /// <returns>A entity object</returns>
        private Entity TranslateToEntity(int x, int y, char c) {
            var entity = new Entity(
                new StationaryShape(
                    TranslatePos(x, y),
                    entitySize),
                imageDictionary[c]
            );
            return entity;
        }

        /// <summary>
        /// Iterates through mapContainer and creates entities for each character in the textfile.
        /// </summary>
        /// <param name="mapContainer"> string array of the map-text</param>
        /// <returns>EntityContainer of all the 'blocks' on the map.</returns>
        public EntityContainer<Entity> CreateMapEntities(string[] mapContainer, char[] platforms) {
            var mapEntities = new EntityContainer<Entity>();
            for (int y = 0; y < 23; y++) {
                var line = mapContainer[y];
                for (int x = 0; x < 40; x++) {
                    if (line[x] == '>') {
                        PlayerPostiotion = new Vec2F(x * 0.025f, 0.975f - y * 0.025f);
                    } else if (imageDictionary.ContainsKey(line[x]) && !platforms.Contains(line[x])) {
                        mapEntities.AddStationaryEntity(TranslateToEntity(x,y,line[x]));    
                    }
                }
            }

            return mapEntities;
        }
        /// <summary>
        /// Creates the seperate Platform container, for making seperate collisions
        /// </summary>
        /// <param name="mapContainer"> Container of all characters in the map</param>
        /// <param name="platforms">Container of platform characters in the legend</param>
        /// <returns></returns>
        public Dictionary<char, Platform> CreatePlatformEntities(string[] mapContainer, char[] platforms, int levelNumber) {
            var platformDictionary = new Dictionary<char, Platform>();
            
            foreach (var character in platforms) {
                platformDictionary.Add(character, new Platform(character));
                platformDictionary[character].AddLevelNumber(levelNumber);
            }
                            
            for (int y = 0; y < 23; y++) {
                var line = mapContainer[y];
                for (int x = 0; x < 40; x++) {
                    if (platforms.Contains(line[x])) {
                        
                        platformDictionary[line[x]].AddEntity(TranslateToEntity(x,y,line[x]));

                    }
                }
            }

            return platformDictionary;
        }
        
    }
}