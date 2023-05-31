using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using UnityEngine;


namespace SaveSystem.Data.Providers.Custom
{
    [CreateAssetMenu(menuName = "Save-System/Providers/Custom", fileName = "CustomProvider")]
    public class CustomProvider : SnapshotsProvider
    {
        [SerializeField] private SnapshotsImporter _importer;
        [SerializeField] private SnapshotsExporter _exporter;
        
        
        public override IImporter<SnapshotDatabase> BuildImporter() =>
            _importer.Build();
        public override IExporter<SnapshotDatabase> BuildExporter() =>
            _exporter.Build();
    }
}