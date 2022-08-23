using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class DeceleratingState : GGStateBase
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;

        public DeceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }

        public override void Setup()
        {
            
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            var movementVector = _controller.GetMovementVector();

            while (movementVector.sqrMagnitude < _config.MAXSpeed * _config.MAXSpeed)
            {
                movementVector -= new Vector3(movementVector.x, 0f, movementVector.z) * _config.AccelerationByTime;
                movementVector *= _config.GeneralVelocityDamping;

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
