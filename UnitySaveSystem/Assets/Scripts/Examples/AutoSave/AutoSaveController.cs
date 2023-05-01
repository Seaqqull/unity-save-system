using SaveSystem.Additional.UI;
using System.Collections;
using SaveSystem.Data;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;


namespace SaveSystem.Examples
{
    public class AutoSaveController : MonoBehaviour
    {
        #region Constants
        private const string SAVE_MESSAGE = "A new save have been made: \"<color=#9066c1>{0}</color>\"";
        private const float MAX_SAVE_TIME = 30.0f;
        private const float MIN_SAVE_TIME = 10.0f;
        #endregion

        
        [SerializeField] private int _maximumSavesCount = 10;
        [SerializeField, Range(MIN_SAVE_TIME, MAX_SAVE_TIME)] private float _savePeriod = 10.0f;
        [Space]
        [SerializeField] private SavesView _view;
        [Header("UI")] 
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _minText;
        [SerializeField] private TMP_Text _maxText;
        [Space]
        [SerializeField] private Transform _snackbarTransform;
        [SerializeField] private Snackbar _snackbar;

        private Coroutine _saveCoroutine;
        private int _saveCounter = 1;

        
        private void Awake()
        {
            _saveCoroutine = StartCoroutine(SaveRoutine());

            
            _minText.text = MIN_SAVE_TIME.ToString();
            _maxText.text = MAX_SAVE_TIME.ToString();

            _slider.minValue = MIN_SAVE_TIME;
            _slider.maxValue = MAX_SAVE_TIME;
            _slider.value = _savePeriod;
            
            _slider.onValueChanged.AddListener((newValue) => _savePeriod = newValue);
        }

        private void OnDestroy()
        {
            StopCoroutine(_saveCoroutine);
        }


        private void MakeSave()
        {
            var saveTitle = $"{_saveCounter++:0000} - {DateTime.Now:HH:mm:ss MM/dd/yyyy}";
            
            SaveManager.Instance.Save(SaveType.AutoSave, saveTitle);
            
            if (SaveManager.Instance.GetSnapshots(SaveType.AutoSave).Count > _maximumSavesCount)
                SaveManager.Instance.DeleteSnapshot(0, SaveType.AutoSave);

            var snackbar = Instantiate(_snackbar, _snackbarTransform);
            snackbar.Text = string.Format(SAVE_MESSAGE, saveTitle);
        }

        private IEnumerator SaveRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_savePeriod);
                
                MakeSave();
                _view.UpdateView();
            }
        }

        public void MakeAutoSave()
        {
            StopCoroutine(_saveCoroutine);

            MakeSave();
            _saveCoroutine = StartCoroutine(SaveRoutine());
        }
    }
}