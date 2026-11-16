namespace Core.UI
{
    public interface IUIService
    {
        // Creates the screen the first time, then reuses it; the opened screen is drawn above the others in its layer.
        T Open<T>() where T : UIScreen;

        void Close(UIScreen screen);
    }
}
