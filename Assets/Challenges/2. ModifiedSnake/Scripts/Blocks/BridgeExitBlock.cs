using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The exit block of the bridge.
    /// </summary>
    public class BridgeExitBlock : MonoBehaviour
    {
        public class BridgeExitBlockPool : MonoMemoryPool<Vector2Int, BridgeExitBlock>
        {
            protected override void OnSpawned(BridgeExitBlock item)
            {
                base.OnSpawned(item);
            }

            protected override void OnDespawned(BridgeExitBlock item)
            {
                base.OnDespawned(item);
                item._occupancyHandler.ClearOccupancy(item._position);
            }

            protected override void Reinitialize(Vector2Int p1, BridgeExitBlock item)
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
