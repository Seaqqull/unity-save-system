using System.Collections.Generic;
using SaveSystem.Processing;
using SaveSystem.Base;
using SaveSystem.Data;
using UnityEngine;
using System.Linq;
using System.IO;
using System;


namespace SaveSystem
{
    public class SaveManager: SingleBehaviour<SaveManager>
    {
        [SerializeField] private string _databasePath;
        [SerializeField] private string _databaseFile;

        private List<ISavable> _savables = new();
        private ProcessingController _controller;

        public event Action<SaveSnapshot> OnLoad;
        public event Action<SaveType> OnSave;

        public IReadOnlyCollection<SaveSnapshot> Snapshots =>
            _controller.Snapshots;


        protected override void Awake()
        {
            base.Awake();

            
            _controller = new ProcessingController(
                Path.Combine(Application.persistentDataPath, _databasePath),
                _databaseFile);
        }


        private void SaveSnapshot(string title, SaveType saveType)
        {
            _controller.ClearSnapshot();

            _controller.SetSnapshotTitle(title);
            _controller.AddToSnapshot(_savables.Select(savable => savable.MakeSnap()));

            _controller.SaveSnapshot(saveType);
        }


        [ContextMenu("Clear")]
        public void Clear()
        {
            _controller.ClearSnapshot();
        }

        [ContextMenu("Clear all")]
        public void ClearAll()
        {
            _controller.ClearSnapshots();
        }

        public void Save(SaveType saveType = SaveType.Ordinal, string title = null)
        {
            SaveSnapshot(title, saveType);

            OnSave?.Invoke(saveType);
        }
        
        public SaveSnapshot Load(int snapshotIndex, SaveType saveType = SaveType.Ordinal)
        {
            var snapshot = _controller.GetSnapshot(snapshotIndex, saveType);
            if (snapshot != null)
            {
                foreach (var savable in _savables)
                foreach (var snap in snapshot.Data)
                {
                    if (snap.Id.Equals(savable.Id))
                        savable.FromSnap(snap);
                }
            }
            
            OnLoad?.Invoke(snapshot);

            return snapshot;
        }
        

        public void RemoveFromSavable(string id)
        {
            var savableIndex = _savables.FindIndex(savable => savable.Id.Equals(id));
            if (savableIndex != -1)
                _savables.RemoveAt(savableIndex);
        }

        public void AddToSavable(ISavable savable)
        {
            if (!_savables.Contains(savable))
                _savables.Add(savable);
        }

        public void RemoveFromSavable(ISavable savable)
        {
            _savables.Remove(savable);
        }

        public void DeleteSnapshot(int snapshotIndex, SaveType saveType = SaveType.Ordinal)
        {
            _controller.RemoveSnapshot(_controller.GetSnapshots(saveType).ElementAt(snapshotIndex), saveType);
        }
        
        public IReadOnlyCollection<SaveSnapshot> GetSnapshots(SaveType saveType = SaveType.Ordinal)
        {
            return _controller.GetSnapshots(saveType);
        }
    }
}