using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "TunnelData", menuName = "SnakeImprovementChallenge/TunnelData")]
    public class TunnelData : ScriptableObject
    {
        public int tunnelLength;
        public int tunnelWidth;
        public Vector2Int tunnelLocation;
        public Direction tunnelDirection;
    }
}
