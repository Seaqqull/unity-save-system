using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using UnityEngine;
using System.Linq;


namespace SaveSystem
{
    public class Location : SingleBehaviour<Location>, ISavable
    {
        [SerializeField] private string _id;
        [SerializeField] private string _worldId;
        [Space]
        // So, in case of [false] the save will be performed in given cases:
        // - AddLocation: of _activeLocation (in case, if not null), before assigning a new one
        // - RemoveLocation: of  _activeLocation, before assigning null
        // - MakeSnap: of -activeLocation, if not null
        [SerializeField] private bool _continuousStateBackup;

        private List<LocationItem> _locationItems = new ();
        private List<SaveSnap> _itemSnapshots = new ();
        private LocationSnap _locationSnap;

        public bool ContinuousStateBackup => _continuousStateBackup;
        public string WorldId => _worldId;
        public string Id => _id;


        private void Start()
        {
            if (World.Instance != null)
                World.Instance.AddLocation(this);
        }

        private void OnDestroy()
        {
            if (World.Instance != null)
                World.Instance.RemoveLocation(this);
        }


        private void TryItemFromSnap(LocationItem locationItem)
        {
            var itemSnapshot = _locationSnap?.Items
                .SingleOrDefault(item => item.Id.Equals(locationItem.Id)) as LocationItemSnap;
            if (itemSnapshot == null)
                return;
            
            _locationSnap = _locationSnap.UpdateSnap((item => !item.Id.Equals(locationItem.Id)));
            locationItem.FromSnap(itemSnapshot);
        }


        public void AddItem(LocationItem newItem)
        {
            if (!_locationItems.Contains(newItem))
                _locationItems.Add(newItem);

            _itemSnapshots = _itemSnapshots.Where(item => !item.Id.Equals(newItem.Id)).ToList();
            _itemSnapshots.Add(newItem.MakeSnap());

            TryItemFromSnap(newItem);

            if (_continuousStateBackup)
                World.Instance.UpdateLocation(this);
        }

        public void RemoveItem(LocationItem newItem)
        {
            if (_locationItems.Contains(newItem))
                _locationItems.Remove(newItem);
        }

        public void UpdateItem(SaveSnap existingItemSnap)
        {
            var itemSnapshot = _itemSnapshots.SingleOrDefault(item => item.Id.Equals(existingItemSnap.Id));
            if (itemSnapshot == null) return;

            _itemSnapshots = _itemSnapshots.Where(item => !item.Id.Equals(existingItemSnap.Id)).ToList();
            _itemSnapshots.Add(existingItemSnap);

            var locationItem = _locationItems.SingleOrDefault(item => item.Id.Equals(existingItemSnap.Id));
            if (locationItem != null)
                locationItem.FromSnap(existingItemSnap);
            
            if (_continuousStateBackup)
                World.Instance.UpdateLocation(this);
        }

        #region Savable
        public SaveSnap MakeSnap()
        {
            // Special case, if not all data from [load] was processed by [item],
            // for that purpose we use [Concat].
            return new LocationSnap(Id, _itemSnapshots.Concat(_locationSnap?.Items ?? Enumerable.Empty<SaveSnap>()));
        }

        public void FromSnap(SaveSnap data)
        {
            _locationSnap = data as LocationSnap;
            if ((_locationSnap == null))
                return;

            foreach (var locationItem in _locationItems.Where(locationItem => locationItem != null))
                TryItemFromSnap(locationItem);

            if (_continuousStateBackup)
                World.Instance.UpdateLocation(this);
        }
        #endregion
    }
}