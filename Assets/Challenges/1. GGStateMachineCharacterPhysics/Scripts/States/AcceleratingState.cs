using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class AcceleratingState : GGStateBase
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;

        public AcceleratingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _controller = controller;
            _config = config;
        }
        
        public override void Setup()
        {

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
            var movementVector = Vector3.zero;
            var sparseVector = Vector3.zero;
            var frameCounter = 10;
            var acceleration = _config.AccelerationByTime;
            var maxSpeed = _config.MAXSpeed;
            var charHeight = _config.CharacterHeight;
            var charRadius = _config.CharacterRadius;
            var withInputDamp = _config.WithInputVelocityDamping;
            var stepLimit = _controller.StepHeightLimit;
            var slopeLimit = _controller.MaxStepAngleInRadians;

            while ((_controller != null) && (_config != null) && (!isCancelled))
            {
                var inputVector = _controller.InputVector.normalized;
                var charPos = _controller.transform.position;

                if (inputVector == Vector2.zero)
                {
                    StateMachine.SwitchToState<DeceleratingState, Vector3>(sparseVector);
                    Debug.Log("State has been changed to DeceleratingState.");
                    return;
                }

                if (movementVector.sqrMagnitude >= maxSpeed * maxSpeed)
                {
                    StateMachine.SwitchToState<MovingState>();
                    Debug.Log("State has been changed to MovingState.");
                    return;
                }

                var newMagnitude = movementVector.magnitude + acceleration * 0.01f;
                movementVector = new Vector3(inputVector.x, 0f, inputVector.y).normalized * newMagnitude;

                var hits = Physics.SphereCastAll(charPos + Vector3.up * charHeight,
                    charRadius / 2f, Vector3.down, charHeight, LayerMask.GetMask("CharacterBlocker"));
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
                }
                //if (groundHits.Count > 0)
                //{
                //    var highest = FindHighestPoint(groundHits);
                //
                //    if (highest.y > charPos.y)
                //    {
                //        var checkingDist = 1f / Mathf.Tan(slopeLimit);
                //        var checkingRay = new Ray(highest + Vector3.up, movementVector);
                //        var isTooSteep = Physics.Raycast(checkingRay, checkingDist, LayerMask.GetMask("CharacterBlocker"));
                //        if (!isTooSteep)
                //        {
                //            _controller.transform.position = new Vector3(charPos.x, highest.y, charPos.z);
                //            Debug.Log("Step was not too steep. Stepping up.");
                //        }
                //        else
                //        {
                //            var xzDir = new Vector3(highest.x - charPos.x, 0, highest.z - charPos.z);
                //            movementVector -= Vector3.Project(movementVector, xzDir);
                //            Debug.Log("Step is too steep! Not stepping up.");
                //        }
                //    }
                //    else
                //    {
                //        _controller.transform.position = new Vector3(charPos.x, highest.y, charPos.z);
                //    }
                //}
                else
                {
                    StateMachine.SwitchToState<FallingState, Vector3>(sparseVector);
                    Debug.Log("State has been changed to FallingState.");
                    return;
                }

                //hits = Physics.SphereCastAll(charPos + Vector3.up * charHeight,
                //    charRadius + 0.1f, Vector3.down, charHeight, LayerMask.GetMask("CharacterBlocker"));
                //var wallHits = new List<Vector3>();
                //
                //for (int i = 0; i < hits.Length; i++)
                //{
                //    if ((hits[i].point.y >= charPos.y + stepLimit) && (hits[i].point.y < charPos.y + charHeight))
                //    {
                //        wallHits.Add(hits[i].point);
                //    }
                //}
                //
                //if (wallHits.Count > 0)
                //{
                //    for (int i = 0; i < wallHits.Count; i++)
                //    {
                //        var xzDir = new Vector3(wallHits[i].x - charPos.x, 0, wallHits[i].z - charPos.z);
                //        movementVector -= Vector3.Project(movementVector, xzDir);
                //    }
                //    Debug.Log("Wall detected!");
                //}

                //movementVector *= generalDamp * withInputDamp;

                if (frameCounter == 10)
                {
                    sparseVector = movementVector;
                    frameCounter = 0;
                }
                else
                {
                    frameCounter++;
                }

                movementVector *= withInputDamp;
                _controller.transform.Translate(movementVector * 0.01f);
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f), cancellationToken: cancellationToken).SuppressCancellationThrow();
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
