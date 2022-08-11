using Challenges._2._ModifiedSnake.Scripts.MonoBehaviours;
using UnityEditor;
using UnityEngine;

namespace Challenges._2._ModifiedSnake.Scripts.Editor
{
    [CustomEditor(typeof(GameManagerView))]
    public class GameManagerViewInspector : UnityEditor.Editor
    {
       
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameManagerView user = (GameManagerView) target;

            if (Application.isPlaying == false) return;
            if (GUILayout.Button("Start Game"))
            {
                user.StartPressed();
            }
            if (GUILayout.Button("End Game"))
            {
                user.StopPressed();
            }
           
        }
    }
}
