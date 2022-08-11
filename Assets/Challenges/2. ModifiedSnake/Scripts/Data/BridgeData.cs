using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "BridgeData", menuName = "SnakeImprovementChallenge/BridgeData")]
    public class BridgeData : ScriptableObject
    {
        public int bridgeLength;
        public int bridgeWidth;
        public Vector2Int bridgeLocation;
        public Direction bridgeDirection;
    }
}
