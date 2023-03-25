using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SaveSystem.Data;
using System.IO;


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
                return new SnapshotDatabase();
            
            var data = (SnapshotDatabase)formatter.Deserialize(stream);
            return data;
        }
    }
}
