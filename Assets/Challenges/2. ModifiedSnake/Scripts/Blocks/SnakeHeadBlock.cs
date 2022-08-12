using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The Snake Head Block acts as a singleton, there can't be more than one.
    /// </summary>
    public class SnakeHeadBlock : SnakeBlock
    {
        public void Respawn(Vector2Int position)
        {
            SetPosition(position);
            LastMovementDirection = Direction.Up;
            BehindBlock = null;
        }
    }
}
