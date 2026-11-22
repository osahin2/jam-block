# Save

> Typed local persistence: one `[Serializable]` document per key, stored as JSON under `persistentDataPath/save/`, read and written synchronously.

**Code:** `Core.Save`. **Entry:** `ISaveService.Load<T>(key)`, `Save<T>(key, document)`; `RegisterCoreSave()`.

## Rules
- A missing or unreadable file returns `new T()` (a warning for unreadable): a corrupt save costs progress but never blocks startup.
- Writes go to a temp file swapped in with `File.Replace`, so a crash keeps the previous file. Every `Save` writes at once; nothing is cached.
- Synchronous on purpose: documents are small, and the OS may suspend a paused app before an async write ends.
- `JsonUtility` rules apply (no dictionaries, no properties). Keys are file names: letters, digits, dashes.
- No version field yet; a document gets one when its format first changes.
