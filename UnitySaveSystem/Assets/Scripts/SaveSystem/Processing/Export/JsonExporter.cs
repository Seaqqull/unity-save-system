using SaveSystem.Data;
using Newtonsoft.Json;
using System.Text;
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
            var serializedData = JsonConvert.SerializeObject(data, _settings); 

            stream.Position = 0;
            stream.Write(Encoding.ASCII.GetBytes(serializedData));

            stream.Close();
        }
    }
}