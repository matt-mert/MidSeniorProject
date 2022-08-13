using System;
using System.Collections.Generic;
using System.Linq;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class GameManager : IInitializable, IGameManager, IDisposable
    {
        private class GameSystemWrapper : IGameSystem
        {
            private IGameSystem _targetSystem;
            private int _state;

            public GameSystemWrapper(IGameSystem targetSystem)
            {
                _targetSystem = targetSystem;
                _state = 2;
            }

            public void StartSystem()
            {
                if (_state != 2) return;
                _targetSystem.StartSystem();
                _state = 0;
            }

            public void StopSystem()
            {
                if (_state != 0) return;
                _targetSystem.StopSystem();
                _state = 1;
            }

            public void ClearSystem()
            {
                if (_state != 1) return;
                _targetSystem.ClearSystem();
                _state = 2;
            }
        }
        
        private readonly SnakeGameData _snakeGameData;
        private readonly IMap _map;
        private readonly SignalBus _signalBus;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly IGameSystem[]_gameSystems; 

        public GameManager(SnakeGameData snakeGameData, IMap map, IGameSystem[] gameSystems,SignalBus signalBus,IGameStateHandler gameStateHandler)
        {
            _snakeGameData = snakeGameData;
            _map = map;
            _signalBus = signalBus;
            _gameStateHandler = gameStateHandler;
            _gameSystems = gameSystems.Select(x=>new GameSystemWrapper(x)).Cast<IGameSystem>().ToArray();
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<LevelCompleteEvent>(OnLevelComplete);
            _signalBus.Subscribe<LevelFailedEvent>(OnLevelFailed);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<LevelCompleteEvent>(OnLevelComplete);
            _signalBus.Unsubscribe<LevelFailedEvent>(OnLevelFailed);
        }

        public void StartGame()
        {
            ClearGame();
            _gameStateHandler.ResetScore();

            foreach (var system in _gameSystems)
            {
                system.StartSystem();
            }
        }

        public void EndGame()
        {
            foreach (var system in _gameSystems)
            {
                system.StopSystem();
            }
        }

        public void ClearGame()
        {
            foreach (var system in _gameSystems)
            {
                system.ClearSystem();
            }
        }

        public void RestartGame()
        {
            EndGame();
            ClearGame();
            StartGame();
        }

        private void OnLevelComplete()
        {
            EndGame();
            Debug.Log("LEVEL SUCCEED");
        }

        private void OnLevelFailed()
        {
            EndGame();
            Debug.Log("LEVEL FAILED");
        }
    }
}
