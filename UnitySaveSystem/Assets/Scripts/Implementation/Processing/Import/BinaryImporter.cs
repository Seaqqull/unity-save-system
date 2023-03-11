using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;
using SaveSystem.Data;


namespace SaveSystem.Processing.Import
{
    public class BinaryImporter : FileImporter<SnapshotDatabase>
    {
        public BinaryImporter(string folder, string file) : base(folder, file) { }
        
        
        protected override SnapshotDatabase ImportData(FileStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;

            if (stream.Length <= 0)
                throw new Exception("Cannot import saves.");
            
            var data = (SnapshotDatabase)formatter.Deserialize(stream);
            return data;
        }
    }
}
