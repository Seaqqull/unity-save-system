using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;


namespace SaveSystem.Additional.UI
{
    public class AcceptRejectButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _index;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _rejectButton;

        public event Action OnAcceptAction;
        public event Action OnRejectAction;
        
        public string Title
        {
            get => _title.text;
            set => _title.text = value;
        }
        public string Id
        {
            get => _index.text;
            set => _index.text = value;
        }

        
        private void Start()
        {
            _acceptButton.onClick.AddListener(OnAccept);
            _rejectButton.onClick.AddListener(OnReject);
        }

        
        private void OnAccept() =>
            OnAcceptAction?.Invoke();

        private void OnReject() =>
            OnRejectAction?.Invoke();
    }
}
