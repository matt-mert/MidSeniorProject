using System;
using System.Collections.Generic;
using System.Threading;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class SnakeMovementController : IGameSystem, IInputListener
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IMap _map;
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly IBlockTypeHandler _blockTypeHandler;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly SnakeHeadBlock _snakeHeadBlock;
        private readonly ISnakeMovementListener[] _snakeMovementListeners;
        private readonly ISnakeBodyController _snakeBodyController;

        private CancellationTokenSource _src;
        //private bool _loopActive = false;
        private Direction _currentDirection;
        private Vector3Int _previousPosition;

        public SnakeMovementController(SnakeGameData snakeGameData, IMap map, IOccupancyHandler occupancyHandler, IBlockTypeHandler blockTypeHandler,
            IGameStateHandler gameStateHandler, SnakeHeadBlock snakeHeadBlock, ISnakeMovementListener[] snakeMovementListeners, ISnakeBodyController snakeBodyController)
        {
            _snakeGameData = snakeGameData;
            _map = map;
            _occupancyHandler = occupancyHandler;
            _blockTypeHandler = blockTypeHandler;
            _gameStateHandler = gameStateHandler;
            _snakeHeadBlock = snakeHeadBlock;
            _snakeMovementListeners = snakeMovementListeners;
            _snakeBodyController = snakeBodyController;
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
            bool blockTypeIsDeadly = _blockTypeHandler.IsOfBlockType(next, BlockType.Deadly);
            bool failedToEnterBridge = (_blockTypeHandler.IsOfBlockType(current, BlockType.BridgeReject))
                && (_blockTypeHandler.IsOfBlockType(next, BlockType.BridgePort));
            if (occupiedWithSnake || blockTypeIsDeadly || failedToEnterBridge) return false;
            else return true;
        }

        private void RealizeMovement()
        {
            var currentPosition = _snakeHeadBlock.Coordinate;
            var nextPosition = _map.GetNextCoordinate(_snakeHeadBlock.Coordinate, _currentDirection);

            bool acceptToPort = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgeAccept) &&
                _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePort);

            bool portToAccept = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePort) &&
                _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgeAccept);

            bool portToPlatform = _blockTypeHandler.IsOfBlockType(_previousPosition, BlockType.BridgeAccept) &&
                _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePort);

            bool platformToPort = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePlatform) &&
                _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePort);

            if (portToPlatform) nextPosition += Vector3Int.forward;
            else if (platformToPort) nextPosition += Vector3Int.back;

            if (CanMoveTo(currentPosition, nextPosition))
            {
                foreach (var snakeMovementListener in _snakeMovementListeners)
                {
                    snakeMovementListener.BeforeSnakeMove(currentPosition, nextPosition);
                }

                if (acceptToPort)
                {
                    _snakeHeadBlock.RotateInDirection(_currentDirection);
                }
                else if (portToPlatform)
                {
                    _snakeHeadBlock.ResetRotation();
                }
                else if (platformToPort)
                {
                    _snakeHeadBlock.RotateInDirection(_map.Invert(_currentDirection));
                }
                else if (portToAccept)
                {
                    _snakeHeadBlock.ResetRotation();
                }

                _snakeHeadBlock.Move(nextPosition);

                _previousPosition = currentPosition;
                currentPosition = _snakeHeadBlock.Coordinate;
                nextPosition = _map.GetNextCoordinate(_snakeHeadBlock.Coordinate, _currentDirection);

                foreach (var snakeMovementListener in _snakeMovementListeners)
                {
                    snakeMovementListener.AfterSnakeMove(currentPosition, nextPosition);
                }
            }
            else
            {
                _gameStateHandler.SetLevelFailed();
            }
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
            _previousPosition = _snakeGameData.startPosition - _map.DirectionToVector(_currentDirection);
        }

        public bool SetActiveDirection(Direction direction)
        {
            var currentPosition = _snakeHeadBlock.Coordinate;
            var nextPosition = _map.GetNextCoordinate(currentPosition, direction);
            bool onBridgePort = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePort);
            bool onBridgePlatform = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePlatform);
            bool nextIsPlatform = _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePlatform);

            if (onBridgePort) return false;
            if (onBridgePlatform && !nextIsPlatform) return false;

            if (_map.Invert(direction) == _snakeHeadBlock.LastMovementDirection) return false;

            _currentDirection = direction;
            return true;
        }
    }
}