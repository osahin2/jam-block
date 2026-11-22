using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Game.Play
{
    public sealed class BlockOutlineMaterials : IDisposable
    {
        private static readonly int ZTestId = Shader.PropertyToID("_ZTest");
        private static readonly int ColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int WidthId = Shader.PropertyToID("_OutlineWidth");

        public Material Mask { get; }
        public Material Fill { get; }

        public BlockOutlineMaterials(BlockOutlineSettings settings)
        {
            Mask = new Material(settings.MaskShader) { hideFlags = HideFlags.DontSave };
            Mask.SetFloat(ZTestId, (float)CompareFunction.Always);
            Fill = new Material(settings.FillShader) { hideFlags = HideFlags.DontSave };
            Fill.SetFloat(ZTestId, (float)CompareFunction.Always);
            Fill.SetColor(ColorId, settings.Color);
            Fill.SetFloat(WidthId, settings.Width);
        }

        public void Dispose()
        {
            Object.Destroy(Mask);
            Object.Destroy(Fill);
        }
    }
}
