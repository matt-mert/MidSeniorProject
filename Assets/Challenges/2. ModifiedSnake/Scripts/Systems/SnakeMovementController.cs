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
        private readonly IBlockTypeHandler _blockTypeHandler;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly SnakeHeadBlock _snakeHeadBlock;
        private readonly ISnakeMovementListener[] _snakeMovementListeners;
        private readonly ISnakeBodyController _snakeBodyController;

        private CancellationTokenSource _src;
        //private bool _loopActive = false;
        private Direction _currentDirection;
        private int serialCounter;

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
            serialCounter = 0;
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
            if (CanMoveTo(currentPosition, nextPosition))
            {
                foreach (var snakeMovementListener in _snakeMovementListeners)
                {
                    snakeMovementListener.BeforeSnakeMove(currentPosition, nextPosition);
                }

                bool acceptToPort = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgeAccept) &&
                    _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePort);
                bool portToPlatform = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePort) &&
                    _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePlatform);
                bool platformToPort = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePlatform) &&
                    _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgePort);
                bool portToAccept = _blockTypeHandler.IsOfBlockType(currentPosition, BlockType.BridgePort) &&
                    _blockTypeHandler.IsOfBlockType(nextPosition, BlockType.BridgeAccept);

                if (acceptToPort)
                {
                    _snakeHeadBlock.RotateUpInDirection(_currentDirection);
                    //_snakeHeadBlock.SetTargetRotationActivity(Direction.Up);
                }
                else if (portToPlatform)
                {
                    _snakeHeadBlock.RotateDownInDirection(_currentDirection);
                    //_snakeHeadBlock.SetTargetRotationActivity(Direction.Down);
                }
                else if (platformToPort)
                {
                    _snakeHeadBlock.RotateDownInDirection(_currentDirection);
                    //_snakeHeadBlock.SetTargetRotationActivity(Direction.Down);
                }
                else if (portToAccept)
                {
                    _snakeHeadBlock.RotateUpInDirection(_currentDirection);
                    //_snakeHeadBlock.SetTargetRotationActivity(Direction.Up);
                }
                //else
                //{
                //    _snakeHeadBlock.SetTargetRotationActivity(Direction.None);
                //}

                //_snakeHeadBlock.Rotate(_currentDirection);

                _snakeHeadBlock.Move(nextPosition);

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
        }

        public bool SetActiveDirection(Direction direction)
        {
            if (_snakeHeadBlock.Coordinate.z > 0) return false;

            if (_map.Invert(direction) == _snakeHeadBlock.LastMovementDirection) return false;

            _currentDirection = direction;
            return true;
        }
    }
}