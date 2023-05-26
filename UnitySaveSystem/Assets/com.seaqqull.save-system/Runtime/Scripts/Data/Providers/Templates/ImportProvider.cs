using SaveSystem.Processing.Import;
using UnityEngine;


namespace SaveSystem.Data.Providers
{
    public abstract class ImportProvider<T> : ScriptableObject
        where T : class
    {
        public abstract IImporter<T> Build();
    }
}