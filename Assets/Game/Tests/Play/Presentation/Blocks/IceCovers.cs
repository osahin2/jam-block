using Game.Play;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public static class IceCovers
    {
        public static IceCover Under(Transform parent)
        {
            var cover = new GameObject("IceCover").AddComponent<IceCover>();
            cover.transform.SetParent(parent, false);
            var count = new GameObject("Count").AddComponent<TextMeshPro>();
            count.transform.SetParent(cover.transform, false);
            var shards = new GameObject("Shards").AddComponent<ParticleSystem>();
            shards.transform.SetParent(cover.transform, false);
            var fields = new SerializedObject(cover);
            fields.FindProperty("_count").objectReferenceValue = count;
            fields.FindProperty("_shards").objectReferenceValue = shards;
            fields.ApplyModifiedPropertiesWithoutUndo();
            return cover;
        }
    }
}
