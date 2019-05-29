using NUnit.Framework;
using SpaceTaxi;

namespace SpaceTaxiTests {
    public class StateTypeTests {
        
        [Test]
        public void TestStringToState1() {
            Assert.AreEqual(StateTransformer.TransformStringToState("GAME_RUNNING"),
                GameStateType.GameRunning);
        }
        
        [Test]
        public void TestStringToState2() {
            Assert.AreEqual(StateTransformer.TransformStringToState("MAIN_MENU"),
                GameStateType.MainMenu);
        }
        
        [Test]
        public void TestStringToState3() {
            Assert.AreEqual(StateTransformer.TransformStringToState("GAME_PAUSED"),
                GameStateType.GamePaused);
        }
        
        [Test]
        public void TestStringToState4() {
            Assert.AreEqual(StateTransformer.TransformStringToState("LEVEL_SELECT"),
                GameStateType.LevelSelect);
        }
        
        [Test]
        public void TestStringToState5() {
            Assert.AreEqual(StateTransformer.TransformStringToState("GAME_OVER"),
                GameStateType.GameOver);
        }
        
        [Test]
        public void TestStateToString1() {
            Assert.AreEqual(StateTransformer.TransformStateToString(GameStateType.GameRunning),
                "GAME_RUNNING");
        }
        
        [Test]
        public void TestStateToString2() {
            Assert.AreEqual(StateTransformer.TransformStateToString(GameStateType.GamePaused),
                "GAME_PAUSED");
        }
        
        [Test]
        public void TestStateToString3() {
            Assert.AreEqual(StateTransformer.TransformStateToString(GameStateType.MainMenu),
                "MAIN_MENU");
        }
        
        [Test]
        public void TestStateToString4() {
            Assert.AreEqual(StateTransformer.TransformStateToString(GameStateType.LevelSelect),
                "LEVEL_SELECT");
        }
        
        [Test]
        public void TestStateToString5() {
            Assert.AreEqual(StateTransformer.TransformStateToString(GameStateType.GameOver),
                "GAME_OVER");
        }
        
    
    
    }
}