using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using System.Linq;


namespace SaveSystem
{
    public class World : SingleBehaviour<World>, ISavable
    {
        #region Constant
        public const string DEFAULT_WORLD_NAME = "Game";
        #endregion


        private Location _activeLocation;
        private Dictionary<string, List<SaveSnap>> _locationSnaps = new ();
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


        private List<SaveSnap> GetWorldLocations(string worldName)
        {
            if (_locationSnaps.TryGetValue(worldName ?? DEFAULT_WORLD_NAME, out var locationSnap))
                return locationSnap;

            locationSnap = new List<SaveSnap>();
            _locationSnaps.Add(worldName ?? DEFAULT_WORLD_NAME, locationSnap);

            return locationSnap;
        }


        public void Clear(string worldName = null)
        {
            if (worldName == null)
                _locationSnaps.Clear();
            else if (_locationSnaps.TryGetValue(worldName, out var snaps))
                snaps.Clear();
        }

        public void AddLocation(Location newLocation)
        {
            var worldName = string.IsNullOrEmpty(newLocation.WorldId) ? DEFAULT_WORLD_NAME : newLocation.WorldId;
            _activeLocation = newLocation;

            var locationSnap = GetWorldLocations(worldName).SingleOrDefault(location => location.Id.Equals(newLocation.Id));
            if (locationSnap != null)
                newLocation.FromSnap(locationSnap);

            _locationSnaps[worldName] = GetWorldLocations(worldName).Where(location => !location.Id.Equals(newLocation.Id)).ToList();
            // _locations.Add(newLocation.MakeSnap());
        }

        public void UpdateLocation(Location newLocation)
        {
            if (_activeLocation == newLocation)
            {
                var worldName = string.IsNullOrEmpty(newLocation.WorldId) ? DEFAULT_WORLD_NAME : newLocation.WorldId;

                _locationSnaps[worldName] = GetWorldLocations(worldName).Where(location => !location.Id.Equals(newLocation.Id)).ToList();
                _locationSnaps[worldName].Add(newLocation.MakeSnap());
                return;
            }

            AddLocation(newLocation);//?
        }


        #region Savable
        public SaveSnap MakeSnap()
        {
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

            for(int i = 0; i < worldData.Worlds.Count(); i++)
                _locationSnaps.Add(worldData.Worlds.ElementAt(i), worldData.Locations.ElementAt(i).ToList());

            if (_activeLocation != null)
            {
                var locationSnap = GetWorldLocations(_activeLocation.WorldId)
                    .SingleOrDefault(location => location.Id.Equals(_activeLocation.Id));
                if (locationSnap != null)
                {
                    _activeLocation.FromSnap(locationSnap);
                    // _locations = _locations.Where(location => !location.Id.Equals(_activeLocation.Id)).ToList();
                }
            }
        }
        #endregion
    }
}