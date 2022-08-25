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
        private float _stepHeightLimit = 0.5f;

        public Vector2 GetInputVector() => _inputVector;
        public Vector3 GetMovementVector() => new Vector3(_movementVectorX, _movementVectorY, _movementVectorZ);
        public void SetMovementVectorX(float x) { _movementVectorX = x; }
        public void SetMovementVectorY(float y) { _movementVectorY = y; }
        public void SetMovementVectorZ(float z) { _movementVectorZ = z; }
        public float StepHeightLimit => _stepHeightLimit;

        //Add your states under this function
        private void SetupStateMachineStates()
        {
            _stateMachine.RegisterUniqueState(new AcceleratingState(this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new MovingState(this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new DeceleratingState(this, characterMovementConfig));
            _stateMachine.RegisterUniqueState(new FallingState(this, characterMovementConfig));
        }

        private void Update()
        {
            var movementVector = new Vector3(_movementVectorX, _movementVectorY, _movementVectorZ);
            transform.Translate(movementVector * Time.deltaTime);

            if (_stateMachine == null) return;

            var currentState = _stateMachine.GetCurrentState();

            if ((_inputVector != Vector2.zero) && (currentState.Identifier == "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.IdleState"))
            {
                _stateMachine.SwitchToState<AcceleratingState, Transform>(transform);
                Debug.Log("State has changed to AcceleratingState.");
            }
        }

        /*
        private async UniTask StateMachineController(CancellationToken token)
        {
            var isCancelled = false;

            _stateMachine.SwitchToState<IdleState>();

            while (!isCancelled)
            {
                

                var currentState = _stateMachine.GetCurrentState();

                bool isGrounded = false;

                var raycastHits = Physics.SphereCastAll(transform.position + new Vector3(0f, characterMovementConfig.CharacterHeight, 0f),
                    characterMovementConfig.CharacterRadius, Vector3.down, characterMovementConfig.CharacterHeight, LayerMask.GetMask("CharacterBlocker"));

                if (raycastHits.Length == 0) isGrounded = true;
                else
                {
                    for (int i = 0; i < raycastHits.Length; i++)
                    {
                        // Check if character is grounded
                        if (raycastHits[i].point.y <= transform.position.y) isGrounded = true;
                
                        // Check if object moving against a collider, apply reacting force
                        if (raycastHits[i].point.y > 0)
                        {
                            var posVec = raycastHits[i].point - transform.position;
                            var xzVec = new Vector3(posVec.x, 0f, posVec.z);
                            var dot = Vector3.Dot(_movementVector, xzVec.normalized);
                
                            if (raycastHits[i].point.y - transform.position.y > _stepHeightLimit)
                            {
                                _movementVector -= _movementVector * dot;
                            }
                            else
                            {
                                // Check if step is too steep or not
                                Ray stepCheck = new Ray(transform.position + new Vector3(0, _stepHeightLimit, 0), xzVec);
                                if (!Physics.Raycast(stepCheck, characterMovementConfig.CharacterRadius + 0.5f, LayerMask.GetMask("CharacterBlocker")))
                                {
                                    transform.Translate(new Vector3(transform.position.x, raycastHits[i].point.y, transform.position.z));
                                }
                            }
                        }
                    }
                }

                //if (!isGrounded && (currentState.Identifier != "FallingState")) _stateMachine.SwitchToState<FallingState>();

                //IdleState test = new IdleState();
                //Type type = test.GetType();
                //string str = type.ToString();

                switch (currentState.Identifier)
                {
                    case "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.IdleState":
                        if (_inputVector != Vector2.zero)
                        {
                            _stateMachine.SwitchToState<AcceleratingState, Vector2>(_inputVector);
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }
                        break;
                
                    case "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.AcceleratingState":
                        if (_movementVector.sqrMagnitude >= characterMovementConfig.MAXSpeed * characterMovementConfig.MAXSpeed)
                        {
                            _stateMachine.SwitchToState<MovingState, Vector2>(_inputVector);
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }

                        if (_inputVector == Vector2.zero)
                        {
                            _stateMachine.SwitchToState<DeceleratingState>();
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }
                        break;
                
                    case "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.MovingState":
                        if (_inputVector == Vector2.zero)
                        {
                            _stateMachine.SwitchToState<DeceleratingState>();
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }
                        break;
                
                    case "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.DeceleratingState":
                        if (_movementVector.sqrMagnitude <= 0.01f)
                        {
                            _movementVector = Vector3.zero;
                            _stateMachine.SwitchToState<IdleState>();
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }
                        break;
                
                    case "Challenges._1._GGStateMachineCharacterPhysics.Scripts.States.FallingState":
                        if (isGrounded)
                        {
                            _stateMachine.SwitchToState<IdleState>();
                        }
                        else
                        {
                            await UniTask.NextFrame();
                        }
                        break;
                
                    default:
                        isCancelled = await UniTask.NextFrame(token).SuppressCancellationThrow();
                        break;
                }
            }
        }

        */

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
