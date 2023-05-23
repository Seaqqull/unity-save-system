using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System.Collections.Generic;
using SaveSystem.Data.Providers;
using SaveSystem.Processing;
using SaveSystem.Base;
using SaveSystem.Data;
using UnityEngine;
using System.Linq;
using System.IO;
using System;


namespace SaveSystem
{
    public class SaveManager : SingleBehaviour<SaveManager>, ISaveManager
    {
        [SerializeField] private string _databasePath = string.Empty;
        [SerializeField] private string _databaseFile = string.Empty;
        [Header("Processing")] 
        [SerializeField] private ProviderType _importType;
        [SerializeField] private ProviderType _exportType;
        [Space]
        [SerializeField] private SnapshotsImporter _importProvider; 
        [SerializeField] private SnapshotsExporter _exportProvider; 
        [Space]
        [SerializeField] private SnapshotsProvider _processingProvider; 

        private List<ISavable> _savables = new();
        private ProcessingController _controller;

        private Action _onSuccess;
        private Action _onFailure;

        public IReadOnlyCollection<SaveSnapshot> Snapshots =>
            _controller.Snapshots;


        protected override void Awake()
        {
            base.Awake();

            var providers = BuildProviders();
            _controller = new ProcessingController(providers.Importer, providers.Exporter, OnSave, OnLoad);
        }

        
        private void OnLoad(OperationResult result)
        {
            if (!result.Success)
                Debug.LogError($"Error occured during loading: {result.Error.Message}");
        }

        private void OnSave(OperationResult result)
        {
            if (!result.Success)
            {
                Debug.LogError($"Error occurred during saving: {result.Error.Message}");
                _onFailure?.Invoke();
            }
            else
            {
                _onSuccess?.Invoke();
            }
        }

        private void SaveSnapshot(string title, SaveType saveType)
        {
            _controller.ClearSnapshot();

            _controller.SetSnapshotTitle(title);
            _controller.AddToSnapshot(_savables.Select(savable => savable.MakeSnap()));

            _controller.SaveSnapshot(saveType);
        }
        
        private (Func<IImporter<SnapshotDatabase>> Importer, Func<IExporter<SnapshotDatabase>> Exporter) BuildProviders()
        {
            var useProcessingProvider = (_exportType == ProviderType.Custom) &&
                                        (_importType == ProviderType.Custom) && (_processingProvider != null);
            if (useProcessingProvider)
                return (() => _processingProvider.BuildImporter(), () => _processingProvider.BuildExporter());
            
            var useImportProvider = (_importType == ProviderType.Custom) && (_importProvider != null);
            var useExportProvider = (_exportType == ProviderType.Custom) && (_exportProvider != null);
            var fileProviderData = new FileProviderData()
            {
                File = _databaseFile,
                Folder = Path.Combine(Application.persistentDataPath, _databasePath)
            };

            return (
                useImportProvider ? () => _importProvider.Build() : 
                    () => ProviderFabric.BuildImporter(_importType, fileProviderData),
                useExportProvider ? () => _exportProvider.Build() : 
                    () => ProviderFabric.BuildExporter(_exportType, fileProviderData));
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

        public void Save(
            SaveType saveType = SaveType.Ordinal, 
            string title = null, 
            Action onSuccess = null, Action onFailure = null)
        {
            _onSuccess = onSuccess;
            _onFailure = onFailure;
            
            SaveSnapshot(title, saveType);
            
            _onSuccess = null;
            _onFailure = null;
        }
        
        public SaveSnapshot Load(int snapshotIndex, SaveType saveType = SaveType.Ordinal)
        {
            var snapshot = _controller.GetSnapshot(snapshotIndex, saveType);
            if (snapshot == null) return snapshot;
            
            foreach (var savable in _savables)
            foreach (var snap in snapshot.Data.Where(snap => snap.Id.Equals(savable.Id)))
                savable.FromSnap(snap);

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

        public void Initialize(ProcessingProvider<SnapshotDatabase> provider)
        {
            _controller = new ProcessingController(provider.BuildImporter, provider.BuildExporter, OnSave, OnLoad);
        }

        public void Initialize(string databasePath, string databaseName, ProviderType importType, ProviderType exportType)
        {
            _databasePath = databasePath;
            _databaseFile = databaseName;

            _importType = importType;
            _exportType = exportType;
            
            var providers = BuildProviders();
            _controller = new ProcessingController(providers.Importer, providers.Exporter, OnSave, OnLoad);
        }

        public void Initialize(Func<IImporter<SnapshotDatabase>> importProvider, Func<IExporter<SnapshotDatabase>> exportProvider)
        {
            _controller = new ProcessingController(importProvider, exportProvider, OnSave, OnLoad);
        }
    }
}