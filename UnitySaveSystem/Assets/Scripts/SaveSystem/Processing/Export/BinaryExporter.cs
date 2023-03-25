using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SaveSystem.Data;
using System.IO;


namespace SaveSystem.Processing.Export
{
    public class BinaryExporter : FileExporter<SnapshotDatabase>
    {
        public BinaryExporter(string folder, string file) : base(folder, file) { }

        
        protected override void ExportData(FileStream stream, SnapshotDatabase data)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;
            
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }
}
