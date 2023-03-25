using System.Collections.Generic;
using SaveSystem.Base;
using SaveSystem.Data;
using UnityEngine;
using System.Linq;
using System;


namespace SaveSystem
{
    public class SaveManager: SingleBehaviour<SaveManager>
    {
        [SerializeField] private string _databasePath;
        [SerializeField] private string _databaseFile;
        
        private List<ISavable> _savables = new();
        private SaveController _controller;

        public event Action<SaveSnapshot> OnLoad;
        public event Action OnSave;

        public IReadOnlyCollection<SaveSnapshot> Snapshots => 
            _controller.Snapshots;


        protected override void Awake()
        {
            base.Awake();
            
            _controller = new SaveController(
                $"{Application.persistentDataPath}{_databasePath}", 
                _databaseFile);
        }
        
        
        public void Save(string title = null)
        {
            _controller.ClearSnapshot();
            
            _controller.SetSnapshotTitle(title);
            foreach (var savable in _savables)
                _controller.AddToSnapshot(savable.MakeSnap());
            
            _controller.SaveSnapshot();
            
            OnSave?.Invoke();
        }
        

        public void RemoveFromSavable(string id)
        {
            var savableIndex = _savables.FindIndex(savable => savable.Id.Equals(id));
            if (savableIndex != -1)
                _savables.RemoveAt(savableIndex);
        }
        
        public SaveSnapshot Load(int snapshotIndex)
        {
            var snapshot = _controller.GetSnapshot(snapshotIndex);
            OnLoad?.Invoke(snapshot);
            
            return snapshot;
        }
        
        public void AddToSavable(ISavable savable)
        {
            if (!_savables.Contains(savable))
                _savables.Add(savable);
        }
        
        public void DeleteSnapshot(int snapshotIndex)
        {
            _controller.RemoveSnapshot(Snapshots.ElementAt(snapshotIndex));
        }
        
        public void RemoveFromSavable(ISavable savable)
        {
            _savables.Remove(savable);
        }
    }
}