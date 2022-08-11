using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "BridgeData", menuName = "SnakeImprovementChallenge/BridgeData")]
    public class BridgeData : ScriptableObject
    {
        public int bridgeLength;
        public int bridgeWidth;
        [Header("Top-left corner is (0,0) location.")]
        public Vector2Int bridgeLocation;
        [Header("None direction is not a valid option.")]
        public Direction bridgeDirection;
    }
}
