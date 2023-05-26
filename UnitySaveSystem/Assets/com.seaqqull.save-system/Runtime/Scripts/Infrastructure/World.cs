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
        private bool _suspendLocationSnapshot;
        private Location _activeLocation;
        
        private string LocationWorld =>
            string.IsNullOrEmpty(_activeLocation.WorldId) ? DEFAULT_WORLD_NAME : _activeLocation.WorldId;
        private bool CanMakeLocationSnapshot =>
            !_activeLocation.ContinuousStateBackup && !_suspendLocationSnapshot;

        public string Id =>
            DEFAULT_WORLD_NAME;


        private void Start()
        {
            _locationSnaps.Add(DEFAULT_WORLD_NAME, new List<SaveSnap>());

            if (SaveManager.Instance != null)
                SaveManager.Instance.AddToSavable(this);
        }

        private void OnDestroy()
        {
            if (SaveManager.Instance != null)
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
            if (_activeLocation != null)
                _suspendLocationSnapshot = true;
            
            foreach (var key in _locationSnaps.Keys)
                Clear(key);
        }

        public void Clear(string worldName = null)
        {
            if ((_activeLocation != null) && (_activeLocation.WorldId == worldName))
                _suspendLocationSnapshot = true;
            
            if (_locationSnaps.TryGetValue(worldName ?? DEFAULT_WORLD_NAME, out var snaps))
                snaps.Clear();
        }

        public void AddLocation(Location newLocation)
        {
            if ((newLocation == _activeLocation) || (newLocation == null))
                return;
            
            if ((_activeLocation != null) && CanMakeLocationSnapshot)
                BackupActiveLocation();

            _suspendLocationSnapshot = false;
            _activeLocation = newLocation;

            var worldName = LocationWorld;
            var locationSnap = GetLocations(worldName)
                .SingleOrDefault(location => location.Id.Equals(_activeLocation.Id));
            if (locationSnap != null)
                _activeLocation.FromSnap(locationSnap);

            _locationSnaps[worldName] = GetLocations(worldName)
                .Where(location => !location.Id.Equals(_activeLocation.Id)).ToList();

            // Case: Load -> Instantly save
            // We need to make a snapshot of the current location (in case, if no items are added to it),
            // otherwise the location's snapshot will be empty.
            if (_activeLocation.ContinuousStateBackup)
                _locationSnaps[worldName].Add(_activeLocation.MakeSnap());
        }

        public void RemoveLocation(Location location)
        {
            if ((location == null) || (_activeLocation != location))
                return;
            
            if (CanMakeLocationSnapshot)
            {
                _suspendLocationSnapshot = false;
                BackupActiveLocation();
            }
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
            if ((_activeLocation != null) && CanMakeLocationSnapshot)
            {
                _suspendLocationSnapshot = false;
                BackupActiveLocation();
            }

            return new WorldSnap(
                Id, 
                _locationSnaps.Keys, 
                _locationSnaps.Values.Select(pair => pair.ToArray()));
        }

        public void FromSnap(SaveSnap data)
        {
            var worldData = data as WorldSnap;
            if (worldData == null)
                return;

            _locationSnaps.Clear();

            for(var i = 0; i < worldData.Worlds.Count(); i++)
                _locationSnaps.Add(worldData.Worlds.ElementAt(i), worldData.Locations.ElementAt(i).ToList());

            if (_activeLocation == null) return;
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
        #endregion
    }
}