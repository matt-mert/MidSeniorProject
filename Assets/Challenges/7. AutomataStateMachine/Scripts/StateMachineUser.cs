using Challenges._3._AutomataStateMachine.Scripts.States;
using UnityEngine;
using Zenject;

namespace Challenges._3._AutomataStateMachine.Scripts
{
    public class StateMachineUser : MonoBehaviour
    {
        [SerializeField]
        private GameObject targetObject;
        
        [Inject]
        private AutomataStateMachineFactory _factory;

        private IAutomataStateMachine _stateMachine;
        private Material _cachedMaterial;
        public IAutomataStateMachine StateMachine => _stateMachine;
        void Start()
        {
            var targetRenderer = targetObject.GetComponent<Renderer>();
            _cachedMaterial = targetRenderer.sharedMaterial;
            var material = new Material(_cachedMaterial);
            targetRenderer.sharedMaterial = material;
            
            
            _stateMachine = _factory.Create();
            _stateMachine.RegisterUniqueState(new BlueState(targetObject, material))
                .RegisterUniqueState(new RedState(targetObject, material))
                .RegisterUniqueState(new GreenState(targetObject, material))
                .RegisterUniqueState(new MagentaState(targetObject, material));

            #region Define automatic transitions here

            
            // MagentaState to BlueState when B is pressed 
            _stateMachine.DefineTransition<MagentaState,BlueState>(new ExampleDefinition());
            
            // GreenState to BlueState when B is pressed
            _stateMachine.DefineTransition<GreenState,BlueState>(new ExampleDefinition());
            
            // BlueState to RedState when Random.value <=0.04 checked every frame
            _stateMachine.DefineTransition<BlueState,RedState>(new ExampleDefinition());
            
            // BlueState to GreenState when BlueState has been active for 3 seconds
            _stateMachine.DefineTransition<BlueState,GreenState>(new ExampleDefinition());
            
            // RedState to GreenState without condition 
            _stateMachine.DefineTransition<RedState,GreenState>(new ExampleDefinition());
            
            // AnyState to MagentaState every 10 seconds
            _stateMachine.DefineTransition<MagentaState>(new ExampleDefinition());
            
            // GreenState to MagentaState when the event SwitchToMagenta is called (Using SignalBus)
            _stateMachine.DefineTransition<GreenState,MagentaState>(new ExampleDefinition());
            
            // MagentaState to GreenState when both M and G buttons are being held down
            _stateMachine.DefineTransition<MagentaState,GreenState>(new ExampleDefinition());
            

            #endregion
            
            
            _stateMachine.StartStateMachine<BlueState>();
        }

        private void OnDestroy()
        {
            var targetRenderer = targetObject.GetComponent<Renderer>();
            Destroy(targetRenderer.sharedMaterial);
            targetRenderer.sharedMaterial = _cachedMaterial;
        }
    }
}
