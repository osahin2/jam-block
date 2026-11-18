using Game.Bootstrap;
using Game.Data.Levels;
using Game.Play.Presentation;
using Game.Play.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor.LevelEditor.Preview
{
    // Shows a level in the open Game scene, built by the game's own BoardBuilder, so the Scene view and the Game view
    // show what the game will build. Its objects are never saved and cannot be edited: the level is edited in the
    // level editor only.
    public sealed class LevelScenePreview
    {
        public const string RootName = "Level Preview (not saved)";
        private const HideFlags PreviewFlags = HideFlags.DontSave | HideFlags.NotEditable;

        private BoardArtSettings _art;
        private ColorPalette _palette;
        private BoardBuilder _builder;
        private GameObject _root;
        private string _shown;

        // Builds nothing when no Game scene is open or the level cannot be built. A level equal to the one on show is
        // not built again, so a refresh that changed nothing costs no rebuild.
        public void Show(LevelData level)
        {
            var snapshot = level != null ? JsonUtility.ToJson(level) : null;
            if (_root != null && snapshot == _shown)
                return;
            Clear();
            _shown = snapshot;
            if (level == null || !TryFindGameScene(out var scene) || !TryLoadSettings() || !CanBuild(level))
                return;
            _root = new GameObject(RootName);
            SceneManager.MoveGameObjectToScene(_root, scene);
            _builder.Build(level, new BoardLayout(level.Width, level.Height, _art.CellSize), _root.transform);
            foreach (var child in _root.GetComponentsInChildren<Transform>(true))
                child.gameObject.hideFlags = PreviewFlags;
        }

        public void Clear()
        {
            if (_root != null)
                Object.DestroyImmediate(_root);
            _root = null;
        }

        // The Game scene is the one whose root holds the game's scope.
        private static bool TryFindGameScene(out Scene gameScene)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (root.TryGetComponent<GameScope>(out _))
                    {
                        gameScene = scene;
                        return true;
                    }
                }
            }
            gameScene = default;
            return false;
        }

        // Loaded on first use: while scripts reload, the asset database may not be ready yet.
        private bool TryLoadSettings()
        {
            if (_builder != null)
                return true;
            _art = EditorAssets.FindFirst<BoardArtSettings>();
            _palette = EditorAssets.FindFirst<ColorPalette>();
            var motion = EditorAssets.FindFirst<BlockMotionSettings>();
            if (_art == null || _palette == null || motion == null)
                return false;
            _builder = new BoardBuilder(_art, _palette, motion, new BlockPieceBuilder(_art));
            return true;
        }

        // A level being drawn may have errors the builder copes with, but not a missing board or color.
        private bool CanBuild(LevelData level)
        {
            if (level.Width < 1 || level.Height < 1)
                return false;
            foreach (var block in level.Blocks)
            {
                if (!_palette.Has(block.ColorId))
                    return false;
            }
            foreach (var door in level.Doors)
            {
                if (!_palette.Has(door.ColorId))
                    return false;
            }
            return true;
        }
    }
}
