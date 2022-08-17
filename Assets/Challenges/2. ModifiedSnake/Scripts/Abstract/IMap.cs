using Challenges._2._ModifiedSnake.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The Map interface provides a variety of map related functions
    /// </summary>
    public interface IMap
    {
        Vector3Int MapSize { get; }
        bool IsCoordinateValid(Vector3Int coordinate);
        Vector3Int GetNextCoordinate(Vector3Int coordinate, Direction direction);
        Vector3Int DirectionToVector(Direction direction);
        Direction VectorToDirection(Vector3Int direction);
        Direction Invert(Direction direction);
        Vector3Int GetRandomCoordinate();
        Vector3 ToWorldPosition(Vector3Int coordinate);
    }
}