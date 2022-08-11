using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The Map interface provides a variety of map related functions
    /// </summary>
    public interface IMap
    {
        Vector2Int MapSize { get; }
        bool IsCoordinateValid(Vector2Int coordinate);
        Vector2Int GetNextCoordinate(Vector2Int coordinate, Direction direction);
        Vector2Int DirectionToVector(Direction direction);
        Direction VectorToDirection(Vector2Int direction);
        Direction Invert(Direction direction);
        Vector2Int GetRandomCoordinate();
        Vector3 ToWorldPosition(Vector2Int coordinate);
    }
}