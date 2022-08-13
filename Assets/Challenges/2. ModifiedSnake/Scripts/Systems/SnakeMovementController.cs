using System;
using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class SnakeMovementController : IGameSystem, IInputListener
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IMap _map;
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly SnakeHeadBlock _snakeHeadBlock;
        private readonly ISnakeMovementListener[] _snakeMovementListeners;

        private CancellationTokenSource _src;
        private bool _loopActive = false;
        private Direction _currentDirection;

        public SnakeMovementController(SnakeGameData snakeGameData, IMap map, IOccupancyHandler occupancyHandler, IGameStateHandler gameStateHandler, SnakeHeadBlock snakeHeadBlock, ISnakeMovementListener[] snakeMovementListeners)
        {
            _snakeGameData = snakeGameData;
            _map = map;
            _occupancyHandler = occupancyHandler;
            _gameStateHandler = gameStateHandler;
            _snakeHeadBlock = snakeHeadBlock;
            _snakeMovementListeners = snakeMovementListeners;
        }

        private async UniTask MovementLoop(CancellationToken token)
        {
            if (await UniTask.Delay(TimeSpan.FromSeconds(0.23f)).AttachExternalCancellation(token)
                .SuppressCancellationThrow()) return;
            bool isCancelled = false;
            float builtUpTime = 0f;
            while (!isCancelled)
            {
                builtUpTime += Time.deltaTime;
                if (builtUpTime >= _snakeGameData.secondsPerTile)
                {
                    RealizeMovement();

                    builtUpTime -= _snakeGameData.secondsPerTile;
                }

                isCancelled = await UniTask.NextFrame(token).SuppressCancellationThrow();
            }
        }

        private bool CanMoveTo(Vector3Int current, Vector3Int next)
        {
            bool occupiedWithSnake = _occupancyHandler.IsOccupiedWith(next, OccupancyType.SnakeBlock);
            bool occupiedWithDeadly = _occupancyHandler.IsOccupiedWith(next, OccupancyType.Deadly);
            bool failedToEnterBridge = (_occupancyHandler.IsOccupiedWith(current, OccupancyType.BridgeReject))
                && (_occupancyHandler.IsOccupiedWith(next, OccupancyType.BridgePort));
            if (occupiedWithSnake || occupiedWithDeadly || failedToEnterBridge) return false;
            else return true;
        }

        private void RealizeMovement()
        {
            var currentPosition = _snakeHeadBlock.Coordinate;
            var nextPosition = _map.GetNextCoordinate(_snakeHeadBlock.Coordinate, _currentDirection);
            if (CanMoveTo(currentPosition, nextPosition))
            {
                var previousPosition = _snakeHeadBlock.Coordinate;
                foreach (var snakeMovementListener in _snakeMovementListeners)
                {
                    snakeMovementListener.BeforeSnakeMove(previousPosition, nextPosition);
                }

                MoveInDirection(_currentDirection);
                foreach (var snakeMovementListener in _snakeMovementListeners)
                {
                    snakeMovementListener.AfterSnakeMove(previousPosition, nextPosition);
                }
            }
            else
            {
                _gameStateHandler.SetLevelFailed();
            }
        }

        private void MoveInDirection(Direction direction)
        {
            var nextPosition = _map.GetNextCoordinate(_snakeHeadBlock.Coordinate, direction);
            _snakeHeadBlock.Move(nextPosition);
        }

        public void StartSystem()
        {
            _src = new CancellationTokenSource();
            MovementLoop(_src.Token);
        }

        public void StopSystem()
        {
            _src?.Cancel();
            _src?.Dispose();
        }

        public void ClearSystem()
        {
            _currentDirection = Direction.Up;
        }

        public bool SetActiveDirection(Direction direction)
        {
            if (_map.Invert(direction) != _snakeHeadBlock.LastMovementDirection)
            {
                _currentDirection = direction;
                return true;
            }

            return false;
        }
    }
}