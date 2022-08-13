using System;
using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Blocks;
using Challenges._2._ModifiedSnake.Scripts.Data;
using Challenges._2._ModifiedSnake.Scripts.Systems;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    /// <summary>
    /// Generates the desired bridges in the beginning of the game.
    /// </summary>
    public class BridgeGenerator : IBridgeGenerator, IInitializable, IGameSystem, ISnakeMovementListener
    {
        private readonly SnakeGameData _snakeGameData;
        private readonly IOccupancyHandler _occupancyHandler;
        private readonly IMap _map;
        private readonly BridgePlatformBlock.BridgePlatformBlockPool _bridgePlatformBlockPool;
        private readonly BridgePortBlock.BridgePortBlockPool _bridgePortBlockPool;
        private Dictionary<Vector2Int, BridgePlatformBlock> _spawnedPlatforms;
        private Dictionary<Vector2Int, BridgePortBlock> _spawnedPorts;

        public BridgeGenerator(SnakeGameData snakeGameData, IOccupancyHandler occupancyHandler, IMap map,
            BridgePlatformBlock.BridgePlatformBlockPool bridgePlatformBlockPool, BridgePortBlock.BridgePortBlockPool bridgePortBlockPool)
        {
            _snakeGameData = snakeGameData;
            _occupancyHandler = occupancyHandler;
            _map = map;
            _bridgePlatformBlockPool = bridgePlatformBlockPool;
            _bridgePortBlockPool = bridgePortBlockPool;
            _spawnedPlatforms = new Dictionary<Vector2Int, BridgePlatformBlock>();
            _spawnedPorts = new Dictionary<Vector2Int, BridgePortBlock>();
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
            var end = bridgeData.bridgeStartCoord + BridgeDirToWorld(bridgeData.bridgeDirection) * bridgeData.bridgeLength;

            if (!_map.IsCoordinateValid(start) || !_map.IsCoordinateValid(end))
            {
                Debug.Log("Invalid start or end coordinates for a bridge.");
                return;
            }

            if (bridgeData.bridgeLength < 3)
            {
                Debug.Log("A bridge is too short and could not be spawned.");
                return;
            }

            if ((_occupancyHandler.GetOccupancy(start) == OccupancyType.None) && 
                (_occupancyHandler.GetOccupancy(end) == OccupancyType.None))
            {
                _occupancyHandler.SetOccupied(start, OccupancyType.BridgePort);
                _occupancyHandler.SetOccupied(end, OccupancyType.BridgePort);
                
                foreach (Vector2Int vector in GetCoordsBetween(start, end))
                {
                    _occupancyHandler.SetOccupied(vector, OccupancyType.BridgePlatform);
                }

                // Spawn the bridge here.
            }
            else
            {
                Debug.Log("A bridge could not be spawned due to overlaps.");
                return;
            }
        }

        private List<Vector2Int> GetCoordsBetween(Vector2Int start, Vector2Int end)
        {
            // This method specifically does not add starting and ending points.
            var between = new List<Vector2Int>();
            if (start.x == end.x)
            {
                for (int i = start.y + 1; i < end.y; i++)
                {
                    between.Add(new Vector2Int(start.x, i));
                }
            }
            else if (start.y == end.y)
            {
                for (int i = start.x + 1; i < end.x; i++)
                {
                    between.Add(new Vector2Int(i, start.y));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, null);
            }

            return between;
        }

        private Vector2Int BridgeDirToWorld(BridgeDirection dir)
        {
            if (dir == BridgeDirection.UpVertical) return Vector2Int.up;
            else if (dir == BridgeDirection.RightHorizontal) return Vector2Int.right;
            else throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
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
            GenerateBridges();
        }

        public void StartSystem()
        {

        }

        public void StopSystem()
        {

        }

        public void ClearSystem()
        {
            ClearBridges();
        }

        public void BeforeSnakeMove(Vector2Int currentPosition, Vector2Int targetPosition)
        {

        }

        public void AfterSnakeMove(Vector2Int previousPosition, Vector2Int currentPosition)
        {

        }
    }
}
