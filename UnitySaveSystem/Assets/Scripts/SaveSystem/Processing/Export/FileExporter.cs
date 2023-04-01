using System.IO;
using System;


namespace SaveSystem.Processing.Export
{
    public abstract class FileExporter<TData> : IExporter<TData>
        where TData : class
    {
        private string _folder;
        private string _file;


        protected FileExporter(string folder, string file)
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

            using var fileHandler = new FileStream(
                $"{_folder}/{_file}",
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None);

            try
            {
                ExportData(fileHandler, data);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    $"[System] Error exporting data from file: [{_folder + "/" + _file}] => {e.Message}");
            }
        }


        protected abstract void ExportData(FileStream stream, TData data);
    }
}