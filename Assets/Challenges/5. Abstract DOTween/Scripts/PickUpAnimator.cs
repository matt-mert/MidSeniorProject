using DG.Tweening;

namespace Challenges._6._Abstract_DOTween.Scripts
{
    
    public class PickUpAnimator : DoTweenAnimation
    {
        //Add parameters here
        
        /// <summary>
        /// Fill out this function
        /// </summary>
        /// <returns></returns>
        public override Tween StartPreview()
        {
            var sequence = DOTween.Sequence();
            return sequence;
        }
    }
}
