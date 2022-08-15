using System;
using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Generates the desired bridges in the beginning of the game.
    /// </summary>
    public class BridgeGenerator : IBridgeGenerator, IInitializable, IGameSystem
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly IBlockTypeHandler _blockTypeHandler;
        private readonly IMap _map;
        private readonly BridgePlatformBlock.BridgePlatformBlockPool _bridgePlatformBlockPool;
        private readonly BridgePortBlock.BridgePortBlockPool _bridgePortBlockPool;
        private Dictionary<Vector3Int, BridgePlatformBlock> _spawnedPlatforms;
        private Dictionary<Vector3Int, BridgePortBlock> _spawnedPorts;

        public BridgeGenerator(SnakeGameData snakeGameData, IOccupancyHandler occupancyHandler, IBlockTypeHandler blockTypeHandler, IMap map,
            BridgePlatformBlock.BridgePlatformBlockPool bridgePlatformBlockPool, BridgePortBlock.BridgePortBlockPool bridgePortBlockPool)
        {
            _snakeGameData = snakeGameData;
            _occupancyHandler = occupancyHandler;
            _blockTypeHandler = blockTypeHandler;
            _map = map;
            _bridgePlatformBlockPool = bridgePlatformBlockPool;
            _bridgePortBlockPool = bridgePortBlockPool;
            _spawnedPlatforms = new Dictionary<Vector3Int, BridgePlatformBlock>();
            _spawnedPorts = new Dictionary<Vector3Int, BridgePortBlock>();
        }

        public void GenerateBridges()
        {
            if (_snakeGameData.bridgesData.Length == 0)
            {
                Debug.Log("No bridges found, continuing game.");
                return;
            }

            foreach (BridgeData bridgeData in _snakeGameData.bridgesData)
            {
                SpawnBridgeIfPossible(bridgeData);
            }
        }

        private void SpawnBridgeIfPossible(BridgeData bridgeData)
        {
            var start = bridgeData.bridgeStartCoord;
            var end = bridgeData.bridgeStartCoord + _map.DirectionToVector(BridgeToDirection(bridgeData.bridgeDirection)) * bridgeData.bridgeLength;
            var betweens = GetCoordsBetween(start, end);

            if (!_map.IsCoordinateValid(start) || !_map.IsCoordinateValid(end))
            {
                Debug.Log("A bridge could not be spawned due to invalid start or end coordinates.");
                return;
            }

            if (bridgeData.bridgeLength < 3)
            {
                Debug.Log("A bridge is too short and could not be spawned.");
                return;
            }

            if ((_blockTypeHandler.GetBlockType(start + Vector3Int.forward) == BlockType.BridgePlatform) ||
                (_blockTypeHandler.GetBlockType(end + Vector3Int.forward) == BlockType.BridgePlatform))
            {
                Debug.Log("A bridge could not be spawned due to one of its ports falling under a platform.");
                return;
            }

            if ((_occupancyHandler.GetOccupancy(start) == OccupancyType.None) && 
                (_occupancyHandler.GetOccupancy(end) == OccupancyType.None) &&
                (_blockTypeHandler.GetBlockType(start) != BlockType.BridgePort) &&
                (_blockTypeHandler.GetBlockType(end) != BlockType.BridgePort))
            {

                // Spawn the bridge here.

                var entryPort = _bridgePortBlockPool.Spawn(start, BridgeToDirection(bridgeData.bridgeDirection));
                _spawnedPorts.Add(start, entryPort);
                _blockTypeHandler.SetBlockType(start, BlockType.BridgePort);
                _blockTypeHandler.SetBlockType(start + Vector3Int.forward, BlockType.BridgePort);
                var endPort = _bridgePortBlockPool.Spawn(end, _map.Invert(BridgeToDirection(bridgeData.bridgeDirection)));
                _spawnedPorts.Add(end, endPort);
                _blockTypeHandler.SetBlockType(end, BlockType.BridgePort);
                _blockTypeHandler.SetBlockType(end + Vector3Int.forward, BlockType.BridgePort);

                if (bridgeData.bridgeDirection == BridgeDirection.UpVertical)
                {
                    _blockTypeHandler.SetBlockType(start + Vector3Int.down, BlockType.BridgeAccept);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.left, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.right, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.up, BlockType.BridgeReject);

                    _blockTypeHandler.SetBlockType(end + Vector3Int.up, BlockType.BridgeAccept);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.left, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.right, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.down, BlockType.BridgeReject);
                }
                else if (bridgeData.bridgeDirection == BridgeDirection.RightHorizontal)
                {
                    _blockTypeHandler.SetBlockType(start + Vector3Int.left, BlockType.BridgeAccept);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.up, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.down, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(start + Vector3Int.right, BlockType.BridgeReject);

                    _blockTypeHandler.SetBlockType(end + Vector3Int.right, BlockType.BridgeAccept);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.up, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.down, BlockType.BridgeReject);
                    _blockTypeHandler.SetBlockType(end + Vector3Int.left, BlockType.BridgeReject);
                }

                for (int i = 0; i < betweens.Count; i++)
                {
                    if (!_spawnedPlatforms.ContainsKey(betweens[i]))
                    {
                        var platform = _bridgePlatformBlockPool.Spawn(betweens[i]);
                        _spawnedPlatforms.Add(betweens[i], platform);
                        _blockTypeHandler.SetBlockType(betweens[i], BlockType.BridgePlatform);
                        _occupancyHandler.SetOccupied(betweens[i], OccupancyType.None);
                    }
                }
            }
            else
            {
                Debug.Log("A bridge could not be spawned due to initial overlaps.");
                return;
            }
        }

        private Direction BridgeToDirection(BridgeDirection bd)
        {
            switch (bd)
            {
                case BridgeDirection.UpVertical:
                    return Direction.Up;
                case BridgeDirection.RightHorizontal:
                    return Direction.Right;
                default:
                    return Direction.None;
            }
        }

        /// <summary>
        /// This method does not add starting and ending points since
        /// they are the start and end ports of the bridge.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private List<Vector3Int> GetCoordsBetween(Vector3Int start, Vector3Int end)
        {
            var between = new List<Vector3Int>();
            if (start.x == end.x)
            {
                for (int i = start.y + 1; i < end.y; i++)
                {
                    between.Add(new Vector3Int(start.x, i, 1));
                }
            }
            else if (start.y == end.y)
            {
                for (int i = start.x + 1; i < end.x; i++)
                {
                    between.Add(new Vector3Int(i, start.y, 1));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, null);
            }

            return between;
        }

        public void ClearBridges()
        {
            foreach (var platform in _spawnedPlatforms)
            {
                _bridgePlatformBlockPool.Despawn(platform.Value);
            }
            _spawnedPlatforms.Clear();
            
            foreach (var port in _spawnedPorts)
            {
                _bridgePortBlockPool.Despawn(port.Value);
            }
            _spawnedPorts.Clear();
        }

        public void Initialize()
        {
            
        }

        public void StartSystem()
        {
            GenerateBridges();
        }

        public void StopSystem()
        {

        }

        public void ClearSystem()
        {
            ClearBridges();
        }
    }
}
