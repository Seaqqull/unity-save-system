using System.IO;
using System;


namespace SaveSystem.Processing.Import
{
    public abstract class FileImporter<TData> : IImporter<TData>
        where TData : class
    {
        private string _folder;
        private string _file;


        protected FileImporter(string folder, string file)
        {
            _folder = folder;
            _file = file;
        }


        public virtual TData Import()
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            using var fileHandler = new FileStream(
                $"{_folder}/{_file}",
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.None);

            try
            {
                return ImportData(fileHandler);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    $"[System] Error importing data from file: [{_folder + "/" + _file}] => {e.Message}");
            }
        }


        protected abstract TData ImportData(FileStream stream);
    }
}