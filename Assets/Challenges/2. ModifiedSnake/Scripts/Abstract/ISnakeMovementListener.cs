using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// ISnakeMovementListener's are injected into the movement system, they'll be notified of the movement of the same
    /// </summary>
    public interface ISnakeMovementListener
    {
        void BeforeSnakeMove(Vector2Int currentPosition, Vector2Int targetPosition);
        void AfterSnakeMove(Vector2Int previousPosition, Vector2Int currentPosition);
    }
}