using DG.Tweening;
using UnityEngine;

namespace Challenges._6._Abstract_DOTween.Scripts
{
    public abstract class AbstractPickUpScript : MonoBehaviour
    {
        [HideInInspector]
        public bool inPreview = false;

        public abstract Tween StartPreview();
        
        public virtual void BeforeStart(){}
        public virtual void AfterEnd(){}

    }
}
