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
        private Dictionary<Vector3Int, BridgePlatformBlock> _spawnedPlatforms;
        private Dictionary<Vector3Int, BridgePortBlock> _spawnedPorts;

        public BridgeGenerator(SnakeGameData snakeGameData, IOccupancyHandler occupancyHandler, IMap map,
            BridgePlatformBlock.BridgePlatformBlockPool bridgePlatformBlockPool, BridgePortBlock.BridgePortBlockPool bridgePortBlockPool)
        {
            _snakeGameData = snakeGameData;
            _occupancyHandler = occupancyHandler;
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
                
                foreach (Vector3Int vector in GetCoordsBetween(start, end))
                {
                    _occupancyHandler.SetOccupied(vector, OccupancyType.None);
                }

                // Spawn the bridge here.
            }
            else
            {
                Debug.Log("A bridge could not be spawned due to overlaps.");
                return;
            }
        }

        private List<Vector3Int> GetCoordsBetween(Vector3Int start, Vector3Int end)
        {
            // This method specifically does not add starting and ending points
            // because they are bridge ports which are the elevation points.
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

        private Vector3Int BridgeDirToWorld(BridgeDirection dir)
        {
            if (dir == BridgeDirection.UpVertical) return Vector3Int.up;
            else if (dir == BridgeDirection.RightHorizontal) return Vector3Int.right;
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

        public void BeforeSnakeMove(Vector3Int currentPosition, Vector3Int targetPosition)
        {

        }

        public void AfterSnakeMove(Vector3Int previousPosition, Vector3Int currentPosition)
        {

        }
    }
}
