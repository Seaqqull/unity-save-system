


namespace SaveSystem.Processing.Export
{
    public interface IExporter<in TData>
        where TData : class
    {
        void Export(TData data);
    }
}
