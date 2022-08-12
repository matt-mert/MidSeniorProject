using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The WallBlock bla bla.
    /// </summary>
    public class WallBlock : MonoBehaviour
    {
        public class WallBlockPool : MonoMemoryPool<Vector2Int, WallBlock>
        {
            // Called immediately after the item is returned to the pool
            protected override void OnDespawned(WallBlock item)
            {
                base.OnDespawned(item);
                item.OccupancyHandler.ClearOccupancy(item._coordinate);
                item.BehindBlock = null;
            }

            protected override void Reinitialize(Vector2Int p1, WallBlock item)
            {
                base.Reinitialize(p1, item);
                item._coordinate = p1;
                item.transform.position = new Vector3(item._coordinate.x, 0.5f, item._coordinate.y);
                item.OccupancyHandler.SetOccupied(item._coordinate, OccupancyType.SnakeBlock);
            }
        }

        [Inject]
        protected readonly IOccupancyHandler OccupancyHandler;
        [Inject]
        protected readonly IMap Map;
        [Inject]
        protected readonly SnakeGameData SnakeGameData;
        protected WallBlock BehindBlock;
        protected Vector2Int _coordinate;

        public Vector2Int Coordinate => _coordinate;
        public Direction LastMovementDirection { get; protected set; }

        public void SetBehindBlock(WallBlock wallBlock)
        {
            BehindBlock = wallBlock;
        }

        protected void SetPosition(Vector2Int targetCoordinate)
        {
            var previousPosition = _coordinate;
            OccupancyHandler.ClearOccupancy(_coordinate);
            _coordinate = targetCoordinate;
            //transform.position = new Vector3(_coordinate.x, 0.5f, _coordinate.y);
            transform.position = Map.ToWorldPosition(targetCoordinate);
            OccupancyHandler.SetOccupied(_coordinate, OccupancyType.SnakeBlock);
            var direction = Map.VectorToDirection(_coordinate - previousPosition);
            LastMovementDirection = direction == Direction.None ? Direction.Right : direction;
        }

        public bool HasBehindBlock()
        {
            return BehindBlock != null;
        }

        public WallBlock GetBehindBlock() => BehindBlock;

    }
}
