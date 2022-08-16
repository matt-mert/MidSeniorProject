using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Challenges._6._Abstract_DOTween.Scripts
{
    public class PickUpAnimator : DoTweenAnimation
    {
        // Note: Loops inside animation sequences can also be implemented,
        // a set of animations in a sequence can be set loopable then.
        // I did not add loops as it was not a requirement.
        
        public enum Actions
        {
            MoveObject,
            ScaleObject
        }

        [Header("Complete sequence of the animation")]
        public List<ActionList> actionSequence;
        
        /// <summary>
        /// Fill out this function
        /// </summary>
        /// <returns></returns>
        public override Tween StartPreview()
        {
            var sequence = DOTween.Sequence();

            if (actionSequence.Count == 0) return null;

            for (int i = 0; i < actionSequence.Count; i++)
            {
                sequence.AppendInterval(actionSequence[i].delayBeforeSet);
                var actions = actionSequence[i].setOfActions;

                if (actions.Count == 0) continue;

                for (int j = 0; j < actions.Count; j++)
                {
                    var actionType = actions[j].actionType;
                    var objs = actions[j].animatedObjects;
                    var easeType = actions[j].actionEaseType;
                    var snapping = actions[j].isSnapping;
                    var direction = actions[j].actionDirection;
                    var duration = actions[j].actionDuration;

                    if (j == 0)
                    {
                        switch (actionType)
                        {
                            case Actions.MoveObject:
                                foreach (var obj in objs) sequence.Append(obj.DOLocalMove(direction, duration, snapping).SetEase(easeType));
                                continue;
                            case Actions.ScaleObject:
                                foreach (var obj in objs) sequence.Append(obj.DOScale(direction, duration).SetEase(easeType));
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
                        }
                    }

                    switch (actionType)
                    {
                        case Actions.MoveObject:
                            foreach (var obj in objs) sequence.Join(obj.DOLocalMove(direction, duration, snapping).SetEase(easeType));
                            continue;
                        case Actions.ScaleObject:
                            foreach (var obj in objs) sequence.Join(obj.DOScale(direction, duration).SetEase(easeType));
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
                    }
                }

                sequence.AppendInterval(actionSequence[i].delayAfterSet);
            }
            
            return sequence;
        }
    }

    [Serializable]
    public class ActionList
    {
        [Header("A set of actions will be executed simultaneously")]
        public float delayBeforeSet = 0;
        public List<Action> setOfActions;
        public float delayAfterSet = 0;
    }

    // Note: Tooltips can be added for each parameter.

    [Serializable]
    public class Action
    {
        [Header("(Not all properties are applicable for all actions)")]
        [Header("Choose the properties of this action")]
        public PickUpAnimator.Actions actionType;
        public Transform[] animatedObjects;
        public Ease actionEaseType = Ease.OutQuad;
        public bool isSnapping = false;
        public Vector3 actionDirection = Vector3.zero;
        public float actionDuration = 0;
    }
}
