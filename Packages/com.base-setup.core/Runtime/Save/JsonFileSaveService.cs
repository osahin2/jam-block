using System;
using System.IO;
using UnityEngine;

namespace Core.Save
{
    internal sealed class JsonFileSaveService : ISaveService
    {
        private readonly string _directory;

        public JsonFileSaveService(string directory)
        {
            _directory = directory;
        }

        public T Load<T>(string key) where T : new()
        {
            string path = PathOf(key);
            if (!File.Exists(path)) return new T();

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception exception) when (exception is ArgumentException || exception is IOException)
            {
                Debug.LogWarning($"[Core.Save] Could not read '{key}', starting from defaults: {exception.Message}");
                return new T();
            }
        }

        public void Save<T>(string key, T document)
        {
            Directory.CreateDirectory(_directory);
            string path = PathOf(key);
            string temporary = path + ".tmp";

            // Write the whole document next to the target, then swap it in, so a crash never leaves half a file.
            File.WriteAllText(temporary, JsonUtility.ToJson(document));
            if (File.Exists(path))
                File.Replace(temporary, path, null);
            else
                File.Move(temporary, path);
        }

        private string PathOf(string key) => Path.Combine(_directory, key + ".json");
    }
}
