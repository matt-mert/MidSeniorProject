using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// This is mainly implemented to have the food blocks exists in the scene
    /// </summary>
    public class FoodBlock : MonoBehaviour
    {
        public class FoodBlockPool : MonoMemoryPool<Vector3Int, FoodBlock>
        {
            // Called immediately after the item is removed from the pool
            protected override void OnSpawned(FoodBlock item)
            {
                base.OnSpawned(item);
            }

            // Called immediately after the item is FoodBlock to the pool
            protected override void OnDespawned(FoodBlock item)
            {
                base.OnDespawned(item);
                item._occupancyHandler.ClearOccupancy(item._position);
            }

            protected override void Reinitialize(Vector3Int p1, FoodBlock item)
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
        protected Vector3Int _position;

        public Vector3Int Position => _position;
        
    }
}