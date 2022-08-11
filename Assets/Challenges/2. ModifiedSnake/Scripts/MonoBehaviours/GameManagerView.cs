using Challenges._2._ModifiedSnake.Scripts.Abstract;
using UnityEngine;
using Zenject;

namespace Challenges._2._ModifiedSnake.Scripts.MonoBehaviours
{
    public class GameManagerView : MonoBehaviour
    {
        [Inject]
        private IGameManager _gameManager;

        public void StartPressed()
        {
            _gameManager.StartGame();
        }

        public void StopPressed()
        {
            _gameManager.EndGame();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Start Game"))
            {
                StartPressed();
            }
            if (GUILayout.Button("End Game"))
            {
                StopPressed();
            }
        }
    }
}
