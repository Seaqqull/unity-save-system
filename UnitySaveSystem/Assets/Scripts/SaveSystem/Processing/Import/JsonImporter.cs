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

            stream.Position = 0;

            var byteData = new byte[stream.Length];
            var numBytesToRead = (int)stream.Length;
            var numBytesRead = 0;
            
            while (numBytesToRead > 0)
            {
                var bytesLeft = stream.Read(byteData, numBytesRead, numBytesToRead);
                // Break when the end of the file is reached.
                if (bytesLeft == 0)
                    break;

                numBytesRead += bytesLeft;
                numBytesToRead -= bytesLeft;
            } 
            
            var serializedData = System.Text.Encoding.Default.GetString(byteData);
            return JsonConvert.DeserializeObject<SnapshotDatabase>(serializedData, _settings);
        }
    }
}