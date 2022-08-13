using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "BridgeData", menuName = "SnakeImprovementChallenge/BridgeData")]
    public class BridgeData : ScriptableObject
    {
        [Header("Bottom-left corner of the map is (0,0)")]
        public Vector3Int bridgeStartCoord;
        public BridgeDirection bridgeDirection;
        public int bridgeLength;
    }
}
