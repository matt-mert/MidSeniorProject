using Challenges._2._ModifiedSnake.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The occupancy handler marks coordinates on the map as 'occupied' alongside what type of occupancy it is.
    /// This is used to determine if the snake is about to move into a food block or onto itself
    /// </summary>
    public interface IOccupancyHandler
    {
        void SetOccupied(Vector3Int coordinate, OccupancyType type);
        void ClearOccupancy(Vector3Int coordinate);
        OccupancyType GetOccupancy(Vector3Int coordinate);
        bool IsOccupiedWith(Vector3Int coordinate, OccupancyType checkType);
        Dictionary<Vector3Int, OccupancyType> GetOccupancyDict();
        void ClearAllOccupancies();
    }
}