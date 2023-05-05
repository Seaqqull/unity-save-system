using SaveSystem.Additional.Actions;
using System.Collections.Generic;
using SaveSystem.Data;
using System.Linq;
using UnityEngine;


namespace SaveSystem.Additional.UI
{
    public class SavesView : MonoBehaviour
    {
        [SerializeField] private string _worldName = World.DEFAULT_WORLD_NAME;
        [SerializeField] private SaveType _saveType = SaveType.Ordinal;
        [SerializeField] private CallbackActionSO _onAccept;
        [Space]
        [SerializeField] private AcceptRejectButton _savePrefab;
        [SerializeField] private GameObject _container;

        private List<AcceptRejectButton> _saveButtons = new();
        

        private void OnEnable()
        {
            var snapshots = SaveManager.Instance.GetSnapshots(_saveType);
            var saveNumber = 1;
            
            for(int i = 0; i < snapshots.Count; i++)
            {
                var snapshotIndex = i;
                var snapshot = snapshots.ElementAt(i);
                
                var hasNeededWorld = snapshot.Data.
                    Any(snapshot => (snapshot as WorldSnap)?.Worlds.Any(world => world.Equals(_worldName)) ?? false);
                if (!hasNeededWorld)
                    continue;
                
                
                var saveView = Instantiate(_savePrefab, _container.transform);
                    _saveButtons.Add(saveView);

                saveView.Id = (saveNumber++).ToString();
                saveView.Title = snapshot.Title;

                var selectedSnapshot = snapshotIndex;
                saveView.OnAcceptAction += () =>
                {
                    World.Instance.ClearAll();
                    SaveManager.Instance.Load(snapshotIndex, _saveType);

                    _onAccept.Do(SaveManager.Instance, () => { });
                };
                saveView.OnRejectAction += () =>
                {
                    SaveManager.Instance.DeleteSnapshot(selectedSnapshot, _saveType);
                    OnDisable();
                    OnEnable();
                };
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _saveButtons.Count; i++)
                Destroy(_saveButtons[i].gameObject);
            
            _saveButtons.Clear();
        }


        public void UpdateView()
        {
            if (!gameObject.activeSelf)
                return;
            
            OnDisable();
            OnEnable();
        }
    }
}