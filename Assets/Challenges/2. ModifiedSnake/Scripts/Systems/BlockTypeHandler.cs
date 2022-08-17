using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class BlockTypeHandler : IBlockTypeHandler, IGameSystem
    {
        private IMap _map;
        private Dictionary<Vector3Int, BlockType> _blockTypes;

        public BlockTypeHandler(IMap map)
        {
            _map = map;
            _blockTypes = new Dictionary<Vector3Int, BlockType>();
        }

        // Getter of the _blockTypes dict is mostly for debugging purposes.
        public Dictionary<Vector3Int, BlockType> GetBlockTypesDict() => _blockTypes;

        public void SetBlockType(Vector3Int coordinate, BlockType type)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (!_blockTypes.ContainsKey(coordinate)) _blockTypes.Add(coordinate, type);
            _blockTypes[coordinate] = type;
        }

        public void ClearBlockType(Vector3Int coordinate)
        {
            if (!_map.IsCoordinateValid(coordinate)) return;
            if (_blockTypes.ContainsKey(coordinate)) _blockTypes.Remove(coordinate);
        }

        public BlockType GetBlockType(Vector3Int coordinate)
        {
            return !_blockTypes.ContainsKey(coordinate) ? BlockType.Default : _blockTypes[coordinate];
        }

        public bool IsOfBlockType(Vector3Int coordinate, BlockType checkType)
        {
            return GetBlockType(coordinate) == checkType;
        }

        public Dictionary<Vector3Int, BlockType> GetBlockTypeDict() => _blockTypes;

        public void ClearAllBlockAllocations()
        {
            _blockTypes.Clear();
        }

        public void StartSystem()
        {

        }

        public void StopSystem()
        {

        }

        public void ClearSystem()
        {
            ClearAllBlockAllocations();
        }
    }
}