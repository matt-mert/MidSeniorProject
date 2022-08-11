using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The occupancy handler marks coordinates on the map as 'occupied' alongside what type of occupancy it is.
    /// This is used to determine if the snake is about to move into a food block or onto itself
    /// </summary>
    public interface IOccupancyHandler
    {
        void SetOccupied(Vector2Int coordinate,OccupancyType type);
        void ClearOccupancy(Vector2Int coordinate);
        OccupancyType GetOccupancy(Vector2Int coordinate);
        bool IsOccupiedWith(Vector2Int coordinate, OccupancyType checkType);
        void ClearAllOccupancies();
    }
}