# Save

> Summary: Typed local persistence: a document is a plain `[Serializable]` class stored as one JSON file per key under `persistentDataPath`, read and written synchronously, with atomic writes.

## Code
- Folder: `Packages/com.base-setup.core/Runtime/Save/`
- Assembly: `Core.Save` (references VContainer for the installer only)
- Entry points:
  - `ISaveService` ([ISaveService.cs](../../../Packages/com.base-setup.core/Runtime/Save/ISaveService.cs)): `Load<T>(key)`, `Save<T>(key, document)`.
  - `RegisterCoreSave()` ([SaveInstaller.cs](../../../Packages/com.base-setup.core/Runtime/Save/SaveInstaller.cs)): registers the service, storing files in `persistentDataPath/save/`.

## How it works
- `Load<T>` reads `<key>.json` with `JsonUtility`. A missing file returns `new T()`. An unreadable file (invalid JSON or an IO error) logs a warning and also returns `new T()`, so a corrupt save costs progress but never blocks the game from starting.
- `Save<T>` writes the whole document to `<key>.json.tmp`, then swaps it in with `File.Replace` (or `File.Move` the first time). A crash during the write leaves the previous file intact.
- Every `Save` writes immediately. Nothing is cached or written later on pause or quit.
- Synchronous on purpose: documents are a few kilobytes, the write that matters most happens when the app is paused or quit and the OS may suspend it before an async write finishes, and callers stay simple.

## Rules & invariants
- Documents follow `JsonUtility` rules: public fields or `[SerializeField]`, no dictionaries, no properties.
- Keys are file names: keep them to letters, digits and dashes.
- Only `Core.Save` touches the save directory; modules and the game do not call `PlayerPrefs` or write files themselves.

## Limits
- **No in-memory cache and no flush on pause or quit.** Every writer today saves rarely (a completed level, a settings change), so an immediate write is simpler and loses nothing. Add a write-behind cache when something saves often (for example a slider saving on every drag frame).
- **No version field yet.** Nothing has been migrated, so a version would have no reader. When a document's format first changes, the document gets its own `version` field and `Load` callers migrate; that change also decides whether the service should handle versions centrally.
- **Serializer: `JsonUtility`.** No dependency, and current documents need no dictionaries. Newtonsoft is the fallback if a document needs them.

## Tests
`Packages/com.base-setup.core/Tests/Save/JsonFileSaveServiceTests.cs` (temporary directory per test): a missing document loads as new, a saved one loads back, saving again replaces it without leaving a temporary file, and a corrupt file loads as new with a warning.

## Related
- Used by [Levels](Levels.md), [Audio](Audio.md), [Haptics](Haptics.md)
