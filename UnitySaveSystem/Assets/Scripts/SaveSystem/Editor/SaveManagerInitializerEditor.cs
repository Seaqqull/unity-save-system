using System.Collections.Generic;
using SaveSystem.Data;
using System.Linq;
using UnityEditor;


namespace SaveSystem.Editor
{
    [CustomEditor(typeof(SaveManagerInitializerSO))]
    public class SaveManagerInitializerEditor : UnityEditor.Editor
    {
        #region Constants
        private const string PROCESSING_FIELD = "_processingProvider";
        private const string IMPORT_TYPE_FIELD = "_importType";
        private const string EXPORT_TYPE_FIELD = "_exportType";
        private const string IMPORT_FIELD = "_importProvider";
        private const string EXPORT_FIELD = "_exportProvider";
        #endregion

        private SerializedProperty _importTypeProvider;
        private SerializedProperty _exportTypeProvider;


        private void OnEnable()
        {
            _importTypeProvider = serializedObject.FindProperty(IMPORT_TYPE_FIELD);
            _exportTypeProvider = serializedObject.FindProperty(EXPORT_TYPE_FIELD);
        }
        
        public override void OnInspectorGUI()
        {
            var importType = (ProviderType)_importTypeProvider.enumValueFlag;
            var exportType = (ProviderType)_exportTypeProvider.enumValueFlag;

            List<string> fieldsToHide = new ();
            if ((importType != ProviderType.Custom) || (exportType != ProviderType.Custom))
                fieldsToHide.Add(PROCESSING_FIELD);
            if (importType != ProviderType.Custom)
                fieldsToHide.Add(IMPORT_FIELD);
            if (exportType != ProviderType.Custom)
                fieldsToHide.Add(EXPORT_FIELD);
            
            DrawInspectorExcept(fieldsToHide.ToArray());
        }

        /// <summary>
        /// Draws all properties like base.OnInspectorGUI() excluding the specified fields.
        /// </summary>
        /// <param name="fieldsToExclude">
        /// An array of field names which should be excluded.
        /// </param>
        private void DrawInspectorExcept(string[] fieldsToExclude)
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            
            if (property.NextVisible(true))
            {
                do
                {
                    if (fieldsToExclude.Any(property.name.Contains))
                        continue;
 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(property.name), true);
                }
                while (property.NextVisible(false));
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}