namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The GameStateHandler is mostly used by game systems, they use it to let the game manager know to end the game
    /// </summary>
    public interface IGameStateHandler
    {
        int CurrentLevel { get; }
        void AddScore(int score);
        void SetLevelComplete();
        void SetLevelFailed();
        void ResetScore();
    }
}