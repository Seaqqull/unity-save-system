


namespace SaveSystem.Processing.Import
{
    public interface IImporter<out TData>
        where TData : class
    {
        TData Import();
    }
}
