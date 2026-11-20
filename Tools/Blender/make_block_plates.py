# Makes Assets/Art/Models/BlockPlates.fbx from the supplied BlockParts.fbx: each block piece cut where the ground meets
# it when sunk to a flat plate, its bottom closed, and moved down so the plate's bottom is where the piece's was.
# Run: Blender -b --factory-startup --python Tools/Blender/make_block_plates.py -- <BlockParts.fbx> <BlockPlates.fbx>
import sys

import bmesh
import bpy

# In the pieces' local space z runs down the piece: the top is at -0.963, the bottom at 0.656.
GROUND_Z = -0.444
SUNK = 0.656 - GROUND_Z

source, target = sys.argv[sys.argv.index("--") + 1:]
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=source)

for obj in bpy.data.objects:
    if obj.type != 'MESH':
        continue
    mesh = bmesh.new()
    mesh.from_mesh(obj.data)
    geometry = mesh.verts[:] + mesh.edges[:] + mesh.faces[:]
    cut = bmesh.ops.bisect_plane(mesh, geom=geometry, plane_co=(0, 0, GROUND_Z), plane_no=(0, 0, 1), clear_outer=True)
    rim = [e for e in cut["geom_cut"] if isinstance(e, bmesh.types.BMEdge) and e.is_boundary]
    if rim:
        bmesh.ops.holes_fill(mesh, edges=rim, sides=0)
    bmesh.ops.translate(mesh, verts=mesh.verts[:], vec=(0, 0, SUNK))
    mesh.to_mesh(obj.data)
    mesh.free()
    print("PLATE", obj.name, len(obj.data.vertices))

bpy.ops.export_scene.fbx(filepath=target, object_types={'MESH'}, use_mesh_modifiers=False, apply_scale_options='FBX_SCALE_UNITS')
