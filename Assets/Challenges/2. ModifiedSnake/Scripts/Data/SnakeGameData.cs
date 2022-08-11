using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "SnakeGameData",menuName = "SnakeImprovementChallenge/SnakeGameData")]
    public class SnakeGameData : ScriptableObject
    {
        public Vector2Int mapSize;
        public float secondsPerTile;
        public Vector2Int startPosition;
        public int startLength;
        public Vector2 foodSpawnInterval;
        public BridgeData[] bridgesData;
        public TunnelData[] tunnelsData;
    }
}
