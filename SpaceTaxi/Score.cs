using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace SpaceTaxi {
    public class Score {
        private Text display;
        private int score;

        public Score() {
            score = 0;
            display = new Text(score.ToString(), new Vec2F(0.01f, -0.25f), new Vec2F(0.3f, 0.3f));
            display.SetColor(new Vec3I(60, 210, 60));
        }
        
        public void ResetScore() {
            score = 0;
        }
        
        public void AddPoints(int points) {
            score += points;
        }
        
        /// <summary>
        /// Renders the text in a green color.
        /// </summary>
        public void RenderScore() {
            display.SetText(string.Format("Score: {0}", score.ToString()));
            display.RenderText();
        }
    }
}