using SaveSystem.Additional.Actions;
using System.Collections.Generic;
using SaveSystem.Data;
using System.Linq;
using UnityEngine;


namespace SaveSystem.Additional.UI
{
    public class SavesView : MonoBehaviour
    {
        [SerializeField] private string _locationId;
        [SerializeField] private CallbackActionSO _onAccept;
        [SerializeField] private ObjectSelector _selector;
        [Space]
        [SerializeField] private AcceptRejectButton _savePrefab;
        [SerializeField] private GameObject _container;

        private List<AcceptRejectButton> _saveButtons = new();
        

        private void OnEnable()
        {
            var snapshotIndex = 0;
            foreach (var snapshot in SaveManager.Instance.Snapshots)
            {
                var worlds = snapshot.Data.
                    Select(snapshot => snapshot as WorldSnap).Where(world => world != null);
                foreach (var world in worlds)
                {
                    if (world.Locations.
                            SingleOrDefault(locationSnap => locationSnap.Id.Equals(_locationId)) is LocationSnap location)
                    {
                        var saveView = Instantiate(_savePrefab, _container.transform);
                        _saveButtons.Add(saveView);

                        saveView.Id = (snapshotIndex + 1).ToString();
                        saveView.Title = snapshot.Title;

                        var selectedSnapshot = snapshotIndex;
                        saveView.OnAcceptAction += () =>
                        {
                            World.Instance.Clear();

                            _onAccept.Do(SaveManager.Instance, () =>
                            {
                                var savableObjects = _selector.Select();
                                foreach (var savableObject in savableObjects)
                                    savableObject.FromSnap(
                                        location.Items.SingleOrDefault(objectSnap =>
                                            objectSnap.Id.Equals(savableObject.Id))
                                    );
                            });
                        };
                        saveView.OnRejectAction += () =>
                        {
                            SaveManager.Instance.DeleteSnapshot(selectedSnapshot);
                            OnDisable();
                            OnEnable();
                        };
                    }
                }

                snapshotIndex++;
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