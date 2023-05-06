using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System.Collections.Generic;
using SaveSystem.Data;
using UnityEngine;
using System;


namespace SaveSystem.Processing
{
    public class OperationResult
    {
        public bool Success => Error == null;
        public Exception Error { get; }

            
        public OperationResult(Exception e = null) =>
            Error = e;
    }
    
    public class ProcessingController
    {
        #region Constants
        private const string DEFAULT_DATE_FORMAT = "HH:mm:ss MM/dd/yyyy";
        #endregion
        
        private SnapshotDatabase _database = new();
        private SaveSnapshot _snapshot = new();
        private string _workDatabasePath;
        private string _databaseFile;

        private Action<OperationResult> _onSave;
        private Action<OperationResult> _onLoad;

        public IReadOnlyCollection<SaveSnapshot> Snapshots =>
            (_workDatabasePath == null) ? Array.Empty<SaveSnapshot>() : _database.Get();
        private string DefaultTitle =>
            DateTime.Now.ToString(DEFAULT_DATE_FORMAT);


        public ProcessingController(
            string databasePath, 
            string databaseFile, 
            Action<OperationResult> onSave, 
            Action<OperationResult> onLoad)
        {
            _workDatabasePath = databasePath;
            _databaseFile = databaseFile;
            _onSave = onSave;
            _onLoad = onLoad;
            
            LoadSnapshots();
        }


        private void LoadSnapshots()
        {
            try
            {
                var importer = new BinaryImporter(_workDatabasePath, _databaseFile);
                _database = new SnapshotDatabase(importer.Import());

                _onLoad?.Invoke(new OperationResult());
            }
            catch (Exception e)
            {
                _onLoad?.Invoke(new OperationResult(e));
            }
        }

        private void SaveSnapshots()
        {
            try
            {
                var exporter = new BinaryExporter(_workDatabasePath, _databaseFile);
                exporter.Export(new SnapshotDatabase(_database));
                
                _onSave?.Invoke(new OperationResult());
            }
            catch (Exception e)
            {
                _onSave?.Invoke(new OperationResult(e));
            }
        }


        public void ClearSnapshot()
        {
            _snapshot = new SaveSnapshot
            {
                Title = DefaultTitle
            };
        }

        public void ClearSnapshots()
        {
            ClearSnapshot();

            _database.Snapshots.Clear();
            SaveSnapshots();
        }

        public void RemoveFromSnapshot(string id)
        {
            _snapshot.Data.RemoveWhere(snap => snap.Id.Equals(id));
        }
        
        public void SetSnapshotTitle(string title)
        {
            _snapshot.Title = string.IsNullOrWhiteSpace(title) ?
                DefaultTitle : title;
        }

        public void AddToSnapshot(SaveSnap saveData)
        {
            _snapshot.Data.Add(saveData);
        }

        public void AddToSnapshot(IEnumerable<SaveSnap> saveData)
        {
            foreach(var data in saveData)
                AddToSnapshot(data);
        }
        
        public void SaveSnapshot(SaveType saveType = SaveType.Ordinal)
        {
            _database.Add(_snapshot, saveType);
            SaveSnapshots();
        }

        public SaveSnapshot GetLastSnapshot(SaveType saveType = SaveType.Ordinal)
        {
            return _database.Get(saveType)[^1];
        }
        
        public List<SaveSnapshot> GetSnapshots(SaveType saveType = SaveType.Ordinal)
        {
            return _database.Get(saveType);
        }

        public SaveSnapshot GetSnapshot(int index, SaveType saveType = SaveType.Ordinal)
        {
            var snapshots = _database.Get(saveType);
            return ((index < 0) || (index >= snapshots.Count)) ? null : snapshots[index];
        }
        
        public void RemoveSnapshot(SaveSnapshot snapshot, SaveType saveType = SaveType.Ordinal)
        {
            _database.Remove(snapshot, saveType);
            SaveSnapshots();
        }
    }
}