using Challenges._2._ModifiedSnake.Scripts.Abstract;
using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The enter block of the bridge.
    /// </summary>
    public class BridgePortBlock : MonoBehaviour
    {
        public class BridgePortBlockPool : MonoMemoryPool<Vector3Int, Direction, BridgePortBlock>
        {
            protected override void OnSpawned(BridgePortBlock item)
            {
                base.OnSpawned(item);
            }

            protected override void OnDespawned(BridgePortBlock item)
            {
                base.OnDespawned(item);
                item._blockTypeHandler.ClearBlockType(item._position);
            }

            protected override void Reinitialize(Vector3Int p1, Direction p2, BridgePortBlock item)
            {
                base.Reinitialize(p1, p2, item);
                item._position = p1;
                item._direction = p2;
                item.transform.position = item._map.ToWorldPosition(p1);
                item._blockTypeHandler.SetBlockType(item._position, BlockType.BridgePort);
                switch (p2)
                {
                    case Direction.Up:
                        item.transform.rotation = Quaternion.identity;
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.down, BlockType.BridgeAccept);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.left, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.right, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.up, BlockType.BridgeReject);
                        break;
                    case Direction.Right:
                        item.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.left, BlockType.BridgeAccept);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.up, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.down, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.right, BlockType.BridgeReject);
                        break;
                    case Direction.Down:
                        item.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.up, BlockType.BridgeAccept);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.left, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.right, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.down, BlockType.BridgeReject);
                        break;
                    case Direction.Left:
                        item.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.right, BlockType.BridgeAccept);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.up, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.down, BlockType.BridgeReject);
                        item._blockTypeHandler.SetBlockType(item.Position + Vector3Int.left, BlockType.BridgeReject);
                        break;
                    default:
                        break;
                }
            }
        }

        [Inject]
        protected readonly IBlockTypeHandler _blockTypeHandler;
        [Inject]
        protected readonly IMap _map;
        protected Vector3Int _position;
        protected Direction _direction;

        public Vector3Int Position => _position;
        public Direction Direction => _direction;
    }
}
