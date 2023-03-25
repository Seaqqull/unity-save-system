using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using System.Linq;


namespace SaveSystem
{
    public class World : SingleBehaviour<World>, ISavable
    {
        #region Constant
        private const string WORLD_NAME = "Game";
        #endregion
        
        private List<SaveSnap> _locationSnaps = new ();
        private Location _activeLocation;
        
        public string Id => 
            WORLD_NAME;


        private void Start()
        {
            SaveManager.Instance.AddToSavable(this);
        }

        private void OnDestroy()
        {
            SaveManager.Instance.RemoveFromSavable(this);
        }


        public void Clear()
        {
            _locationSnaps.Clear();
        }

        public void AddLocation(Location newLocation)
        {
            _activeLocation = newLocation;
            
            var locationSnap = _locationSnaps.SingleOrDefault(location => location.Id.Equals(newLocation.Id));
            if (locationSnap != null)
                newLocation.FromSnap(locationSnap);
            
            _locationSnaps = _locationSnaps.Where(location => !location.Id.Equals(newLocation.Id)).ToList();
            // _locations.Add(newLocation.MakeSnap());
        }
        
        public void UpdateLocation(Location newLocation)
        {
            if (_activeLocation == newLocation)
            {
                _locationSnaps = _locationSnaps.Where(location => !location.Id.Equals(newLocation.Id)).ToList();
                _locationSnaps.Add(newLocation.MakeSnap());
                return;
            }

            AddLocation(newLocation);//?
        }


        #region Savable
        public SaveSnap MakeSnap()
        {
            return new WorldSnap(Id, _locationSnaps);
        }

        public void FromSnap(SaveSnap data)
        {
            var worldData = data as WorldSnap;
            if ((worldData == null) || !worldData.Id.Equals(Id))
                return;
            
            _locationSnaps = worldData.Locations.ToList();
            if (_activeLocation != null)
            {
                var locationSnap = _locationSnaps.SingleOrDefault(location => location.Id.Equals(_activeLocation.Id));
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