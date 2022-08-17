using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class Map : IMap
    {
        private SnakeGameData _snakeGameData;

        public Map(SnakeGameData snakeGameData)
        {
            _snakeGameData = snakeGameData;
        }

        public Vector3Int MapSize => _snakeGameData.mapSize;

        public bool IsCoordinateValid(Vector3Int coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < _snakeGameData.mapSize.x && coordinate.y >= 0 &&
                   coordinate.y < _snakeGameData.mapSize.y && coordinate.z >= 0 && coordinate.z < _snakeGameData.mapSize.z;
        }

        public Vector3Int DirectionToVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector3Int.up;
                case Direction.Right:
                    return Vector3Int.right;
                case Direction.Down:
                    return Vector3Int.down;
                case Direction.Left:
                    return Vector3Int.left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public Direction VectorToDirection(Vector3Int direction)
        {
            if (direction == Vector3Int.up)
            {
                return Direction.Up;
            }
            if (direction == Vector3Int.right)
            {
                return Direction.Right;
            }
            if (direction == Vector3Int.left)
            {
                return Direction.Left;
            }
            if (direction == Vector3Int.down)
            {
                return Direction.Down;
            }

            return Direction.None;
        }

        public Direction Invert(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public Vector3Int GetRandomCoordinate()
        {
            return new Vector3Int(Random.Range(0, _snakeGameData.mapSize.x), Random.Range(0, _snakeGameData.mapSize.y), 0);
        }
        
        /*
        public Vector3Int GetRandomCoordinate(List<OccupancyType> acceptedOccupancies, List<BlockType> acceptedBlocks)
        {
            var occupancyDict = _occupancyHandler.GetOccupancyDict();
            var blockTypeDict = _blockTypeHandler.GetBlockTypeDict();
            var availableCoords = new List<Vector3Int>();
            
            foreach (KeyValuePair<Vector3Int, OccupancyType> pair in occupancyDict)
            {
                if (acceptedOccupancies.Contains(pair.Value)) availableCoords.Add(pair.Key);
            }

            foreach (KeyValuePair<Vector3Int, BlockType> pair in blockTypeDict)
            {
                if (acceptedBlocks.Contains(pair.Value)) availableCoords.Add(pair.Key);
            }

            var random = new Random();
            int index = random.Next(availableCoords.Count);

            return availableCoords[index];
        }
        */

        public Vector3 ToWorldPosition(Vector3Int coordinate)
        {
            var worldHalfX = (MapSize.x + 1f) / 2f;
            var worldHalfY = (MapSize.y + 1f) / 2f;
            float yValue = coordinate.z * 0.7f;
            // About the magic number 0.7: Determined height of the bridge platforms.
            // About the magic number 0.184: Determined half height of snake blocks.

            return new Vector3(coordinate.x-worldHalfX+1, yValue, coordinate.y-worldHalfY+1);
        }

        public Vector3Int GetNextCoordinate(Vector3Int coordinate, Direction direction)
        {
            var delta = DirectionToVector(direction);
            var newPosition = coordinate + delta;
            newPosition.x = newPosition.x < 0 ? newPosition.x + _snakeGameData.mapSize.x : newPosition.x;
            newPosition.y = newPosition.y < 0 ? newPosition.y + _snakeGameData.mapSize.y : newPosition.y;
            newPosition.x = newPosition.x % _snakeGameData.mapSize.x;
            newPosition.y = newPosition.y % _snakeGameData.mapSize.y;
            return newPosition;
        }
    }
}
