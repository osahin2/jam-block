using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Installers
{
    // A feature's installer as an asset: it references the feature's settings assets and registers its services.
    // A scope runs a serialized list of them, so adding a feature to a scene is adding its asset to that list.
    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}
