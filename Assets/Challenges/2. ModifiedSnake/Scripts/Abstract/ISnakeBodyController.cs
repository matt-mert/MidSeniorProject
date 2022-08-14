using System.Collections.Generic;
using Challenges._2._ModifiedSnake.Scripts.Blocks;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// Snake body controller is responsible for generating the snake and adding more blocks to it
    /// </summary>
    public interface ISnakeBodyController
    {
        void AddBlock();

        List<SnakeBlock> GetSpawnedBlocks();
    }
}