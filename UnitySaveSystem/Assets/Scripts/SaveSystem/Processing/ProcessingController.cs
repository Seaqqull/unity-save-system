using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System.Collections.Generic;
using SaveSystem.Data;
using System;


namespace SaveSystem.Processing
{
    public class ProcessingController
    {
        #region Constants
        private const string DEFAULT_DATE_FORMAT = "HH:mm:ss MM/dd/yyyy";
        #endregion
        
        private SnapshotDatabase _database = new();
        private SaveSnapshot _snapshot = new();

        private Func<IExporter<SnapshotDatabase>> _exportProvider;
        private Func<IImporter<SnapshotDatabase>> _importProvider;
        
        private Action<OperationResult> _onSave;
        private Action<OperationResult> _onLoad;

        public IReadOnlyCollection<SaveSnapshot> Snapshots =>
            _database.Get();
        private string DefaultTitle =>
            DateTime.Now.ToString(DEFAULT_DATE_FORMAT);


        public ProcessingController(
            Func<IImporter<SnapshotDatabase>> importProvider, 
            Func<IExporter<SnapshotDatabase>> exportProvider, 
            Action<OperationResult> onSave, 
            Action<OperationResult> onLoad)
        {
            _exportProvider = exportProvider;
            _importProvider = importProvider;
            _onSave = onSave;
            _onLoad = onLoad;
            
            LoadSnapshots();
        }


        private void LoadSnapshots()
        {
            try
            {
                var importer = _importProvider();
                _database = new SnapshotDatabase(importer.Import());

                _onLoad?.Invoke(new OperationResult());
            }
            catch (Exception e)
            {
                _database = new SnapshotDatabase();
                _onLoad?.Invoke(new OperationResult(e));
            }
        }

        private void SaveSnapshots()
        {
            try
            {
                var exporter = _exportProvider();
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