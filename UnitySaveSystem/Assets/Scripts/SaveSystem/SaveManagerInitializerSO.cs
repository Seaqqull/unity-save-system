using SaveSystem.Data.Providers;
using System.Reflection;
using SaveSystem.Data;
using SaveSystem.Base;
using UnityEngine;


namespace SaveSystem
{
    public class SaveManagerInitializerSO : ScriptableObject
    {
        #region Constants
        private const BindingFlags PROPERTY_GETTER = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string INSTANCE_PATH = "Seaqqull.SaveSystem/SaveSystemInitializer";
        private const string DON_DESTROY_FIELD = "_dontDestroyOnLoad";
        private const string REWRITABLE_FIELD = "_rewritable";
        #endregion

        [SerializeField] private bool _autoInitialize = true; 
        [Space]
        [SerializeField] private string _databasePath = "SaveSystem/";
        [SerializeField] private string _databaseFile = "save.dat";
        [Header("Processing")] 
        [SerializeField] private ProviderType _importType;
        [SerializeField] private ProviderType _exportType;
        [Space]
        [SerializeField] private SnapshotsImporter _importProvider; 
        [SerializeField] private SnapshotsExporter _exportProvider; 
        [Space]
        [SerializeField] private SnapshotsProvider _processingProvider; 


        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            if (SaveManager.Instance != null)
                return;

            var initializationData = Resources.Load<SaveManagerInitializerSO>(INSTANCE_PATH);
            if (!initializationData._autoInitialize)
                return;
            
            // Object initialization.
            var obj = new GameObject("SaveSystem");
            obj.SetActive(false);
            DontDestroyOnLoad(obj);
            
            // SaveManager.
            var saveManager = obj.AddComponent<SaveManager>();
            
            var singleManagerType = typeof(SingleBehaviour<SaveManager>);
            var saveManagerType = typeof(SaveManager);
            
            // SingleBehaviour fields.
            var dontDestroyField = singleManagerType.GetField(DON_DESTROY_FIELD, PROPERTY_GETTER);
            
            dontDestroyField.SetValue(saveManager, true);

            // SaveManager fields.
            var processingProvider = saveManagerType.GetField("_processingProvider", PROPERTY_GETTER);
            var importProvider = saveManagerType.GetField("_importProvider", PROPERTY_GETTER);
            var exportProvider = saveManagerType.GetField("_exportProvider", PROPERTY_GETTER);
            var databasePath = saveManagerType.GetField("_databasePath", PROPERTY_GETTER);
            var databaseFile = saveManagerType.GetField("_databaseFile", PROPERTY_GETTER);
            var importType = saveManagerType.GetField("_importType", PROPERTY_GETTER);
            var exportType = saveManagerType.GetField("_exportType", PROPERTY_GETTER);
            
            processingProvider.SetValue(saveManager, initializationData._processingProvider);
            importProvider.SetValue(saveManager, initializationData._importProvider);
            exportProvider.SetValue(saveManager, initializationData._exportProvider);
            databasePath.SetValue(saveManager, initializationData._databasePath);
            databaseFile.SetValue(saveManager, initializationData._databaseFile);
            importType.SetValue(saveManager, initializationData._importType);
            exportType.SetValue(saveManager, initializationData._exportType);
            
            // World.
            var world = obj.AddComponent<World>();
            var singleWorldType = typeof(SingleBehaviour<World>);
            
            dontDestroyField = singleWorldType.GetField(DON_DESTROY_FIELD, PROPERTY_GETTER);
            var rewritableField = singleWorldType.GetField(REWRITABLE_FIELD, PROPERTY_GETTER);
            
            dontDestroyField.SetValue(world, true);
            rewritableField.SetValue(world, true);
            
            obj.SetActive(true);
        }
    }
}