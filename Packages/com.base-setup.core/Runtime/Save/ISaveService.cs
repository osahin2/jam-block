namespace Core.Save
{
    // Documents are plain [Serializable] classes stored with JsonUtility, one file per key.
    public interface ISaveService
    {
        // A missing or unreadable document comes back as new T().
        T Load<T>(string key) where T : new();

        // Writes immediately and atomically.
        void Save<T>(string key, T document);
    }
}
