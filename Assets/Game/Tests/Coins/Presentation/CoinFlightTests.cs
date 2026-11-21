using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tests.Coins
{
    public sealed class CoinFlightTests
    {
        // The coins land on the HUD's icon, so their canvas must scale exactly as the screens' canvas does.
        [Test]
        public void FlightCanvas_ScalesLikeTheGameUIRoot()
        {
            var screens = Scaler("Assets/Game/Runtime/Bootstrap/Prefabs/GameUIRoot.prefab");
            var flight = Scaler("Assets/Game/Runtime/Coins/Prefabs/CoinFlight.prefab");

            Assert.AreEqual(screens.uiScaleMode, flight.uiScaleMode);
            Assert.AreEqual(screens.referenceResolution, flight.referenceResolution);
            Assert.AreEqual(screens.screenMatchMode, flight.screenMatchMode);
        }

        private static CanvasScaler Scaler(string path) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponentInChildren<CanvasScaler>(true);
    }
}
