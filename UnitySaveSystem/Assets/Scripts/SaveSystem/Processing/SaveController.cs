using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System.Collections.Generic;
using SaveSystem.Data;
using UnityEngine;
using System;


namespace SaveSystem
{
    public class SaveController
    {
        private const string DEFAULT_DATE_FORMAT = "HH:mm:ss MM/dd/yyyy";

        private string _databaseFile;

        private SnapshotDatabase _database = new();
        private SaveSnapshot _snapshot = new();
        private string _workDatabasePath;

        public IReadOnlyCollection<SaveSnapshot> Snapshots =>
            (_workDatabasePath == null) ? Array.Empty<SaveSnapshot>() : _database.Snapshots;
        public string DefaultTitle =>
            DateTime.Now.ToString(DEFAULT_DATE_FORMAT);


        public SaveController(string databasePath, string databaseFile)
        {
            _workDatabasePath = databasePath;
            _databaseFile = databaseFile;

            LoadSnapshots();
        }


        private void LoadSnapshots()
        {
            try
            {
                var importer = new BinaryImporter(_workDatabasePath, _databaseFile);
                _database = new SnapshotDatabase(importer.Import());
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured during loading: {e.Message}");
            }
        }

        private void SaveSnapshots()
        {
            try
            {
                var exporter = new BinaryExporter(_workDatabasePath, _databaseFile);
                exporter.Export(new SnapshotDatabase(_database));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occurred during saving: {e.Message}");
            }
        }


        public void SaveSnapshot()
        {
            try
            {
                LoadSnapshots();
                _database.Snapshots.Add(_snapshot);
                SaveSnapshots();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearSnapshot()
        {
            _snapshot = new SaveSnapshot
            {
                Title = DefaultTitle
            };
        }

        public SaveSnapshot GetLastSnapshot()
        {
            return _database.Snapshots[^1];
        }

        public void RemoveFromSnapshot(string id)
        {
            _snapshot.Data.RemoveWhere(snap => snap.Id.Equals(id));
        }

        public SaveSnapshot GetSnapshot(int index)
        {
            return ((index < 0) || (index >= _database.Snapshots.Count)) ? null : _database.Snapshots[index];
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

        public void RemoveSnapshot(SaveSnapshot snapshot)
        {
            _database.Snapshots.Remove(snapshot);
            SaveSnapshots();
        }
        
        public void AddToSnapshot(IEnumerable<SaveSnap> saveData)
        {
            foreach(var data in saveData)
                AddToSnapshot(data);
        }
    }
}