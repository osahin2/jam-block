using Core.Pooling;
using Core.Save;
using UnityEngine;

namespace Core.Audio
{
    // Builds the scene side of the module: one "Core.Audio" object that survives scene loads, holding the two music
    // sources, the template and the pool of sound effect sources, and the ticker.
    internal static class AudioServiceFactory
    {
        public static AudioService Create(AudioServiceSettings settings, ISaveService save)
        {
            var root = new GameObject("Core.Audio").transform;
            if (Application.isPlaying)
                Object.DontDestroyOnLoad(root.gameObject);

            var music = new MusicPlayer(CreateSource("Music A", root), CreateSource("Music B", root), settings.MusicFadeDuration);

            AudioSource template = CreateSource("Sfx Template", root);
            template.gameObject.SetActive(false);
            var pool = new PrefabPool<AudioSource>(template, root, settings.SfxPrewarmCount, settings.SfxMaxIdle);
            var sfx = new SfxPlayer(pool, root);

            root.gameObject.AddComponent<AudioTicker>().Init(sfx, music);
            return new AudioService(save, sfx, music);
        }

        private static AudioSource CreateSource(string name, Transform parent)
        {
            var source = new GameObject(name).AddComponent<AudioSource>();
            source.transform.SetParent(parent, false);
            source.playOnAwake = false;
            return source;
        }
    }
}
