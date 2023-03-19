


namespace SaveSystem.Processing.Export
{
    public interface IExporter<TData>
        where TData : class
    {
        void Export(TData data);
    }
}
