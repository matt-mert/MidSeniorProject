using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The enter block of the bridge.
    /// </summary>
    public class BridgeEnterBlock : MonoBehaviour
    {
        public class BridgeEnterBlockPool : MonoMemoryPool<Vector2Int, BridgeEnterBlock>
        {
            protected override void OnSpawned(BridgeEnterBlock item)
            {
                base.OnSpawned(item);
            }

            protected override void OnDespawned(BridgeEnterBlock item)
            {
                base.OnDespawned(item);
                item._occupancyHandler.ClearOccupancy(item._position);
            }

            protected override void Reinitialize(Vector2Int p1, BridgeEnterBlock item)
            {
                base.Reinitialize(p1, item);
                item._position = p1;
                item.transform.position = item._map.ToWorldPosition(p1);
                item._occupancyHandler.SetOccupied(item._position, OccupancyType.Food);
            }
        }

        [Inject]
        protected readonly IOccupancyHandler _occupancyHandler;
        [Inject]
        protected readonly IMap _map;
        protected Vector2Int _position;

        public Vector2Int Position => _position;
    }
}
