using System;
using Game.Data.Levels;
using Game.Editor.LevelEditor.Levels;
using Game.Levels;
using Game.Logic.Levels;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.LevelEditor.Window
{
    // The level open in the editor: a level asset, a new level that has no asset until its first save, or none. It is
    // edited through its working copy, and only a copy without errors is written, so a level with errors never becomes
    // or changes an asset. The window keeps it in a serialized field, so it survives script reloads.
    [Serializable]
    public sealed class LevelDocument
    {
        private static readonly LevelData EmptyLevel = new LevelData();

        [SerializeField] private LevelAsset _asset;
        // A new level has no asset until its first save.
        [SerializeField] private bool _isNew;
        [SerializeField] private LevelDraft _draft;

        public LevelAsset Asset => _asset;
        public LevelDraft Draft => _draft;
        public bool IsOpen => _asset != null || _isNew;
        // A new level counts as changed once something is on its board, so leaving an untouched one asks nothing.
        public bool HasUnsavedChanges => IsOpen && !_draft.Matches(_isNew ? EmptyLevel : _asset.Level);
        // A new level has no saved state to go back to.
        public bool CanRevert => !_isNew && HasUnsavedChanges;

        // The working copy is never saved: after the editor restarts the document remembers its asset but not the copy,
        // so the level is loaded again from the asset, and a new level starts empty again.
        public void EnsureDraft()
        {
            if (_draft != null)
                return;
            _draft = LevelDraft.Create();
            if (_asset != null)
                _draft.Load(_asset.Level);
        }

        // Opening the level that is already open loads it again from its asset, which is how Revert works.
        public void Open(LevelAsset asset)
        {
            _asset = asset;
            _isNew = false;
            _draft.Load(asset.Level);
        }

        public void StartNew()
        {
            _asset = null;
            _isNew = true;
            _draft.Load(EmptyLevel);
        }

        // A new level that starts as a copy of the open one as it is in the window, unsaved changes included. Its first
        // save adds it to the game as a level of its own; the original keeps what was last saved.
        public void Duplicate()
        {
            if (!IsOpen)
                return;
            _asset = null;
            _isNew = true;
            _draft.Load(_draft.Level);
        }

        // Nothing open, as after the open level was deleted.
        public void Unload()
        {
            _asset = null;
            _isNew = false;
            _draft.Load(EmptyLevel);
        }

        // A new level shows the name its first save will give it.
        public string Name(LevelLibrary library)
        {
            if (!_isNew)
                return _asset != null ? _asset.name : string.Empty;
            return library != null ? $"{library.NextName()} (new)" : "New level";
        }

        // False while the copy has errors, and for a new level when there is no library to add it to.
        public bool TrySave(LevelLibrary library)
        {
            if (!IsOpen || LevelValidator.Validate(_draft.Level).HasErrors)
                return false;
            if (_isNew)
            {
                if (library == null)
                    return false;
                _asset = library.Create();
                _isNew = false;
            }
            _draft.WriteTo(_asset.Level);
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssetIfDirty(_asset);
            return true;
        }

        public void Close() => _draft.Discard();
    }
}
