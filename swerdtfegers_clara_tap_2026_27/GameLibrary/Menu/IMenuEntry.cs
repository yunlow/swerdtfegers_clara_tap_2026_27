namespace GameLibrary.Menu
{
    public interface IMenuEntry
    {
        string GetText();

        void Execute();
    }
}
