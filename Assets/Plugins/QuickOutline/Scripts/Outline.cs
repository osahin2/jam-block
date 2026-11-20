//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

//  Changes from the original (MIT, see License.txt), made for this project:
//  - The renderers are given by SetRenderers, not found with GetComponentsInChildren in Awake, so pooled pieces fit.
//  - One shared mask and one shared fill material are given with them; color, width and ZTest are theirs.
//  - Show and Hide swap each renderer's sharedMaterials between two arrays kept per renderer, so they allocate
//    nothing; the arrays are filled in SetRenderers, where the renderer's own material is read. Enabling the
//    component no longer shows it.
//  - Smooth normals are written into UV3 at import by the game's editor code; the run-time computation, the static mesh
//    registry, the precompute and bake fields, submesh combining, the modes and OnValidate are gone.
//  - Deactivating its object, or giving it new renderers, hides it on the renderers before and forgets them: they go
//    back to their pools and may serve another block.

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour {
  private readonly List<Renderer> renderers = new List<Renderer>();
  private readonly List<Material[]> plainMaterials = new List<Material[]>();
  private readonly List<Material[]> outlinedMaterials = new List<Material[]>();

  public bool IsShown { get; private set; }

  public void SetRenderers(IReadOnlyList<Renderer> targets, Material mask, Material fill) {
    Hide();
    renderers.Clear();

    for (var i = 0; i < targets.Count; i++) {
      if (i == plainMaterials.Count) {
        plainMaterials.Add(new Material[1]);
        outlinedMaterials.Add(new Material[3]);
      }

      var own = targets[i].sharedMaterial;
      plainMaterials[i][0] = own;
      outlinedMaterials[i][0] = own;
      outlinedMaterials[i][1] = mask;
      outlinedMaterials[i][2] = fill;
      renderers.Add(targets[i]);
    }
  }

  public void Show() {
    IsShown = true;
    Apply(outlinedMaterials);
  }

  public void Hide() {
    if (!IsShown) {
      return;
    }

    IsShown = false;
    Apply(plainMaterials);
  }

  void OnDisable() {
    Hide();
    renderers.Clear();
  }

  void Apply(List<Material[]> materials) {
    for (var i = 0; i < renderers.Count; i++) {
      renderers[i].sharedMaterials = materials[i];
    }
  }
}
