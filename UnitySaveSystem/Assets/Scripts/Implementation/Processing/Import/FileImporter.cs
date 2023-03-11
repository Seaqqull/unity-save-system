using System.IO;
using System;


namespace SaveSystem.Processing.Import
{
    public abstract class FileImporter<TData> : IImporter<TData>
        where TData : class 
    {
        private string _folder;
        private string _file;
        
        
        public FileImporter(string folder, string file)
        {
            _folder = folder;
            _file = file;
        }
        
        
        public virtual TData Import()
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
            if (!File.Exists($"{_folder}/{_file}"))
                File.Create($"{_folder}/{_file}");
            
            var fileHandler = new FileStream(
                $"{_folder}/{_file}", 
                FileMode.OpenOrCreate, 
                FileAccess.ReadWrite, 
                FileShare.None);

            try
            {
                return ImportData(fileHandler);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"[System] Error exporting data from file: [{_folder + "/" + _file}] => {e.Message}");
            }
        }
        
        
        protected abstract TData ImportData(FileStream stream);
    }
}