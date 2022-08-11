using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._3._AutomataStateMachine.Scripts.States
{
    public class RedState : GGStateBase
    {
        private Transform _transform;
        private Material _material;
        private Tween _loop;

        public RedState(GameObject targetObject,Material material)
        {
            _transform = targetObject.transform;
            _material = material;
        }
        public override void Setup()
        {
        }

        public override async UniTask Entry(CancellationToken cancellationToken)
        {
            var seq = DOTween.Sequence();
            seq.Append(_transform.DOMove(Vector3.left, 1f));
            seq.Join(_transform.DOScale(Vector3.one*1.3f ,1f));
            seq.Join(_material.DOBlendableColor(Color.red, 1f));
            await seq.AsyncWaitForCompletion();
            _loop = _transform.DOScale(Vector3.one, 2f).SetLoops(-1, LoopType.Yoyo);
        }

        public override async UniTask Exit(CancellationToken cancellationToken)
        {
            _loop.Kill();
            var seq = DOTween.Sequence();
            seq.Append(_transform.DOMove(Vector3.zero, 0.5f));
            seq.Join(_transform.DOScale(Vector3.one,1f));
            seq.Join(_material.DOBlendableColor(Color.white, 1f));
            await seq.AsyncWaitForCompletion();
        }

        public override void CleanUp()
        {
        }
    }
}