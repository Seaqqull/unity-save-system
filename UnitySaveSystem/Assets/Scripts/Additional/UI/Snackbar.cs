using System.Collections;
using UnityEngine;
using TMPro;


namespace SaveSystem.Additional.UI
{
    public class Snackbar : MonoBehaviour
    {
        [SerializeField] public float _showTime = 2.0f;
        [SerializeField] [Range(0.1f, 1.0f)] public float _animationTime = 0.3f;
        [Space] 
        [SerializeField] private TMP_Text _textField;
        [Header("Animations")]
        [SerializeField] private AnimationCurve _showCurve;
        [SerializeField] private AnimationCurve _hideCurve;

        private Transform _transform;

        public string Text
        {
            get => _textField.text;
            set => _textField.text = value;
        }


        private void Start()
        {
            _transform = transform;
            
            Destroy(gameObject, _showTime);
            StartCoroutine(AnimationRoutine());
        }

        private void OnDestroy()
        {
            StopCoroutine(AnimationRoutine());
        }

        
        private IEnumerator AnimationRoutine()
        {
            var wholeAnimationTime = _showTime * _animationTime;
            var singleAnimationTime = wholeAnimationTime * 0.5f;
            var animationTime = 0.0f;

            while (animationTime < singleAnimationTime)
            {
                animationTime += Time.deltaTime;
                UpdateScale(animationTime, singleAnimationTime, _showCurve);
                
                yield return null;
            }

            yield return new WaitForSeconds(_showTime - wholeAnimationTime);

            animationTime = 0.0f;
            
            while (animationTime < singleAnimationTime)
            {
                animationTime += Time.deltaTime;
                UpdateScale(animationTime, singleAnimationTime, _hideCurve);

                yield return null;
            }
        }
        
        private void UpdateScale(float animationTimeSpend, float animationTime, AnimationCurve curve)
        {
            var animationProgress = Mathf.Clamp01(animationTimeSpend / animationTime);
            var curveValue = curve.Evaluate(animationProgress);
            
            _transform.localScale = new Vector3(curveValue, curveValue, curveValue);
        }
    }
}