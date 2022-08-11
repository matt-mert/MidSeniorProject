using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GGPlugins.GGStateMachine.Scripts.Abstract;
using UnityEngine;

namespace Challenges._3._AutomataStateMachine.Scripts.States
{
    public class GreenState : GGStateBase
    {
        private Transform _transform;
        private Material _material;
        private Tween _loop;

        public GreenState(GameObject targetObject,Material material)
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
            seq.Append(_transform.DOMove(Vector3.up, 1f));
            seq.Join(_transform.DOScale(Vector3.one ,1f));
            seq.Join(_transform.DORotateQuaternion(Quaternion.Euler(45,45,45) , 1f));
            seq.Join(_material.DOBlendableColor(Color.green, 1f));
            await seq.AsyncWaitForCompletion();
            _loop = _transform.DOBlendableRotateBy(new Vector3(0,60,0), 0.25f).SetLoops(-1, LoopType.Incremental);
        }

        public override async UniTask Exit(CancellationToken cancellationToken)
        {
            _loop.Kill();
            var seq = DOTween.Sequence();
            seq.Append(_transform.DOMove(Vector3.zero, 0.5f));
            seq.Join(_transform.DOScale(Vector3.one,1f));
            seq.Join(_transform.DORotateQuaternion(Quaternion.identity , 1f));
            seq.Join(_material.DOBlendableColor(Color.white, 1f));
            await seq.AsyncWaitForCompletion();
        }

        public override void CleanUp()
        {
        }
    }
}