using System.Threading;
using Challenges._1._GGStateMachineCharacterPhysics.Scripts.MonoBehaviours;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class AcceleratingState : GGStateBase<Transform>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private Transform _characterTransform;

        public AcceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }
        
        public override void Setup(Transform transform)
        {
            _characterTransform = transform;
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            while ((_controller != null) && (_config != null))
            {
                var inputVector = _controller.GetInputVector();
                if (inputVector == Vector2.zero)
                {
                    StateMachine.SwitchToState<DeceleratingState, Transform>(_characterTransform);
                    Debug.Log("State has changed to DeceleratingState.");
                    return;
                }
                var movementVector = _controller.GetMovementVector();
                if (movementVector.magnitude >= _config.MAXSpeed)
                {
                    StateMachine.SwitchToState<MovingState, Transform>(_characterTransform);
                    Debug.Log("State has changed to MovingState.");
                    return;
                }
                movementVector += new Vector3(inputVector.x, 0f, inputVector.y) * _config.AccelerationByTime;
                // set mov x and set mov z

                var isGrounded = false;
                var hits = Physics.SphereCastAll(_characterTransform.position + Vector3.up * _config.CharacterHeight,
                    _config.CharacterRadius, Vector3.down, _config.CharacterHeight, LayerMask.GetMask("CharacterBlocker"));
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].point.y <= _characterTransform.position.y)
                    {
                        isGrounded = true;
                    }
                    else if (hits[i].point.y <= _characterTransform.position.y + _controller.StepHeightLimit)
                    {
                        var obstacleDir = (hits[i].point - _characterTransform.position).normalized;
                        bool isTooSteep = Physics.Raycast(_characterTransform.position + Vector3.up * _controller.StepHeightLimit,
                            obstacleDir, _config.CharacterRadius + 0.5f, LayerMask.GetMask("CharacterBlocker"));
                        if (isTooSteep)
                        {
                            var blockingForceMagnitude = Vector3.Dot(movementVector, obstacleDir);
                            movementVector -= movementVector.normalized * blockingForceMagnitude;
                        }
                        else
                        {
                            _characterTransform.position = new Vector3(_characterTransform.position.x, hits[i].point.y, _characterTransform.position.z);
                        }
                    }
                    else
                    {
                        //var obstacleDir = (hits[i].point - _characterTransform.position).normalized;
                        //var blockingForceMagnitude = Vector3.Dot(movementVector, obstacleDir);
                        //movementVector -= movementVector.normalized * blockingForceMagnitude;
                    }
                }

                _controller.SetMovementVectorX(movementVector.x);
                _controller.SetMovementVectorZ(movementVector.z);

                //if (!isGrounded)
                //{
                //    StateMachine.SwitchToState<FallingState, Transform>(_characterTransform);
                //    Debug.Log("State has changed to FallingState.");
                //    return;
                //}

                await UniTask.NextFrame();
            }
        }

        public override async UniTask Exit(CancellationToken cancellationToken)
        {
            
        }

        public override void CleanUp()
        {

        }
    }
}
