using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class DeceleratingState : GGStateBase<Transform>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private Transform _characterTransform;

        public DeceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
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
                var movementVector = _controller.GetMovementVector();
                var resultVector = movementVector - movementVector.normalized * _config.AccelerationByTime;
                if ((movementVector.magnitude <= 0.1f) || (Vector3.Dot(movementVector, resultVector) < 0))
                {
                    StateMachine.SwitchToState<IdleState>();
                    _controller.SetMovementVectorX(0);
                    _controller.SetMovementVectorY(0);
                    _controller.SetMovementVectorZ(0);
                    Debug.Log("State has changed to IdleState.");
                    return;
                }
                _controller.SetMovementVectorX(resultVector.x);
                _controller.SetMovementVectorZ(resultVector.z);
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
