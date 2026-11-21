using Game.Bootstrap;
using Game.Data.Levels;
using Game.Play;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelScenePreview
    {
        public const string RootName = "Level Preview (not saved)";

        private readonly PreviewBoardPieces _pieces = new PreviewBoardPieces();
        private ColorPalette _palette;
        private LevelStage _stage;
        private GameObject _root;
        private string _shown;

        public void Show(LevelData level)
        {
            var snapshot = level != null ? JsonUtility.ToJson(level) : null;
            if (_root != null && snapshot == _shown)
                return;
            Clear();
            _shown = snapshot;
            if (level == null || !TryFindGameScene(out var scene, out var camera, out var board) || !TryLoadSettings() ||
                !CanBuild(level))
                return;
            _root = new GameObject(RootName);
            var boardTransform = board.transform;
            _root.transform.SetPositionAndRotation(boardTransform.position, boardTransform.rotation);
            _root.transform.localScale = boardTransform.lossyScale;
            SceneManager.MoveGameObjectToScene(_root, scene);
            _root.hideFlags = PreviewBoardPieces.PreviewFlags;
            _stage.Show(level, _root.transform, _pieces, camera);
        }

        public void Clear()
        {
            if (_root != null)
                Object.DestroyImmediate(_root);
            _root = null;
        }

        private static bool TryFindGameScene(out Scene gameScene, out Camera camera, out BoardRoot board)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    var scope = root.GetComponent(typeof(GameScope));
                    if (scope == null)
                        continue;
                    var fields = new SerializedObject(scope);
                    gameScene = scene;
                    camera = fields.FindProperty("_camera").objectReferenceValue as Camera;
                    board = fields.FindProperty("_board").objectReferenceValue as BoardRoot;
                    return camera != null && board != null;
                }
            }
            gameScene = default;
            camera = null;
            board = null;
            return false;
        }

        private bool TryLoadSettings()
        {
            if (_stage != null)
                return true;
            var art = EditorAssets.FindFirst<BoardArtSettings>();
            _palette = EditorAssets.FindFirst<ColorPalette>();
            var motion = EditorAssets.FindFirst<BlockMotionSettings>();
            var ice = EditorAssets.FindFirst<IceSettings>();
            var framing = EditorAssets.FindFirst<CameraFramingSettings>();
            if (art == null || _palette == null || motion == null || ice == null || framing == null)
                return false;
            _stage = new LevelStage(art, _palette, motion, ice, framing);
            return true;
        }

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
