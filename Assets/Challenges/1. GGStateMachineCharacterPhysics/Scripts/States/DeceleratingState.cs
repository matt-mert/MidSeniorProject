using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class DeceleratingState : GGStateBase<float, Vector3>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private float _deltaTime;
        private Vector3 _movementVectorBeforeDeceleration;

        public DeceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }

        public override void Setup(float time, Vector3 vector)
        {
            _deltaTime = time;
            _movementVectorBeforeDeceleration = vector;
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
            var isCancelled = _controller.CurrentStateCancelled;
            var movementVector = _movementVectorBeforeDeceleration;
            var deceleration = _config.AccelerationByTime;
            var charHeight = _config.CharacterHeight;
            var charRadius = _config.CharacterRadius;
            var noInputDamp = _config.NoInputVelocityDamping;
            var stepLimit = _controller.StepHeightLimit;
            var angleLimit = _controller.StepAngleLimit;

            while ((_controller != null) && (_config != null) && (!isCancelled))
            {
                var charPos = _controller.transform.position;

                var tempVector = movementVector;
                movementVector -= movementVector.normalized * deceleration * _deltaTime;

                if (movementVector.sqrMagnitude <= 0.1f || Vector3.Dot(movementVector, tempVector) < 0)
                {
                    StateMachine.SwitchToState<IdleState>();
                    Debug.Log("State has been changed to IdleState.");
                    return;
                }

                var groundHits = new List<Vector3>();
                
                var wallHits = new List<Vector3>();
                
                var hits = Physics.SphereCastAll(charPos + Vector3.up * charHeight,
                    charRadius / 3f, Vector3.down, charHeight, LayerMask.GetMask("CharacterBlocker"));
                
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].point.y < charPos.y + stepLimit)
                    {
                        groundHits.Add(hits[i].point);
                    }
                }
                
                hits = Physics.CapsuleCastAll(charPos, charPos + Vector3.up * charHeight, charRadius, movementVector, 0.2f, LayerMask.GetMask("CharacterBlocker"));
                
                for (int i = 0; i < hits.Length; i++)
                {
                    if ((hits[i].point.y > charPos.y + stepLimit) && (hits[i].point.y < charPos.y + charHeight))
                    {
                        wallHits.Add(hits[i].point);
                    }
                }

                hits = Physics.CapsuleCastAll(charPos + Vector3.up * (stepLimit + 0.1f), charPos + Vector3.up * (charHeight + 0.1f), 0.1f, movementVector, (charRadius / 3f) + 0.025f, LayerMask.GetMask("CharacterBlocker"));

                for (int i = 0; i < hits.Length; i++)
                {
                    wallHits.Add(hits[i].point);
                }

                if (groundHits.Count > 0)
                {
                    var highest = FindHighestPoint(groundHits);
                
                    _controller.transform.position = new Vector3(charPos.x, highest.y, charPos.z);
                }
                else
                {
                    StateMachine.SwitchToState<FallingState, float, Vector3>(_deltaTime, movementVector);
                    Debug.Log("State has been changed to FallingState.");
                    return;
                }
                
                if (wallHits.Count > 0)
                {
                    for (int i = 0; i < wallHits.Count; i++)
                    {
                        var xzDir = new Vector3(wallHits[i].x - charPos.x, 0f, wallHits[i].z - charPos.z);
                        movementVector -= Vector3.Project(movementVector, xzDir);
                    }
                }

                movementVector *= noInputDamp;
                _controller.transform.Translate(movementVector * _deltaTime);
                await UniTask.Delay(TimeSpan.FromSeconds(_deltaTime), cancellationToken: cancellationToken);
                isCancelled = _controller.CurrentStateCancelled;
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
