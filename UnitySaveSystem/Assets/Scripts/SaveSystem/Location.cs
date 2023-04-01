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

        private List<LocationItem> _locationItems = new ();
        private List<SaveSnap> _itemsSnapshot = new ();
        private LocationSnap _locationSnap;

        public string WorldId => _worldId;
        public string Id => _id;


        private void Start()
        {
            if (World.Instance != null)
                World.Instance.AddLocation(this);
        }


        public void AddItem(LocationItem newItem)
        {
            if (!_locationItems.Contains(newItem))
                _locationItems.Add(newItem);

            _itemsSnapshot = _itemsSnapshot.Where(item => !item.Id.Equals(newItem.Id)).ToList();
            _itemsSnapshot.Add(newItem.MakeSnap());

            // Check whether there exists some item snapshot
            if (_locationSnap?.Items.SingleOrDefault(item => item.Id.Equals(newItem.Id)) is LocationItemSnap itemSnapshot)
            {
                _locationSnap = _locationSnap.UpdateSnap((item => !item.Id.Equals(newItem.Id)));
                newItem.FromSnap(itemSnapshot);
            }

            World.Instance.UpdateLocation(this);
        }

        public void RemoveItem(LocationItem newItem)
        {
            if (_locationItems.Contains(newItem))
                _locationItems.Remove(newItem);
        }

        public void UpdateItem(SaveSnap existingItemSnap)
        {
            var itemSnapshot = _itemsSnapshot.SingleOrDefault(item => item.Id.Equals(existingItemSnap.Id));
            if (itemSnapshot == null) return;

            _itemsSnapshot = _itemsSnapshot.Where(item => !item.Id.Equals(existingItemSnap.Id)).ToList();
            _itemsSnapshot.Add(existingItemSnap);

            if (_locationItems.SingleOrDefault(item => item.Id.Equals(existingItemSnap.Id)) is LocationItem locationItem)
                locationItem.FromSnap(existingItemSnap);

            World.Instance.UpdateLocation(this);
        }

        #region Savable
        public SaveSnap MakeSnap()
        {
            return new LocationSnap(Id, _itemsSnapshot);
        }

        public void FromSnap(SaveSnap data)
        {
            _locationSnap = data as LocationSnap;
            if ((_locationSnap == null))
                return;

            foreach (var locationItem in _locationItems)
            {
                if (locationItem == null) continue;

                if (_locationSnap?.Items.SingleOrDefault(item => item.Id.Equals(locationItem.Id)) is LocationItemSnap itemSnapshot)
                {
                    _locationSnap = _locationSnap.UpdateSnap((item => !item.Id.Equals(locationItem.Id)));
                    locationItem.FromSnap(itemSnapshot);
                }
            }

            World.Instance.UpdateLocation(this);
        }
        #endregion
    }
}