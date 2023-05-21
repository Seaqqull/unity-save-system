using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


namespace SaveSystem.Data
{
    public enum SaveType { Ordinal, AutoSave, Special }

    [Flags]
    public enum DefaultIdInitialization { Name = 1, Location = 2, Both = Name | Location }

    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class SaveSnap : IEqualityComparer<SaveSnap>
    {
        [JsonProperty] private string _id = string.Empty;

        public virtual DefaultIdInitialization IdGeneration => DefaultIdInitialization.Both;
        public string Id =>
            _id;


        public SaveSnap(MonoBehaviour behaviour)
        {
            _id = GetHash(behaviour, IdGeneration);
        }

        public SaveSnap(string id)
        {
            _id = id;
        }

        public SaveSnap() { }


        #region Comparison
        public int GetHashCode(SaveSnap obj)
        {
            return obj._id.GetHashCode();
        }

        public bool Equals(SaveSnap x, SaveSnap y)
        {
            if ((x == null) && (y == null))
                return true;
            if ((x == null) || (y == null))
                return false;
            return (x._id == y._id);
        }
        #endregion


        public static string GetHash(SHA256 hashAlgorithm, string input)
        {
            var inputData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var hash = new StringBuilder();
            foreach (var data in inputData)
                hash.Append(data.ToString("x2"));

            return hash.ToString();
        }
        
        /// <summary>
        ///     Generates unique Id based on MonoBehaviour's either name, position or combination of them.
        /// </summary>
        /// <remarks>In case of using
        ///     <see cref="DefaultIdInitialization"/>.<see cref="DefaultIdInitialization.Location"/> position for objects
        ///     in canvases differs for different resolutions.
        /// </remarks>
        /// <param name="behaviour">source <see cref="MonoBehaviour"/>.</param>
        /// <param name="way">specifies parameters of the <see cref="MonoBehaviour"/> used to generate id.</param>
        /// <returns>Unique Id.</returns>
        public static string GetHash(MonoBehaviour behaviour, DefaultIdInitialization way = DefaultIdInitialization.Both)
        {
            var gameObject = behaviour.gameObject;
            var hashInput = string.Empty;
            if (way.HasFlag(DefaultIdInitialization.Name))
                hashInput += gameObject.name;
            if (way.HasFlag(DefaultIdInitialization.Location))
                hashInput += gameObject.transform.position.ToString("F2");

            return GetHash(SHA256.Create(), hashInput);
        }
    }

    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public class SaveSnapshot
    {
        [JsonProperty] private long Time = DateTime.Now.ToBinary();

        [JsonProperty] public HashSet<SaveSnap> Data = new();
        [JsonProperty] public string Title = string.Empty;
        
        public DateTime TimeInfo => DateTime.FromBinary(Time);
    }

    [Serializable] [JsonObject(MemberSerialization.OptIn)]
    public sealed class SnapshotDatabase : ISerializable
    {
        #region Constants
        private const string VALUES_CONTAINER = "Values";
        private const string KEYS_CONTAINER = "Keys";
        #endregion


        [JsonProperty] public Dictionary<SaveType, List<SaveSnapshot>> Snapshots { get; set; } = new();

        
        public SnapshotDatabase(IDictionary<SaveType, List<SaveSnapshot>> snapshots)
        {
            Snapshots = new Dictionary<SaveType, List<SaveSnapshot>>(snapshots);
        }

        public SnapshotDatabase(SnapshotDatabase database) : this(database.Snapshots) { }
        
        public SnapshotDatabase() { }


        public void Add(SaveSnapshot snapshot, SaveType saveType = SaveType.Ordinal)
        {
            if (Snapshots.TryGetValue(saveType, out var snapshots))
                snapshots.Add(snapshot);
            else
                Snapshots.Add(saveType, new() { snapshot });
        }
        
        public List<SaveSnapshot> Get(SaveType saveType = SaveType.Ordinal)
        {
            return Snapshots.TryGetValue(saveType, out var snapshots) ? snapshots :
                new List<SaveSnapshot>();
        }
        
        public void Remove(SaveSnapshot snapshot, SaveType saveType = SaveType.Ordinal)
        {
            if (Snapshots.TryGetValue(saveType, out var snapshots))
                snapshots.Remove(snapshot);
        }

        #region ISerializable
        public SnapshotDatabase(SerializationInfo info, StreamingContext context)
        {
            List<SaveType> keys;
            try
            {
                keys = (List<SaveType>) info.GetValue(
                    $"{nameof(Snapshots)}_{KEYS_CONTAINER}", 
                    typeof(List<SaveType>));
            }
            catch
            {
                keys = new List<SaveType>();
            }


            List<List<SaveSnapshot>> values;
            try
            {
                values = (List<List<SaveSnapshot>>) info.GetValue(
                    $"{nameof(Snapshots)}_{VALUES_CONTAINER}",
                    typeof(List<List<SaveSnapshot>>));
            }
            catch
            {
                values = new List<List<SaveSnapshot>>();
            }
            
            Snapshots = new Dictionary<SaveType, List<SaveSnapshot>>(
                keys.Select((k, i) => new KeyValuePair<SaveType, List<SaveSnapshot>>(k, values[i])));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(
                $"{nameof(Snapshots)}_{KEYS_CONTAINER}", 
                Snapshots.Keys.ToList(), 
                Snapshots.Keys.GetType());
            
            info.AddValue(
                $"{nameof(Snapshots)}_{VALUES_CONTAINER}", 
                Snapshots.Values.ToList(), 
                Snapshots.Values.GetType());
        }
        #endregion
    }
}