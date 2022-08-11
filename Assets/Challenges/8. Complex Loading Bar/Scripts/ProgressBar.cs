using UnityEngine;

namespace Challenges._8._Complex_Loading_Bar.Scripts
{
    public class ProgressBar : MonoBehaviour
    {
        private const float IgnoreMargin = 0.0001f;
        private enum ProgressSnapOptions
        {
            SnapToLowerValue,
            SnapToHigherValue,
            DontSnap
        }

        [Header("Options")]
        [SerializeField]
        private float baseSpeed;
        [SerializeField]
        private ProgressSnapOptions snapOptions;

        [SerializeField]
        private RectTransform fillTransform;
        [SerializeField]
        private float minimumSpeed;

        private float _targetValue;
        private float? _currentSpeedOverride;

        private float CurrentValue => fillTransform.localScale.x;
        
        /// <summary>
        /// Sets the progress bar to the given normalized value instantly.
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        public void ForceValue(float value)
        {
            SetXScale(value);
            _targetValue = value;
            _currentSpeedOverride = null;
        }

        /// <summary>
        /// The progress bar will move to the given value
        /// </summary>
        /// <param name="value">Must be in range [0,1]</param>
        /// <param name="speedOverride">Will override the base speed if one is given</param>
        public void SetTargetValue(float value, float? speedOverride = null)
        {
            _targetValue = value;
            _currentSpeedOverride = speedOverride;
        }


        private void SetXScale(float value)
        {
            fillTransform.localScale = new Vector3(value, 1, 1);
        }
        private void Update()
        {
            var toTarget = _targetValue - CurrentValue;
            var distanceToTarget = Mathf.Abs(toTarget);
            if (distanceToTarget < IgnoreMargin)
            {
                ForceValue(_targetValue);
                return;
            }

            if (toTarget < 0 && snapOptions == ProgressSnapOptions.SnapToLowerValue)
            {
                ForceValue(_targetValue);
                return;
            }

            if (toTarget > 0 && snapOptions == ProgressSnapOptions.SnapToHigherValue)
            {
                ForceValue(_targetValue);
                return;
            }

            var delta = toTarget * Time.deltaTime * (_currentSpeedOverride ?? baseSpeed);
            if (Mathf.Abs(delta) < minimumSpeed) delta = minimumSpeed * Mathf.Sign(delta);
            if (CurrentValue + delta > _targetValue && snapOptions == ProgressSnapOptions.SnapToLowerValue)
            {
                ForceValue(_targetValue);
                return;
            }
            if (CurrentValue + delta < _targetValue && snapOptions == ProgressSnapOptions.SnapToHigherValue)
            {
                ForceValue(_targetValue);
                return;
            }
            SetXScale(CurrentValue+delta);
        }
    }
}
