using System;
using System.Collections.Generic;
using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Periodically spawns a food on the map
    /// </summary>
    public class FoodGenerator : IFoodGenerator, IGameSystem, ISnakeMovementListener
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly FoodBlock.FoodBlockPool _foodBlockPool;
        private readonly IMap _map;
        private readonly ISnakeBodyController _snakeBodyController;
        private readonly IOccupancyHandler _occupancyHandler;
        private CancellationTokenSource _cts;
        private bool _running = false;
        private Dictionary<Vector3Int, FoodBlock> _spawnedBlocks;

        public FoodGenerator(SnakeGameData snakeGameData, ISnakeBodyController snakeBodyController, FoodBlock.FoodBlockPool foodBlockPool,
            IMap map, IOccupancyHandler occupancyHandler)
        {
            _snakeGameData = snakeGameData;
            _snakeBodyController = snakeBodyController;
            _foodBlockPool = foodBlockPool;
            _map = map;
            _occupancyHandler = occupancyHandler;
            _spawnedBlocks = new Dictionary<Vector3Int, FoodBlock>();
        }


        public void StartGeneration()
        {
            if (_running) return;
            _running = true;
            _cts = new CancellationTokenSource();
            Loop();
        }

        private async UniTask Loop()
        {
            while (true)
            {
                var randomTime = GetRandomTime();
                var isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(randomTime),cancellationToken: _cts.Token).SuppressCancellationThrow();
                if (isCancelled) return;
                var randomPosition = _map.GetRandomCoordinate();
                SpawnFoodIfPossible(randomPosition);
            }
        }

        private void SpawnFoodIfPossible(Vector3Int randomPosition)
        {
            if (_spawnedBlocks.Count == _snakeGameData.maxSimultaneousFoods) return;
            
            if (_occupancyHandler.GetOccupancy(randomPosition) == OccupancyType.None)
            {
                var block = _foodBlockPool.Spawn(randomPosition);
                _spawnedBlocks.Add(randomPosition,block);
                _occupancyHandler.SetOccupied(randomPosition, OccupancyType.Food);
            }
        }

        private void ClearFoods()
        {
            foreach (var block in _spawnedBlocks)
            {
                _foodBlockPool.Despawn(block.Value);
            }
            _spawnedBlocks.Clear();
        }

        private float GetRandomTime()
        {
            return Random.Range(_snakeGameData.foodSpawnInterval.x, _snakeGameData.foodSpawnInterval.y);
        }

        public void StopGeneration()
        {
            if (!_running) return;
            _running = false;
            _cts.Cancel();
            _cts = null;
        }

        public void StartSystem()
        {
            StartGeneration();
        }

        public void StopSystem()
        {
            StopGeneration();
        }

        public void ClearSystem()
        {
            ClearFoods();
        }

        public void BeforeSnakeMove(Vector3Int currentPosition, Vector3Int targetPosition)
        {
            var blockExists = _spawnedBlocks.ContainsKey(targetPosition);
            if (blockExists)
            {
                var block = _spawnedBlocks[targetPosition];
                _foodBlockPool.Despawn(block);
                _spawnedBlocks.Remove(targetPosition);
                _snakeBodyController.AddBlock();
            }
        }

        public void AfterSnakeMove(Vector3Int previousPosition, Vector3Int currentPosition)
        {

        }
    }
}
