using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using UnityEngine;


namespace SaveSystem.Data.Providers
{
    public abstract class ProcessingProvider<T> : ScriptableObject
        where T : class
    {
        public abstract IImporter<T> BuildImporter();
        public abstract IExporter<T> BuildExporter();
    }
}