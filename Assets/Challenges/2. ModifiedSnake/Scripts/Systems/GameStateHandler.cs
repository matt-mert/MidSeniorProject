using Challenges._2._ModifiedSnake.Scripts.Abstract;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class GameStateHandler : IGameStateHandler, IInitializable
    {
        private readonly SignalBus _signalBus;

        public GameStateHandler(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public int CurrentLevel { get; private set; } = 0;
        public int CurrentScore { get; private set; } = 0;

        public void AddScore(int score)
        {
            CurrentScore += score;
            if (CurrentScore > 50) SetLevelComplete();
        }

        public void SetLevelComplete()
        {
            CurrentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
            _signalBus.Fire(new LevelCompleteEvent());
        }

        public void SetLevelFailed()
        {
            _signalBus.Fire(new LevelFailedEvent());
        }

        public void ResetScore()
        {
            CurrentScore = 0;
        }

        public void Initialize()
        {
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        }
    }
}