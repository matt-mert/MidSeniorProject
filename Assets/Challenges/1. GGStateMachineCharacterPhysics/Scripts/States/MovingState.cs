using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._1._GGStateMachineCharacterPhysics.Scripts.States
{
    public class MovingState : GGStateBase<Transform>
    {
        private readonly MonoBehaviours.CharacterController _controller;
        private readonly MonoBehaviours.CharacterMovementConfig _config;
        private Transform _characterTransform;

        public MovingState(MonoBehaviours.CharacterController controller, MonoBehaviours.CharacterMovementConfig config)
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
                // set mov x and set mov z

                var hits = Physics.SphereCastAll(_characterTransform.position + Vector3.up * _config.CharacterHeight,
                    _config.CharacterRadius, Vector3.down, _config.CharacterHeight + 0.1f, LayerMask.GetMask("CharacterBlocker"));
                var lowerHits = new List<Vector3>();
                var middleHits = new List<Vector3>();
                var higherHits = new List<Vector3>();
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].point.y < _characterTransform.position.y)
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
                    Vector3 closestHit = lowerHits[0];
                    for (int i = 0; i < lowerHits.Count; i++)
                    {
                        var dist = lowerHits[i].y - _characterTransform.position.y;
                        if (i != 0)
                        {
                            var prev = lowerHits[i - 1].y - _characterTransform.position.y;
                            closestHit = (prev > dist) ? lowerHits[i] : lowerHits[i - 1];
                        }
                        else closestHit = lowerHits[i];
                    }

                    _characterTransform.position = new Vector3(_characterTransform.position.x, closestHit.y, _characterTransform.position.z);
                }
                if (middleHits.Count == 1)
                {
                    _characterTransform.position = new Vector3(_characterTransform.position.x, middleHits[0].y, _characterTransform.position.z);
                }
                if (higherHits.Count > 0)
                {

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
