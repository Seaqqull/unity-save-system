


namespace SaveSystem.Processing.Import
{
    interface IImporter<TData>
        where TData : class
    {
        TData Import();
    }
}
