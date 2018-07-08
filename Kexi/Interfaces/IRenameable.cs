namespace Kexi.Interfaces
{
    public interface IRenameable
    {
        void Rename(string newName);
        (int, int) GetRenameSelectonBorder();
    }
}