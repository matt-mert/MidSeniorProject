using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Data
{
    [CreateAssetMenu(fileName = "SnakeGameData",menuName = "SnakeImprovementChallenge/SnakeGameData")]
    public class SnakeGameData : ScriptableObject
    {
        public Vector3Int mapSize;
        public float secondsPerTile;
        public Vector3Int startPosition;
        public int startLength;
        public int maxSimultaneousFoods;
        public Vector2 foodSpawnInterval;
        public BridgeData[] bridgesData;
    }
}
