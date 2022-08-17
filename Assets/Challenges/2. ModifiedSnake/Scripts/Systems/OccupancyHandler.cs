using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class OccupancyHandler : IOccupancyHandler, IGameSystem
    {
        private IMap _map;
        private Dictionary<Vector3Int, OccupancyType> _occupancy;

        public OccupancyHandler(IMap map)
        {
            _map = map;
            _occupancy = new Dictionary<Vector3Int, OccupancyType>();
        }
        public void SetOccupied(Vector3Int coordinate, OccupancyType type)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (!_occupancy.ContainsKey(coordinate)) _occupancy.Add(coordinate, type);
            _occupancy[coordinate] = type;
        }

        public void ClearOccupancy(Vector3Int coordinate)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (_occupancy.ContainsKey(coordinate)) _occupancy.Remove(coordinate);
        }

        public OccupancyType GetOccupancy(Vector3Int coordinate)
        {
            return !_occupancy.ContainsKey(coordinate) ? OccupancyType.None : _occupancy[coordinate];
        }

        public bool IsOccupiedWith(Vector3Int coordinate, OccupancyType checkType)
        {
            return GetOccupancy(coordinate) == checkType;
        }

        public Dictionary<Vector3Int, OccupancyType> GetOccupancyDict() => _occupancy;

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