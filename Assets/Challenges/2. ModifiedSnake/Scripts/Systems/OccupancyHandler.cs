using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class OccupancyHandler : IOccupancyHandler, IGameSystem
    {
        private IMap _map;
        private Dictionary<Vector2Int,OccupancyType> _occupancy;

        public OccupancyHandler(IMap map)
        {
            _map = map;
            _occupancy = new Dictionary<Vector2Int, OccupancyType>();
        }
        public void SetOccupied(Vector2Int coordinate, OccupancyType type)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (!_occupancy.ContainsKey(coordinate)) _occupancy.Add(coordinate,type);
            _occupancy[coordinate] = type;
        }

        public void ClearOccupancy(Vector2Int coordinate)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (_occupancy.ContainsKey(coordinate)) _occupancy.Remove(coordinate);
        }

        public OccupancyType GetOccupancy(Vector2Int coordinate)
        {
            return !_occupancy.ContainsKey(coordinate) ? OccupancyType.None : _occupancy[coordinate];
        }

        public bool IsOccupiedWith(Vector2Int coordinate, OccupancyType checkType)
        {
            return GetOccupancy(coordinate) == checkType;
        }

        public void ClearAllOccupancies()
        {
            _occupancy.Clear();
        }

        public void StartSystem()
        {
            
        }

        public void StopSystem()
        {
            
        }

        public void ClearSystem()
        {
            ClearAllOccupancies();
        }
    }
}