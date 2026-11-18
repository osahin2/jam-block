using Core.Scenes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Bootstrap
{
    // Moves the app between its content scenes. Callers only ask; the switch runs on its own with a token that
    // outlives the scene being left, as Core.Scenes requires.
    public sealed class SceneFlow
    {
        private const string HomeScene = "Home";
        private const string GameScene = "Game";

        private readonly ISceneService _scenes;

        public SceneFlow(ISceneService scenes) => _scenes = scenes;

        public void ToHome() => SwitchTo(HomeScene);

        public void ToGame() => SwitchTo(GameScene);

        private void SwitchTo(string scene) => _scenes.SwitchToAsync(scene, Application.exitCancellationToken).Forget();
    }
}
