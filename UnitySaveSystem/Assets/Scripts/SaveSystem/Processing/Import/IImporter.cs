


namespace SaveSystem.Processing.Import
{
    interface IImporter<out TData>
        where TData : class
    {
        TData Import();
    }
}
