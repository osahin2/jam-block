using System.IO;
using UnityEngine;
using VContainer;

namespace Core.Save
{
    public static class SaveInstaller
    {
        public static void RegisterCoreSave(this IContainerBuilder builder) =>
            builder.RegisterInstance<ISaveService>(new JsonFileSaveService(Path.Combine(Application.persistentDataPath, "save")));
    }
}
