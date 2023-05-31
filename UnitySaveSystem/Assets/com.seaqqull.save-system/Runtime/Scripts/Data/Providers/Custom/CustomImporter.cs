using SaveSystem.Processing.Import;
using UnityEngine;
using System.IO;


namespace SaveSystem.Data.Providers.Custom
{
    [CreateAssetMenu(menuName = "Save-System/Importers/Custom", fileName = "CustomImporter")]
    public class CustomImporter : SnapshotsImporter
    {
        [SerializeField] private ProviderType _providerType;
        [Space]
        [SerializeField] private string _databasePath;
        [SerializeField] private string _databaseFile;        
        
        
        public override IImporter<SnapshotDatabase> Build()
        {
            var fileProviderData = new FileProviderData()
            {
                File = _databaseFile,
                Folder = Path.Combine(Application.persistentDataPath, _databasePath)
            };
            
            return ProviderFabric.BuildImporter(_providerType, fileProviderData);
        }
    }
}