using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class FallingState : GGStateBase<float, Vector3>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private float _deltaTime;
        private Vector3 _movementVectorBeforeFalling;

        public FallingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }

        public override void Setup(float time, Vector3 vector)
        {
            _deltaTime = time;
            _movementVectorBeforeFalling = vector;
        }

        private Vector3 FindHighestPoint(List<Vector3> list)
        {
            var highest = Vector3.zero;
            if (list.Count == 0)
            {
                return highest;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0) highest = list[i];
                    else highest = (list[i].y >= list[i - 1].y) ? list[i] : list[i - 1];
                }
                return highest;
            }
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            var movementVector = _movementVectorBeforeFalling;
            var charHeight = _config.CharacterHeight;
            var charRadius = _config.CharacterRadius;
            var stepLimit = _controller.StepHeightLimit;
            var gravity = _config.Gravity;
            var midAirXZDamp = _config.MidAirXZVelocityDamping;

            while ((_controller != null) && (_config != null))
            {
                var charPos = _controller.transform.position;
                movementVector += Vector3.down * gravity * _deltaTime;

                var hits = Physics.SphereCastAll(charPos + Vector3.up * charHeight,
                    charRadius / 2f, Vector3.down, charHeight - charRadius / 2f, LayerMask.GetMask("CharacterBlocker"));
                var groundHits = new List<Vector3>();

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].point.y < charPos.y + stepLimit)
                    {
                        groundHits.Add(hits[i].point);
                    }
                }

                if (groundHits.Count > 0)
                {
                    var highest = FindHighestPoint(groundHits);
                    _controller.transform.position = new Vector3(charPos.x, highest.y, charPos.z);
                    StateMachine.SwitchToState<IdleState>();
                    Debug.Log("State has changed to IdleState.");
                    return;
                }

                movementVector.x *= midAirXZDamp;
                movementVector.z *= midAirXZDamp;
                _controller.transform.transform.Translate(movementVector * _deltaTime);
                await UniTask.Delay(TimeSpan.FromSeconds(_deltaTime), cancellationToken: cancellationToken);
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
