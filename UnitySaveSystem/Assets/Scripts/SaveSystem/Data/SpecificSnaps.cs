using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System;


namespace SaveSystem.Data
{
    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class LocationItemSnap : SaveSnap
    {
        [JsonProperty] public bool Exists { get; private set; }


        public LocationItemSnap(MonoBehaviour behaviour, bool exists = true) : base(behaviour)
        {
            Exists = exists;
        }

        public LocationItemSnap(string id, bool exists = true) : base(id)
        {
            Exists = exists;
        }

        public LocationItemSnap() { }
    }

    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class LocationSnap : SaveSnap
    {
        [JsonProperty] public SaveSnap[] Items { get; private set; } = Array.Empty<SaveSnap>();


        public LocationSnap(MonoBehaviour behaviour, IEnumerable<SaveSnap> items = null) : base(behaviour)
        {
            if (items != null)
                Items = items.ToArray();
        }

        public LocationSnap(string id, IEnumerable<SaveSnap> items = null) : base(id)
        {
            if (items != null)
                Items = items.ToArray();
        }

        public LocationSnap() { }


        public LocationSnap UpdateSnap(Func<SaveSnap, bool> itemsSelector)
        {
            return new LocationSnap(Id, Items.Where(itemsSelector.Invoke).ToArray());
        }
    }

    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class WorldSnap : SaveSnap
    {
        [JsonProperty] private SaveSnap[][] _locations = Array.Empty<SaveSnap[]>();
        [JsonProperty] private string[] _worlds = Array.Empty<string>();

        public IEnumerable<IEnumerable<SaveSnap>> Locations => _locations;
        public IEnumerable<string> Worlds => _worlds;


        public WorldSnap(MonoBehaviour behaviour, IEnumerable<string> worlds = null, IEnumerable<IEnumerable<SaveSnap>> locations = null) : base(behaviour)
        {
            if (locations != null)
                _locations = locations.Select(locationGroup => locationGroup.ToArray()).ToArray();
            if (worlds != null)
                _worlds = worlds.ToArray();
        }

        public WorldSnap(string id, IEnumerable<string> worlds = null, IEnumerable<SaveSnap[]> locations = null) : base(id)
        {
            if (locations != null)
                _locations = locations.Select(locationGroup => locationGroup.ToArray()).ToArray();
            if (worlds != null)
                _worlds = worlds.ToArray();
        }

        public WorldSnap() { }


        public IEnumerable<SaveSnap> GetWorldInfo(string worldName)
        {
            var worldIndex = -1;
            for(var i = 0; (_worlds != null) && (i < _worlds.Length) && (worldIndex == -1); i++)
                if (_worlds[i].Equals(worldName))
                    worldIndex = i;
            
            return (worldIndex == -1) ? Enumerable.Empty<SaveSnap>() : _locations[worldIndex];
        }
        
        public void RemoveLocation(string locationId, string worldId)
        {
            var world = _worlds
                .Select((name, index) => (name, index))
                .SingleOrDefault(world => world.name.Equals(worldId));
            if (world.name == null)
                return;
            
            _locations[world.index] = 
                _locations[world.index].Where(location => !location.Id.Equals(locationId)).ToArray();
        }
    }
}