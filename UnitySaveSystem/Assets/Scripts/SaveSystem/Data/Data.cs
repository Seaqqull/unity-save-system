using SaveSystem.Processing.Export;
using SaveSystem.Processing.Import;
using System;


namespace SaveSystem.Data
{
    public enum ProviderType { Binary, Json, Custom }

    public class ProviderData { }

    public class NoProviderData : ProviderData
    {
        public static NoProviderData Instance { get; } = new NoProviderData();
    }

    public class FileProviderData : ProviderData
    {
        public string Folder;
        public string File;
    }


    public static class ProviderFabric
    {
        public static IImporter<SnapshotDatabase> BuildImporter(ProviderType providerType, ProviderData data)
        {
            switch (providerType)
            {
                case ProviderType.Binary:
                    var binaryData = (data as FileProviderData)!;
                    return new BinaryImporter(binaryData.Folder, binaryData.File);
                case ProviderType.Json:
                    var jsonData = (data as FileProviderData)!;
                    return new JsonImporter(jsonData.Folder, jsonData.File);
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
            }
        }

        public static IExporter<SnapshotDatabase> BuildExporter(ProviderType providerType, ProviderData data)
        {
            switch (providerType)
            {
                case ProviderType.Binary:
                    var binaryData = (data as FileProviderData)!;
                    return new BinaryExporter(binaryData.Folder, binaryData.File);
                case ProviderType.Json:
                    var jsonData = (data as FileProviderData)!;
                    return new JsonExporter(jsonData.Folder, jsonData.File);
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
            }
        }
    }
}