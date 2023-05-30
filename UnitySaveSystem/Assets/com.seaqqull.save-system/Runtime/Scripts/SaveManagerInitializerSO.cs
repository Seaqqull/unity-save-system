using System.Reflection;
using Newtonsoft.Json;
using SaveSystem.Data;
using SaveSystem.Base;
using UnityEngine;
using System;


namespace SaveSystem
{
    public class SaveManagerInitializerSO : ScriptableObject
    {
        private struct InitializationData
        {
            public string DatabasePath;
            public string DatabaseFile;
            public ProviderType ImportType;
            public ProviderType ExportType;
        }

        #region Constants
        private const BindingFlags PROPERTY_GETTER = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string INSTANCE_PATH = "Seaqqull.Save-System/SaveSystemInitializer";
        private const string INITIALIZATION_PATH = "Seaqqull.Save-System/SaveSystemSetup";
        private const string DONT_DESTROY_FIELD = "_dontDestroyOnLoad";
        private const string REWRITABLE_FIELD = "_rewritable";
        #endregion

        [SerializeField] private bool _autoInitialize = true; 
        [Space]
        [SerializeField] private string _databasePath = "SaveSystem/";
        [SerializeField] private string _databaseFile = "save.dat";
        [Header("Processing")] 
        [SerializeField] private ProviderType _importType;
        [SerializeField] private ProviderType _exportType;


        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            if (SaveManager.Instance != null)
                return;

            var initializer = Resources.Load<SaveManagerInitializerSO>(INSTANCE_PATH);
            if (!initializer._autoInitialize)
                return;
            
            // Object initialization.
            ProcessInitialization(initializer);
            
            var obj = new GameObject("SaveSystem");
            obj.SetActive(false);
            DontDestroyOnLoad(obj);
            
            // SaveManager.
            var saveManager = obj.AddComponent<SaveManager>();
            
            var singleManagerType = typeof(SingleBehaviour<SaveManager>);
            var saveManagerType = typeof(SaveManager);
            
            // SingleBehaviour fields.
            var dontDestroyField = singleManagerType.GetField(DONT_DESTROY_FIELD, PROPERTY_GETTER);
            
            dontDestroyField.SetValue(saveManager, true);

            // SaveManager fields.
            var databasePath = saveManagerType.GetField("_databasePath", PROPERTY_GETTER);
            var databaseFile = saveManagerType.GetField("_databaseFile", PROPERTY_GETTER);
            var importType = saveManagerType.GetField("_importType", PROPERTY_GETTER);
            var exportType = saveManagerType.GetField("_exportType", PROPERTY_GETTER);

            databasePath.SetValue(saveManager, initializer._databasePath);
            databaseFile.SetValue(saveManager, initializer._databaseFile);
            importType.SetValue(saveManager, initializer._importType);
            exportType.SetValue(saveManager, initializer._exportType);
            
            // World.
            var world = obj.AddComponent<World>();
            var singleWorldType = typeof(SingleBehaviour<World>);
            
            dontDestroyField = singleWorldType.GetField(DONT_DESTROY_FIELD, PROPERTY_GETTER);
            var rewritableField = singleWorldType.GetField(REWRITABLE_FIELD, PROPERTY_GETTER);
            
            dontDestroyField.SetValue(world, true);
            rewritableField.SetValue(world, true);
            
            obj.SetActive(true);
        }
        
        private static void ProcessInitialization(SaveManagerInitializerSO initializer)
        {
            var initializationData = Resources.Load<TextAsset>(INITIALIZATION_PATH);
            if (initializationData == null)
                return;

            try
            {
                var data = JsonConvert.DeserializeObject<InitializationData>(initializationData.text);
                if (!Enum.IsDefined(typeof(ProviderType), data.ImportType) ||
                    !Enum.IsDefined(typeof(ProviderType), data.ExportType))
                    return;

                initializer._databasePath = data.DatabasePath;
                initializer._databaseFile = data.DatabaseFile;
                initializer._importType = data.ImportType;
                initializer._exportType = data.ExportType;
            }
            catch (Exception) { }
        }
    }
}