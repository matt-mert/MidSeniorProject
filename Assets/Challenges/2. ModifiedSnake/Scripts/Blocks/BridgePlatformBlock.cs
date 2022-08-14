using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The platform blocks of the bridge.
    /// </summary>
    public class BridgePlatformBlock : MonoBehaviour
    {
        public class BridgePlatformBlockPool : MonoMemoryPool<Vector3Int, BridgePlatformBlock>
        {
            protected override void OnSpawned(BridgePlatformBlock item)
            {
                base.OnSpawned(item);
            }

            protected override void OnDespawned(BridgePlatformBlock item)
            {
                base.OnDespawned(item);
                item._blockTypeHandler.ClearBlockType(item._position);
            }

            protected override void Reinitialize(Vector3Int p1, BridgePlatformBlock item)
            {
                base.Reinitialize(p1, item);
                item._position = p1;
                item.transform.position = item._map.ToWorldPosition(p1);
                item._blockTypeHandler.SetBlockType(item._position, BlockType.BridgePlatform);
            }
        }

        [Inject]
        protected readonly IBlockTypeHandler _blockTypeHandler;
        [Inject]
        protected readonly IMap _map;
        protected Vector3Int _position;

        public Vector3Int Position => _position;
    }
}
