using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class FallingState : GGStateBase<Transform>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private Transform _characterTransform;

        public FallingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
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
                var resultVector = new Vector3(inputVector.x, 0f, inputVector.y) * _config.MAXSpeed;
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
