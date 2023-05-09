using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SaveSystem.Data;
using Newtonsoft.Json;
using System.IO;


namespace SaveSystem.Processing.Export
{
    public class JsonExporter : FileExporter<SnapshotDatabase>
    {
        private readonly JsonSerializerSettings _settings = new ()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };


        public JsonExporter(string folder, string file) : base(folder, file) { }


        protected override void ExportData(FileStream stream, SnapshotDatabase data)
        {
            var serializationData = JsonConvert.SerializeObject(data, _settings); 
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;

            formatter.Serialize(stream, serializationData);
            stream.Close();
        }
    }
}