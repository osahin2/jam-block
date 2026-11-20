---
name: unity-editor
description: Compile, test and change this Unity 2022.3 project from the shell in batch mode with the `unity` CLI - check for compile errors after script changes, run EditMode tests, and edit scenes, prefabs, assets and project settings through editor scripts run with `-executeMethod`. Use whenever code or assets change or a result must be checked in Unity.
---

# Working with Unity 2022.3 in batch mode

The live `unity command` bridge needs the Pipeline package, which needs Unity 6; it does not work here. Every Unity action is a separate batch-mode run of the closed project. Batch mode cannot open the project while the editor has it open: ask the developer to close it first. One run takes about 10-30 s once `Library/` exists.

Keep logs and reports in the session scratchpad, not in the repository. Batch logs contain an access token on the command line: never paste them anywhere.

## After editing scripts: tests compile everything
`unity test . --mode EditMode --output <scratch>/editmode.xml` (optional `--filter <name>`).
- It prints nothing on success. Read the report: the `<test-run>` line has `result`, `total`, `passed`, `failed`; failing cases have `result="Failed"`.
- On a compile error it exits with code 6, prints `Scripts have compiler errors`, and writes no report. The errors are in `~/Library/Logs/Unity/Editor.log`: `grep -n "error CS"`.
- Expected warnings from tests (`[Core.Audio] Sfx cue 'Cue' has no clips.`, `[Core.Save] Could not read 'doc'`) are asserted by those tests; they are not problems.

Compile only, without tests: `unity run . --no-tail --log-file <scratch>/compile.log`, then `grep -n "error CS\|warning CS" <scratch>/compile.log`.

## Editor scripts: scenes, prefabs, assets, settings
Never edit `.unity`, `.prefab`, `.asset`, `.meta` or `ProjectSettings/` by hand. Write a static method in an editor-only assembly and run it:

`unity run . --no-tail --log-file <scratch>/run.log -- -executeMethod <Namespace.Class.Method>`

- `unity run` already passes `-batchmode -quit -projectPath -logFile`; forwarding `-quit` again is refused. Arguments after `--` go to Unity unchanged (`-buildTarget Android` is accepted).
- The method runs after compilation and the editor quits when it returns. Log what it did with `Debug.Log("<Tag>: ...")` and grep the log for the tag; an exception shows up as `Exception` in the log.
- Use `AssetDatabase` for every asset change (`DeleteAsset`, `MoveAsset`, `CreateAsset`, `SaveAssets`) so `.meta` files stay consistent. For prefabs: `PrefabUtility.LoadPrefabContents` / `SaveAsPrefabAsset` / `UnloadPrefabContents`; for scenes: `EditorSceneManager.OpenScene` / `SaveScene`.
- Project settings: the `PlayerSettings` and `EditorBuildSettings` APIs. A setting without an API (for example `activeInputHandler`) goes through `new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0])`.
- A one-off setup script can delete its own folder with `AssetDatabase.DeleteAsset` at the end; the run finishes normally.
- A file deleted from disk leaves an orphan `.meta`; the next batch run removes it ("A meta data file (.meta) exists but its asset ... can't be found").
- Check `git status` and `git diff` after every run: it shows exactly which serialized files changed.

## Packages
After editing `Packages/manifest.json`, the next batch run resolves packages (the 2022.3 Package Manager is x86_64 and needs Rosetta 2). Confirm the versions in `Packages/packages-lock.json`. Versions bundled with 2022.3.62f2: Input System 1.14.0, Test Framework 1.1.33, TextMeshPro 3.0.7, uGUI 1.0.0, URP 14.0.12.

## Play mode
Play mode cannot be driven from the shell on 2022.3. Cover rules with EditMode tests; for a visual or input check, describe what to look at and ask the developer to run it in the editor.
