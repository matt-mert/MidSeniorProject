using Challenges._2._ModifiedSnake.Scripts.Data;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Blocks
{
    /// <summary>
    /// The Snake Head Block acts as a singleton, there can't be more than one.
    /// </summary>
    public class SnakeHeadBlock : SnakeBlock
    {
        public void Respawn(Vector3Int position)
        {
            SetPosition(position);
            LastMovementDirection = Direction.Up;
            BehindBlock = null;
        }

        public override void ApplyShiftings()
        {
            // This method must only affect non-head snake blocks.
        }

        public void RotateInDirection(Direction direction)
        {
            var childTransform = transform.GetChild(0);
            switch (direction)
            {
                case Direction.Up:
                    childTransform.localEulerAngles = new Vector3(-35, 0, 0);
                    childTransform.localPosition = new Vector3(0, 0.5f, -0.1f);
                    break;
                case Direction.Right:
                    childTransform.localEulerAngles = new Vector3(0, 0, 35);
                    childTransform.localPosition = new Vector3(-0.1f, 0.5f, 0);
                    break;
                case Direction.Down:
                    childTransform.localEulerAngles = new Vector3(35, 0, 0);
                    childTransform.localPosition = new Vector3(0, 0.5f, 0.1f);
                    break;
                case Direction.Left:
                    childTransform.localEulerAngles = new Vector3(0, 0, -35);
                    childTransform.localPosition = new Vector3(0.1f, 0.5f, 0);
                    break;
                default:
                    break;
            }
        }

        public void ResetRotation()
        {
            var childTransform = transform.GetChild(0);
            childTransform.localPosition = new Vector3(0, 0.184f, 0);
            childTransform.localEulerAngles = Vector3.zero;
        }
    }
}
