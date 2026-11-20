using System.Text;
using Game.Editor.LevelEditor;
using Game.Levels;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    public sealed class LevelStageTests
    {
        private const float Tolerance = 0.002f;

        private CameraFramingSettings _framing;
        private LevelStage _stage;
        private Camera _camera;

        [SetUp]
        public void SetUp()
        {
            _framing = Load<CameraFramingSettings>();
            _stage = new LevelStage(Load<BoardArtSettings>(), Load<ColorPalette>(), Load<BlockMotionSettings>(), _framing);
            // The Game scene's camera: a 63 degree field of view, tilted steeply down toward the board.
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _camera.fieldOfView = 63f;
            _camera.transform.rotation = Quaternion.Euler(76f, 0f, 0f);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_camera.gameObject);

        [TestCase(1080, 1920)]
        [TestCase(1080, 2400)]
        public void Show_EveryCatalogLevel_KeepsEveryPieceInsideTheWindow(int screenWidth, int screenHeight)
        {
            _camera.aspect = (float)screenWidth / screenHeight;
            // Batch runs have no notch, so the screen's safe area is all of it.
            var window = new CameraFraming(_framing).ViewportWindow(new Rect(0f, 0f, 1f, 1f), _camera.aspect);
            var outside = new StringBuilder();
            foreach (var level in Load<LevelCatalog>().Levels)
            {
                var board = new GameObject("Board").transform;
                _stage.Show(level.Level, board, new PreviewBoardPieces(), _camera);
                foreach (var piece in board.GetComponentsInChildren<Renderer>())
                {
                    var bounds = piece.bounds;
                    for (var corner = 0; corner < 8; corner++)
                    {
                        var point = new Vector3(
                            (corner & 1) == 0 ? bounds.min.x : bounds.max.x,
                            (corner & 2) == 0 ? bounds.min.y : bounds.max.y,
                            (corner & 4) == 0 ? bounds.min.z : bounds.max.z);
                        var onScreen = _camera.WorldToViewportPoint(point);
                        if (onScreen.x < window.xMin - Tolerance || onScreen.x > window.xMax + Tolerance ||
                            onScreen.y < window.yMin - Tolerance || onScreen.y > window.yMax + Tolerance)
                            outside.AppendLine($"{level.name}: {piece.name} at {onScreen}");
                    }
                }
                Object.DestroyImmediate(board.gameObject);
            }

            Assert.IsEmpty(outside.ToString());
        }

        private static T Load<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/Game" });
            Assert.AreEqual(1, guids.Length, $"one {typeof(T).Name} asset");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
