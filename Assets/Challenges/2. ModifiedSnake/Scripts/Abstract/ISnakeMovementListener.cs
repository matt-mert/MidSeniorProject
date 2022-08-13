using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Abstract
{
    /// <summary>
    /// ISnakeMovementListener's are injected into the movement system, they'll be notified of the movement of the same
    /// </summary>
    public interface ISnakeMovementListener
    {
        void BeforeSnakeMove(Vector3Int currentPosition, Vector3Int targetPosition);
        void AfterSnakeMove(Vector3Int previousPosition, Vector3Int currentPosition);
    }
}