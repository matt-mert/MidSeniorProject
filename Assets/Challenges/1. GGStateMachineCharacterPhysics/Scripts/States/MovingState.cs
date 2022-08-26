using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class MovingState : GGStateBase<float, Transform>
    {
        private readonly float _staticWaitTime;
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private float _dynamicWaitTime;
        private Transform _characterTransform;

        public MovingState(float time, MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
        {
            _staticWaitTime = time;
            _controller = controller;
            _config = config;
        }

        public override void Setup(float time, Transform transform)
        {
            _dynamicWaitTime = time;
            _characterTransform = transform;
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
            while ((_controller != null) && (_config != null))
            {
                var inputVector = _controller.GetInputVector();
                if (inputVector == Vector2.zero)
                {
                    StateMachine.SwitchToState<DeceleratingState, float, Transform>(_dynamicWaitTime, _characterTransform);
                    Debug.Log("State has changed to DeceleratingState.");
                    return;
                }
                var resultVector = new Vector3(inputVector.x, 0f, inputVector.y) * _config.MAXSpeed / (_staticWaitTime + _dynamicWaitTime);

                var hits = Physics.SphereCastAll(_characterTransform.position + Vector3.up * _config.CharacterHeight,
                    _config.CharacterRadius, Vector3.down, _config.CharacterHeight + 0.1f, LayerMask.GetMask("CharacterBlocker"));
                var lowerHits = new List<Vector3>();
                var middleHits = new List<Vector3>();
                var higherHits = new List<Vector3>();
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].point.y <= _characterTransform.position.y)
                    {
                        lowerHits.Add(hits[i].point);
                    }
                    else if (hits[i].point.y > _characterTransform.position.y && hits[i].point.y <= _characterTransform.position.y + _controller.StepHeightLimit)
                    {
                        middleHits.Add(hits[i].point);
                    }
                    else if (hits[i].point.y > _characterTransform.position.y + _controller.StepHeightLimit)
                    {
                        higherHits.Add(hits[i].point);
                    }
                }
                
                if (lowerHits.Count > 0)
                {
                    var highest = FindHighestPoint(lowerHits);
                    _characterTransform.position = new Vector3(_characterTransform.position.x, highest.y, _characterTransform.position.z);
                }
                if (middleHits.Count > 0)
                {
                    var highest = FindHighestPoint(middleHits);
                    var checkingDist = 1 / Mathf.Tan(_controller.MaxStepAngleInRadians);
                    var ray = new Ray(highest + Vector3.up, resultVector);
                    bool isTooSteep = Physics.Raycast(ray, checkingDist, LayerMask.GetMask("CharacterBlocker"));
                    if (!isTooSteep)
                    {
                        _characterTransform.position = new Vector3(_characterTransform.position.x, highest.y, _characterTransform.position.z);
                    }
                    else
                    {

                        //resultVector = Vector3.zero;
                        Debug.Log("Too steep!");
                    }
                }
                if (higherHits.Count > 0)
                {
                    //resultVector = Vector3.zero;
                    Debug.Log("Wall detected!");
                }

                _controller.SetMovementVectorX(resultVector.x);
                _controller.SetMovementVectorZ(resultVector.z);
                _characterTransform.Translate(resultVector * (_staticWaitTime + _dynamicWaitTime));
                await UniTask.Delay(TimeSpan.FromSeconds(_staticWaitTime + _dynamicWaitTime), cancellationToken: cancellationToken);
                // await UniTask.NextFrame();
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
