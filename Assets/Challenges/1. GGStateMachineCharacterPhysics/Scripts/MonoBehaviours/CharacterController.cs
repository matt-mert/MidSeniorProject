using System;
using Challenges._1._GGStateMachineCharacterPhysics.Scripts.States;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using GGPlugins.GGStateMachine.Scripts.Data;
using GGPlugins.GGStateMachine.Scripts.Installers;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.MonoBehaviours
{
    [Serializable]
    public class CharacterMovementConfig
    {
        [SerializeField,Range(0,3)]
        private float characterRadius;
        [SerializeField,Range(0,8)]
        private float characterHeight;
        // u/s^2 = units per seconds squared
        [SerializeField,Range(0,20)][Tooltip("How quickly the character speeds up: u/s^2")]
        private float accelerationByTime;
        [SerializeField,Range(0,10)][Tooltip("Maximum speed: u/s")]
        private float maxSpeed;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame ")]
        private float generalVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when there's input")]
        private float withInputVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when there's no input")]
        private float noInputVelocityDamping;
        [SerializeField,Range(0.9f,1)][Tooltip("Multiplied with speed every frame only when the character is above ground")]
        private float midAirXZVelocityDamping;
        [SerializeField,Min(0)][Tooltip("u/s^2")]
        private float gravity;
        
        public float Gravity => gravity;

        public float MidAirXZVelocityDamping => midAirXZVelocityDamping;

        public float NoInputVelocityDamping => noInputVelocityDamping;

        public float WithInputVelocityDamping => withInputVelocityDamping;

        public float GeneralVelocityDamping => generalVelocityDamping;

        public float MAXSpeed => maxSpeed;

        public float AccelerationByTime => accelerationByTime;

        public float CharacterHeight => characterHeight;

        public float CharacterRadius => characterRadius;
    }

    [ExecuteAlways]
    public class CharacterController : MonoBehaviour, IInputListener
    {
        [SerializeField]
        private CharacterMovementConfig characterMovementConfig;
        [SerializeField]
        private Transform headTransform;
        
        private GGStateMachineFactory _ggStateMachineFactory;
        private IGGStateMachine _stateMachine;

        [Inject]
        public void Inject(GGStateMachineFactory ggStateMachineFactory)
        {
            _ggStateMachineFactory = ggStateMachineFactory;
        }
        private void Start()
        {
            if (!Application.isPlaying) return;
            CreateStateMachine();
            SetupStateMachineStates();
            foreach (var stateMachineUser in transform.GetComponentsInChildren<IStateMachineUser>())
            {
                stateMachineUser.SetStateMachine(_stateMachine);
            }
            _stateMachine.StartStateMachine<IdleState>();
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
                _stateMachine.RequestExit();
        }

        private void CreateStateMachine()
        {
            _stateMachine = _ggStateMachineFactory.Create();
            //We don't want the machine to leave a state and re-enter it.
            _stateMachine.SetSettings(new StateMachineSettings(true));
            _stateMachine.RegisterUniqueState(new FlowerEarnedState(transform, headTransform));
            _stateMachine.RegisterUniqueState(new IdleState());
        }

        #region EDIT

        // Note: Movement can also be implemented so that character slows down when moving on
        // an inclined ground, i.e. climbing a hill. However, the requirements document for
        // the controller does not provide any information regarding this point.

        // You should only need to edit in this region, you can add any variables you wish.

        private Vector2 _inputVector;
        private float _movementVectorX;
        private float _movementVectorY;
        private float _movementVectorZ;
        private float _staticWaitTime = 0.005f;
        private float _dynamicWaitTime = 0.005f;
        private float _stepHeightLimit = 1.5f;
        private float _maxStepAngleInRadians = 2 * Mathf.PI / 3;

        public Vector2 GetInputVector() => _inputVector;
        public Vector3 GetMovementVector() => new Vector3(_movementVectorX, _movementVectorY, _movementVectorZ);
        public void SetMovementVectorX(float x) { _movementVectorX = x; }
        public void SetMovementVectorY(float y) { _movementVectorY = y; }
        public void SetMovementVectorZ(float z) { _movementVectorZ = z; }
        public float StepHeightLimit => _stepHeightLimit;
        public float MaxStepAngleInRadians => _maxStepAngleInRadians;

        //Add your states under this function
        private void SetupStateMachineStates()
        {
            _stateMachine.RegisterUniqueState(new AcceleratingState(_staticWaitTime, this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new MovingState(_staticWaitTime, this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new DeceleratingState(_staticWaitTime, this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new FallingState(this, characterMovementConfig));
        }

        private void Update()
        {
            var movementVector = new Vector3(_movementVectorX, _movementVectorY, _movementVectorZ);
            //transform.Translate(movementVector * Time.deltaTime);

            if (_stateMachine == null) return;

            var currentState = _stateMachine.GetCurrentState();

            if ((_inputVector != Vector2.zero) && (currentState.Identifier == "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.IdleState"))
            {
                _stateMachine.SwitchToState<AcceleratingState, float, Transform>(_dynamicWaitTime, transform);
                Debug.Log("State has changed to AcceleratingState.");
            }
        }

        // EnqueueState
        
        // CharacterInput.cs will call this function every frame in Update. xzPlaneMovementVector specifies the current input.
        // Ex:
        // (W is pressed) -> (0,1) ;
        // (W and D) -> (1,1) ;
        // (W and S) -> (0,0) ;
        // (A and S) -> (-1,-1) ;
        // (A) -> (-1,0)

        public void SetCurrentMovement(Vector2 xzPlaneMovementVector)
        {
            _inputVector = xzPlaneMovementVector;
        }
        
        #endregion
        
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position,transform.up,characterMovementConfig.CharacterRadius);
            Handles.DrawWireDisc(transform.position+(transform.up*characterMovementConfig.CharacterHeight),transform.up,characterMovementConfig.CharacterRadius);
            for (int i = 0; i < 10; i++)
            {
                var angle = Mathf.PI*2 * ((i + 0f) / 10f);
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);
                var localPos = new Vector3(x, 0, y)*characterMovementConfig.CharacterRadius;
                var localTargetPos = localPos + Vector3.up * characterMovementConfig.CharacterHeight;
                Handles.DrawLine(transform.TransformPoint(localPos),transform.TransformPoint(localTargetPos));
            }
        }
    }
}
