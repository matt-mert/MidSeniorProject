using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class AcceleratingState : GGStateBase<Vector2>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private Vector2 _inputVector;

        public AcceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }
        
        public override void Setup(Vector2 input)
        {
            _inputVector = input;
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            while ((_controller != null) && (_config != null))
            {
                var inputVector = _controller.GetInputVector();
                var movementVector = _controller.GetMovementVector();
                if (movementVector.sqrMagnitude < _config.MAXSpeed * _config.MAXSpeed)
                {
                    movementVector += new Vector3(inputVector.x, 0f, inputVector.y) * _config.AccelerationByTime;
                    movementVector *= _config.GeneralVelocityDamping;
                    _controller.SetMovementVector(movementVector);
                }
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
