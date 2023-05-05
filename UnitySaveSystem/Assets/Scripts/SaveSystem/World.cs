using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using System.Linq;


namespace SaveSystem
{
    public class World : SingleBehaviour<World>, ISavable
    {
        #region Constants
        public const string DEFAULT_WORLD_NAME = "Game";
        #endregion


        private Dictionary<string, List<SaveSnap>> _locationSnaps = new ();
        private Location _activeLocation;

        private string LocationWorld =>
            string.IsNullOrEmpty(_activeLocation.WorldId) ? DEFAULT_WORLD_NAME : _activeLocation.WorldId;
        
        public string Id =>
            DEFAULT_WORLD_NAME;


        private void Start()
        {
            _locationSnaps.Add(DEFAULT_WORLD_NAME, new List<SaveSnap>());

            SaveManager.Instance.AddToSavable(this);
        }

        private void OnDestroy()
        {
            SaveManager.Instance.RemoveFromSavable(this);
        }


        private void BackupActiveLocation()
        {
            var worldName = LocationWorld;

            _locationSnaps[worldName] = GetLocations(worldName)
                .Where(location => !location.Id.Equals(_activeLocation.Id)).ToList();
            _locationSnaps[worldName].Add(_activeLocation.MakeSnap());
        }

        private List<SaveSnap> GetLocations(string worldName = null)
        {
            if (_locationSnaps.TryGetValue(worldName ?? DEFAULT_WORLD_NAME, out var locationSnap))
                return locationSnap;

            locationSnap = new List<SaveSnap>();
            _locationSnaps.Add(worldName ?? DEFAULT_WORLD_NAME, locationSnap);

            return locationSnap;
        }

        
        public void ClearAll()
        {
            foreach (var key in _locationSnaps.Keys)
                Clear(key);
        }

        public void Clear(string worldName = null)
        {
            if (_locationSnaps.TryGetValue(worldName ?? DEFAULT_WORLD_NAME, out var snaps))
                snaps.Clear();
        }

        public void AddLocation(Location newLocation)
        {
            if ((newLocation == _activeLocation) || (newLocation == null))
                return;
            
            if ((_activeLocation != null) && !_activeLocation.ContinuousStateBackup)
                BackupActiveLocation();
            
            _activeLocation = newLocation;

            var worldName = LocationWorld;
            var locationSnap = GetLocations(worldName)
                .SingleOrDefault(location => location.Id.Equals(newLocation.Id));
            if (locationSnap != null)
                newLocation.FromSnap(locationSnap);

            _locationSnaps[worldName] = GetLocations(worldName)
                .Where(location => !location.Id.Equals(newLocation.Id)).ToList();

            // Case: Load -> Instantly save
            // We need to make a snapshot of the current location (in case, if no items are added to it),
            // otherwise the location's snapshot will be empty.
            if (newLocation.ContinuousStateBackup)
                _locationSnaps[worldName].Add(newLocation.MakeSnap());
        }

        public void RemoveLocation(Location location)
        {
            if ((location == null) || (_activeLocation != location))
                return;
            
            if (!_activeLocation.ContinuousStateBackup)
                BackupActiveLocation();
            _activeLocation = null;
        }

        public void UpdateLocation(Location newLocation)
        {
            if (_activeLocation == newLocation)
                BackupActiveLocation();
            else
                AddLocation(newLocation);
        }


        #region Savable
        public SaveSnap MakeSnap()
        {
            if ((_activeLocation != null) && !_activeLocation.ContinuousStateBackup)
                BackupActiveLocation();
            
            return new WorldSnap(
                Id, 
                _locationSnaps.Keys, 
                _locationSnaps.Values.Select(pair => pair.ToArray()));
        }

        public void FromSnap(SaveSnap data)
        {
            if (data is not WorldSnap worldData)
                return;

            _locationSnaps.Clear();

            for(int i = 0; i < worldData.Worlds.Count(); i++)
                _locationSnaps.Add(worldData.Worlds.ElementAt(i), worldData.Locations.ElementAt(i).ToList());

            if (_activeLocation != null)
            {
                var locationSnap = GetLocations(_activeLocation.WorldId)
                    .SingleOrDefault(location => location.Id.Equals(_activeLocation.Id));
                if (locationSnap != null)
                {
                    _activeLocation.FromSnap(locationSnap);
                    // Additionally we could do:
                    // BackupActiveLocation();
                    // But it's handled by the location itself.
                }
            }
        }
        #endregion
    }
}