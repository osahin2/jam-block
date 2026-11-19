using System.Text;
using Game.Editor.LevelEditor;
using Game.Levels;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Play
{
    // Shows the catalog's levels with the project's settings and prefabs, so the framing is checked against the pieces
    // as built, not against the box the framing assumes.
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
            // The Game scene's camera: a 45 degree field of view, tilted down toward the board.
            _camera = new GameObject("Camera").AddComponent<Camera>();
            _camera.fieldOfView = 45f;
            _camera.transform.rotation = Quaternion.Euler(71.4f, 0f, 0f);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_camera.gameObject);

        [TestCase(1080, 1920)]
        [TestCase(1080, 2400)]
        public void Show_EveryCatalogLevel_KeepsEveryPieceInsideTheMargins(int screenWidth, int screenHeight)
        {
            _camera.aspect = (float)screenWidth / screenHeight;
            var side = _framing.SideMargin;
            var topAndBottom = _framing.TopAndBottomMargin;
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
                        if (onScreen.x < side - Tolerance || onScreen.x > 1f - side + Tolerance ||
                            onScreen.y < topAndBottom - Tolerance || onScreen.y > 1f - topAndBottom + Tolerance)
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
