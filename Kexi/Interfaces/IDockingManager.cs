namespace Kexi.Interfaces
{
    public interface IDockingManager
    {
        void SerializeLayout(string   file);
        void DeserializeLayout(string file);
    }
}