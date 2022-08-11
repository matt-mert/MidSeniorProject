namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The main manager responsible for starting/stopping the game
    /// </summary>
    public interface IGameManager
    {
        void StartGame();
        void EndGame();
        void RestartGame();
    }
}