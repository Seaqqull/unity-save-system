using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SaveSystem.Data;
using Newtonsoft.Json;
using System.IO;


namespace SaveSystem.Processing.Import
{
    public class JsonImporter : FileImporter<SnapshotDatabase>
    {
        private readonly JsonSerializerSettings _settings = new ()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };


        public JsonImporter(string folder, string file) : base(folder, file) { }


        protected override SnapshotDatabase ImportData(FileStream stream)
        {
            if (stream.Length <= 0)
                return new SnapshotDatabase();

            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0;

            var serializedData = (string)formatter.Deserialize(stream);
            return JsonConvert.DeserializeObject<SnapshotDatabase>(serializedData, _settings);
        }
    }
}