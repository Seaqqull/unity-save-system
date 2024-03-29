using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using UnityEngine;
using System.Linq;


namespace SaveSystem
{
    public class Location : SingleBehaviour<Location>, ISavable
    {
        private static readonly List<ISavable> _pendingItems = new();
        
        
        public static void AddGlobally(ISavable item)
        {
            if (Instance == null)
                _pendingItems.Add(item);
            else
                Instance.Add(item);
        }
        
        public static void RemoveGlobally(ISavable item)
        {
            if (Instance != null)
                Instance.Remove(item);
        }


        [SerializeField] private string _id;
        [SerializeField] private string _worldId;
        [Space]
        // So, in case of [false] the save will be performed in given cases:
        // - AddLocation: of _activeLocation (in case, if not null), before assigning a new one
        // - RemoveLocation: of  _activeLocation, before assigning null
        // - MakeSnap: of -activeLocation, if not null
        [SerializeField] private bool _continuousStateBackup;

        private List<SaveSnap> _itemSnapshots = new ();
        private List<ISavable> _items = new ();
        private LocationSnap _locationSnap;

        public bool ContinuousStateBackup => _continuousStateBackup;
        public string WorldId => _worldId;
        public string Id => _id;


        private void Start()
        {
            foreach (var item in _pendingItems)
                Add(item);
            _pendingItems.Clear();
            
            if (World.Instance != null)
                World.Instance.AddLocation(this);
        }

        private void OnDestroy()
        {
            if (World.Instance != null)
                World.Instance.RemoveLocation(this);
        }


        private void TryItemFromSnap(ISavable item)
        {
            var itemSnapshot = _locationSnap?.Items
                .SingleOrDefault(itemSnap => itemSnap.Id.Equals(item.Id));
            if (itemSnapshot == null)
                return;
            
            _locationSnap = _locationSnap.UpdateSnap(itemSnap => !itemSnap.Id.Equals(item.Id));
            item.FromSnap(itemSnapshot);
        }


        public void Add(ISavable item)
        {
            if (!_items.Contains(item))
                _items.Add(item);

            _itemSnapshots = _itemSnapshots.Where(itemSnap => !itemSnap.Id.Equals(item.Id)).ToList();
            _itemSnapshots.Add(item.MakeSnap());

            TryItemFromSnap(item);

            if (_continuousStateBackup)
                World.Instance.UpdateLocation(this);
        }

        public void Remove(ISavable item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        public void UpdateItem(SaveSnap existingItemSnap)
        {
            var itemSnapshot = _itemSnapshots.SingleOrDefault(itemSnap => itemSnap.Id.Equals(existingItemSnap.Id));
            if (itemSnapshot == null) return;

            _itemSnapshots = _itemSnapshots.Where(item => !item.Id.Equals(existingItemSnap.Id)).ToList();
            _itemSnapshots.Add(existingItemSnap);

            var locationItem = _items.SingleOrDefault(item => item.Id.Equals(existingItemSnap.Id));
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

            foreach (var locationItem in _items.Where(locationItem => locationItem != null))
                TryItemFromSnap(locationItem);

            if (_continuousStateBackup)
                World.Instance.UpdateLocation(this);
        }
        #endregion
    }
}