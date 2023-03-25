using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;


namespace SaveSystem.Data
{
    [Serializable]
    public class SaveSnap : IEqualityComparer<SaveSnap>
    {
        private Func<MonoBehaviour> _monoGetter;
        private Func<bool> _isMonoAccessible;
        private string _id = string.Empty;
        
        public MonoBehaviour GetMono =>
            _monoGetter?.Invoke();
        public bool MonoAccessible => 
            _isMonoAccessible?.Invoke() ?? true;
        public string Id =>
            _id;
        
        
        public SaveSnap(MonoBehaviour behaviour)
        {
            _id = GetHash(behaviour);
        }

        public SaveSnap(string id)
        {
            _id = id;
        }


        public void AssignAccessor(Func<bool> isMonoAccessible)
        {
            _isMonoAccessible = isMonoAccessible;
        }

        public void AssignGetter<T>(Func<T> monoGetter) 
            where T : MonoBehaviour, ISavable
        {
            _monoGetter = monoGetter;
        }


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
        
        
        public static string GetHash(MonoBehaviour behaviour)
        {
            var gameObject = behaviour.gameObject;
            var position = gameObject.transform.position;
            var name = gameObject.name;
            
            return GetHash(SHA256.Create(), name + position.ToString("F2"));
        }
        
        public static string GetHash(SHA256 hashAlgorithm, string input)
        {
            var inputData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var hash = new StringBuilder();
            foreach (var data in inputData)
                hash.Append(data.ToString("x2"));

            return hash.ToString();
        }
    }
    
    [Serializable]
    public class SaveSnapshot
    {
        public string Title = string.Empty;
        public HashSet<SaveSnap> Data = new();
    }
    
    [Serializable]
    public class SnapshotDatabase : ISerializable
    {
        public List<SaveSnapshot> Snapshots { get; set; } = new ();

        
        public SnapshotDatabase()
        {
            Snapshots = new List<SaveSnapshot>();
        }
        
        public SnapshotDatabase(IEnumerable<SaveSnapshot> snapshots)
        {
            Snapshots = new List<SaveSnapshot>(snapshots);
        }
        
        public SnapshotDatabase(SnapshotDatabase database) : 
            this(database.Snapshots) { }
        
        public SnapshotDatabase(SerializationInfo info, StreamingContext context)
        {
            Snapshots = (List<SaveSnapshot>) info.GetValue(nameof(Snapshots), typeof(List<SaveSnapshot>));
        }
        

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Snapshots), Snapshots, typeof(List<SaveSnapshot>));
        }
    }
}