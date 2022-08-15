using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The SnakeBlock supports placement on the map,
    /// It will automatically handle Occupancy related functions.
    /// It also acts as a linked list node as it holds a reference to the snake block behind it.
    /// </summary>
    public class SnakeBlock : MonoBehaviour
    {
        public class SnakeBlockPool : MonoMemoryPool<Vector3Int, SnakeBlock>
        {
            // Called immediately after the item is returned to the pool
            protected override void OnDespawned(SnakeBlock item)
            {
                base.OnDespawned(item);
                item.OccupancyHandler.ClearOccupancy(item._coordinate);
                item.BehindBlock = null;
            }

            protected override void Reinitialize(Vector3Int p1, SnakeBlock item)
            {
                base.Reinitialize(p1, item);
                item._coordinate = p1;
                item.transform.position = new Vector3(item._coordinate.x, 0f, item._coordinate.y);
                item.transform.rotation = Quaternion.identity;
                item.OccupancyHandler.SetOccupied(item._coordinate,OccupancyType.SnakeBlock);
            }
        }

        [Inject]
        protected readonly IOccupancyHandler OccupancyHandler;
        [Inject]
        protected readonly IMap Map;
        [Inject]
        protected readonly SnakeGameData SnakeGameData;
        protected SnakeBlock BehindBlock;
        protected Vector3Int _coordinate;
        public Vector3Int Coordinate => _coordinate;
        public Direction LastMovementDirection { get; protected set; }

        public Vector3 ChildPosCurrent { get; protected set; }
        public Quaternion ChildRotCurrent { get; protected set; }
        public Vector3 ChildPosPrev { get; protected set; }
        public Quaternion ChildRotPrev { get; protected set; }


        public void Move(Vector3Int targetCoordinate)
        {
            var prevCoordinate = _coordinate;
            SetPosition(targetCoordinate);
            if (BehindBlock != null)
            {
                BehindBlock.Move(prevCoordinate);
            }
        }

        protected void SetPosition(Vector3Int targetCoordinate)
        {
            var previousPosition = _coordinate;
            OccupancyHandler.ClearOccupancy(_coordinate);
            _coordinate = targetCoordinate;
            transform.position = Map.ToWorldPosition(targetCoordinate);
            OccupancyHandler.SetOccupied(_coordinate, OccupancyType.SnakeBlock);
            var direction = Map.VectorToDirection(_coordinate - previousPosition);
            LastMovementDirection = direction == Direction.None ? Direction.Right : direction;
        }

        public virtual void ApplyShiftings()
        {
            var child = transform.GetChild(0);

            child.transform.localPosition = ChildPosPrev;
            child.transform.localRotation = ChildRotPrev;
        }

        public void SetChildPosition(Vector3 position)
        {
            var child = transform.GetChild(0);
            child.localPosition = position;
        }

        public void SetChildRotation(Quaternion rotation)
        {
            var child = transform.GetChild(0);
            child.localRotation = rotation;
        }

        public bool HasBehindBlock()
        {
            return BehindBlock != null;
        }

        public SnakeBlock GetBehindBlock() => BehindBlock;

        public void SetBehindBlock(SnakeBlock snakeBlock)
        {
            BehindBlock = snakeBlock;
        }

        public void SetChildPosCurrent(Vector3 position)
        {
            ChildPosCurrent = position;
        }

        public void SetChildRotCurrent(Quaternion rotation)
        {
            ChildRotCurrent = rotation;
        }

        public void SetChildPosPrev(Vector3 position)
        {
            ChildPosPrev = position;
        }

        public void SetChildRotPrev(Quaternion rotation)
        {
            ChildRotPrev = rotation;
        }
    }
}
