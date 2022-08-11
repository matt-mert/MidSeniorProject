using Challenges._2._ModifiedSnake.Scripts.Abstract;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.Systems
{
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField]
        private Camera levelCamera;

        [SerializeField]
        private Transform ground;

        [Inject]
        private IMap _map;

        private void Start()
        {
            var effective = Mathf.Max(_map.MapSize.x, _map.MapSize.y);
            levelCamera.transform.position = new Vector3(0, effective, -1f * _map.MapSize.y / 2f);
            levelCamera.transform.LookAt(ground.transform.position - Vector3.forward * _map.MapSize.y / 8f);
        }
    }
}