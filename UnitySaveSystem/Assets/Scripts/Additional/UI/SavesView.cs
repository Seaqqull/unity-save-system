using SaveSystem.Additional.Actions;
using System.Collections.Generic;
using SaveSystem.Data;
using System.Linq;
using UnityEngine;


namespace SaveSystem.Additional.UI
{
    public class SavesView : MonoBehaviour
    {
        [SerializeField] private string _worldName = World.WORLD_NAME;
        [SerializeField] private CallbackActionSO _onAccept;
        [SerializeField] private ObjectSelector _selector;
        [Space]
        [SerializeField] private AcceptRejectButton _savePrefab;
        [SerializeField] private GameObject _container;

        private List<AcceptRejectButton> _saveButtons = new();
        

        private void OnEnable()
        {
            for(int i = 0; i < SaveManager.Instance.Snapshots.Count; i++)
            {
                var snapshotIndex = i;
                var snapshot = SaveManager.Instance.Snapshots.ElementAt(i);
                
                var hasNeededWorld = snapshot.Data.
                    Any(snapshot => (snapshot as WorldSnap)?.Worlds.Any(world => world.Equals(_worldName)) ?? false);
                if (!hasNeededWorld)
                    continue;
                
                
                var saveView = Instantiate(_savePrefab, _container.transform);
                    _saveButtons.Add(saveView);

                saveView.Id = (snapshotIndex + 1).ToString();
                saveView.Title = snapshot.Title;

                var selectedSnapshot = snapshotIndex;
                saveView.OnAcceptAction += () =>
                {
                    World.Instance.Clear();
                    SaveManager.Instance.Load(snapshotIndex);

                    _onAccept.Do(SaveManager.Instance, () => { });
                };
                saveView.OnRejectAction += () =>
                {
                    SaveManager.Instance.DeleteSnapshot(selectedSnapshot);
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
    }
}