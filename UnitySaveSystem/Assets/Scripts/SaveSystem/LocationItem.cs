using System.Collections;
using SaveSystem.Data;
using UnityEngine;


namespace SaveSystem
{
    public class LocationItem : MonoBehaviour, ISavable
    {
        private bool _exists = true;

        public string Id { get; private set; }
        public bool Exists
        {
            get => _exists;
            set
            {
                _exists = value;
                Location.AddGlobally(this);
            }
        }


        protected virtual void Awake()
        {
            if (Id == null) InitializeId();
        }

        protected virtual void Start()
        {
            Location.AddGlobally(this);
        }

        private void OnDestroy()
        {
            Location.RemoveGlobally(this);
        }


        private void InitializeId()
        {
            Id = SaveSnap.GetHash(this);
        }


        public void UpdateItemState()
        {
            Exists = _exists;
        }

        #region Savable
        public virtual SaveSnap MakeSnap()
        {
            return new LocationItemSnap(Id, _exists);
        }

        public virtual void FromSnap(SaveSnap data)
        {
            if (Id == null) InitializeId();

            var itemData = data as LocationItemSnap;
            if (itemData == null)
                return;

            _exists = itemData.Exists;
            Location.AddGlobally(this);

            if (!_exists)
                Destroy(gameObject);
        }
        #endregion
    }
}