


namespace SaveSystem.Data
{
    public interface ISavable
    {
        public string Id { get; }


        public SaveSnap MakeSnap();
        public void FromSnap(SaveSnap data);
    }
}