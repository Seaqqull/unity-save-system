using SaveSystem.Processing.Export;
using UnityEngine;


namespace SaveSystem.Data.Providers
{
    public abstract class ExportProvider<T> : ScriptableObject
        where T : class
    {
        public abstract IExporter<T> Build();
    }
}