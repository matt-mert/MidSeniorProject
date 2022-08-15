using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Snake body length does not include the snake head.
    /// </summary>
    public class SnakeBodyController : ISnakeBodyController, IGameSystem, ISnakeMovementListener
    {
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly SnakeGameData _snakeGameData;
        private readonly SnakeHeadBlock _snakeHeadBlock;
        private readonly SnakeBlock.SnakeBlockPool _snakeBlockPool;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly IMap _map;
        private List<SnakeBlock> _spawnedBlocks = new List<SnakeBlock>();

        public SnakeBodyController(IOccupancyHandler occupancyHandler, SnakeGameData snakeGameData, SnakeHeadBlock snakeHeadBlock, SnakeBlock.SnakeBlockPool snakeBlockPool, IGameStateHandler gameStateHandler, IMap map)
        {
            _occupancyHandler = occupancyHandler;
            _snakeGameData = snakeGameData;
            _snakeHeadBlock = snakeHeadBlock;
            _snakeBlockPool = snakeBlockPool;
            _gameStateHandler = gameStateHandler;
            _map = map;
            _snakeHeadBlock.transform.position = Vector3.down * -50;
        }

        private void GenerateSnake()
        {
            SnakeBlock previousBlock = _snakeHeadBlock;
            _snakeHeadBlock.Respawn(_snakeGameData.startPosition);
            for (int i = 0; i < _snakeGameData.startLength; i++)
            {
                var position = _map.GetNextCoordinate(previousBlock.Coordinate, Direction.Down);
                var block = _snakeBlockPool.Spawn(position);
                _spawnedBlocks.Add(block);
                previousBlock.SetBehindBlock(block);
                previousBlock = block;
            }
        }

        public void ShiftSnakeData()
        {
            for (int i = 0; i < _spawnedBlocks.Count; i++)
            {
                var currentBlock = _spawnedBlocks[i];

                if (i == 0)
                {
                    var child = _snakeHeadBlock.transform.GetChild(0);

                    currentBlock.SetChildPosPrev(currentBlock.ChildPosCurrent);
                    currentBlock.SetChildPosCurrent(child.localPosition);

                    currentBlock.SetChildRotPrev(currentBlock.ChildRotCurrent);
                    currentBlock.SetChildRotCurrent(child.localRotation);
                }
                else
                {
                    var earlierBlock = _spawnedBlocks[i - 1];

                    currentBlock.SetChildPosPrev(currentBlock.ChildPosCurrent);
                    currentBlock.SetChildPosCurrent(earlierBlock.ChildPosPrev);

                    currentBlock.SetChildRotPrev(currentBlock.ChildRotCurrent);
                    currentBlock.SetChildRotCurrent(earlierBlock.ChildRotPrev);
                }

                currentBlock.ApplyShiftings();
            }
        }
        
        public void AddBlock()
        {
            SnakeBlock previousBlock = _snakeHeadBlock;
            while (previousBlock.HasBehindBlock()) previousBlock = previousBlock.GetBehindBlock();
            var position = _map.GetNextCoordinate(previousBlock.Coordinate, Direction.Down);
            var block = _snakeBlockPool.Spawn(position);
            _spawnedBlocks.Add(block);
            previousBlock.SetBehindBlock(block);
        }
        
        private void ClearSnake()
        {
            foreach (var block in _spawnedBlocks)
            {
                _occupancyHandler.ClearOccupancy(block.Coordinate);
                _snakeBlockPool.Despawn(block);
            }
            _spawnedBlocks.Clear();
        }

        public void StartSystem()
        {
            GenerateSnake();
        }

        public void StopSystem()
        {
            
        }

        public void ClearSystem()
        {
            ClearSnake();
            _snakeHeadBlock.transform.position = Vector3.down * -50;
        }

        public void BeforeSnakeMove(Vector3Int currentPosition, Vector3Int targetPosition)
        {

        }

        public void AfterSnakeMove(Vector3Int previousPosition, Vector3Int currentPosition)
        {
            ShiftSnakeData();
        }

        public List<SnakeBlock> GetSpawnedBlocks() => _spawnedBlocks;

    }
}