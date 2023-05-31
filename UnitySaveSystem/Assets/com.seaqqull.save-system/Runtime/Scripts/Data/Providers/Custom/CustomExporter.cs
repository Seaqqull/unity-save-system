using SaveSystem.Processing.Export;
using UnityEngine;
using System.IO;


namespace SaveSystem.Data.Providers.Custom
{
    [CreateAssetMenu(menuName = "Save-System/Exporters/Custom", fileName = "CustomExporter")]
    public class CustomExporter : SnapshotsExporter
    {
        [SerializeField] private ProviderType _providerType;
        [Space]
        [SerializeField] private string _databasePath;
        [SerializeField] private string _databaseFile;


        public override IExporter<SnapshotDatabase> Build()
        {
            var fileProviderData = new FileProviderData()
            {
                File = _databaseFile,
                Folder = Path.Combine(Application.persistentDataPath, _databasePath)
            };
            
            return ProviderFabric.BuildExporter(_providerType, fileProviderData);
        }
    }
}