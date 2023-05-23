using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System.Collections.Generic;
using SaveSystem.Data.Providers;
using SaveSystem.Data;
using System;


namespace SaveSystem
{
    public interface ISaveManager: ISaveManagerInitializer, ISaveManagerWriteOnly, ISaveManagerReadOnly
    {
        void Clear();
        void ClearAll();
        void RemoveFromSavable(string id);
        void AddToSavable(ISavable savable);
        void RemoveFromSavable(ISavable savable);
        void DeleteSnapshot(int snapshotIndex, SaveType saveType = SaveType.Ordinal);
    }

    public interface ISaveManagerInitializer
    {
        void Initialize(ProcessingProvider<SnapshotDatabase> provider);
        void Initialize(string databasePath, string databaseName, ProviderType importType, ProviderType exportType);
        void Initialize(Func<IImporter<SnapshotDatabase>> importProvider, Func<IExporter<SnapshotDatabase>> exportProvider);
    }

    public interface ISaveManagerWriteOnly
    {
        void Save(
            SaveType saveType = SaveType.Ordinal,
            string title = null,
            Action onSuccess = null, Action onFailure = null);
    }

    public interface ISaveManagerReadOnly
    {
        SaveSnapshot Load(int snapshotIndex, SaveType saveType = SaveType.Ordinal);
        IReadOnlyCollection<SaveSnapshot> GetSnapshots(SaveType saveType = SaveType.Ordinal);
    }
}