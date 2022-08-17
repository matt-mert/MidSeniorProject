using Challenges._2._ModifiedSnake.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// The block type handler determines the block type of coordinates on the map.
    /// This is used to determine if the snake is about to enter the bridge from the correct way.
    /// </summary>
    public interface IBlockTypeHandler
    {
        Dictionary<Vector3Int, BlockType> GetBlockTypesDict();
        void SetBlockType(Vector3Int coordinate, BlockType type);
        void ClearBlockType(Vector3Int coordinate);
        BlockType GetBlockType(Vector3Int coordinate);
        bool IsOfBlockType(Vector3Int coordinate, BlockType checkType);
        Dictionary<Vector3Int, BlockType> GetBlockTypeDict();
        void ClearAllBlockAllocations();
    }
}