using System.Collections.Generic;
using Core.Save;
using UnityEngine;

namespace Core.Audio.Tests
{
    // Round-trips through JSON so tests see what a real save would keep.
    internal sealed class MemorySave : ISaveService
    {
        public readonly Dictionary<string, string> Files = new Dictionary<string, string>();
        public int SaveCount;

        public T Load<T>(string key) where T : new() =>
            Files.TryGetValue(key, out string json) ? JsonUtility.FromJson<T>(json) : new T();

        public void Save<T>(string key, T document)
        {
            SaveCount++;
            Files[key] = JsonUtility.ToJson(document);
        }
    }
}
