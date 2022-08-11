using Challenges._3._AutomataStateMachine.Scripts;
using Challenges._3._AutomataStateMachine.Scripts.States;
using UnityEditor;
using UnityEngine;

namespace Challenges._3._AutomataStateMachine.Editor
{
    [CustomEditor(typeof(StateMachineUser))]
    public class TesterEditor : UnityEditor.Editor
    {
       
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StateMachineUser user = (StateMachineUser) target;

            if (Application.isPlaying == false) return;
            if (GUILayout.Button("BlueState"))
            {
                user.StateMachine.SwitchToState<BlueState>();
            }
            if (GUILayout.Button("RedState"))
            {
                user.StateMachine.SwitchToState<RedState>();
            }
            if (GUILayout.Button("GreenState"))
            {
                user.StateMachine.SwitchToState<GreenState>();
            }
            if (GUILayout.Button("MagentaState"))
            {
                user.StateMachine.SwitchToState<MagentaState>();
            }
        }
    }
}
