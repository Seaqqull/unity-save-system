using System.IO;


namespace SaveSystem.Processing.Export
{
    public abstract class FileExporter<TData> : IExporter<TData>
        where TData : class
    {
        private string _folder;
        private string _file;
        
        
        public FileExporter(string folder, string file)
        {
            _folder = folder;
            _file = file;
        }
        
        
        public virtual void Export(TData data)
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
            ExportData(fileHandler, data);
        }
        
        
        protected abstract void ExportData(FileStream stream, TData data);
    }
}