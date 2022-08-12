using System;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
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

        public Vector2Int MapSize => _snakeGameData.mapSize;

        public bool IsCoordinateValid(Vector2Int coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < _snakeGameData.mapSize.x && coordinate.y >= 0 &&
                   coordinate.y < _snakeGameData.mapSize.y;
        }

        public Vector2Int DirectionToVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public Direction VectorToDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.up)
            {
                return Direction.Up;
            }
            if (direction == Vector2Int.right)
            {
                return Direction.Right;
            }
            if (direction == Vector2Int.left)
            {
                return Direction.Left;
            }
            if (direction == Vector2Int.down)
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
        
        public Vector2Int GetRandomCoordinate()
        {
            return new Vector2Int(Random.Range(0, _snakeGameData.mapSize.x),
                Random.Range(0, _snakeGameData.mapSize.y));
        }

        public Vector3 ToWorldPosition(Vector2Int coordinate)
        {
            var worldHalfX = (MapSize.x + 1f) / 2f;
            var worldHalfY = (MapSize.y + 1f) / 2f;
            return new Vector3(coordinate.x-worldHalfX+1, 0, coordinate.y-worldHalfY+1);
        }

        public Vector2Int GetNextCoordinate(Vector2Int coordinate, Direction direction)
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
