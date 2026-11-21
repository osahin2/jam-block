using System;
using Game.Data.Levels;
using Game.Levels;
using Game.Logic.Levels;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    [Serializable]
    public sealed class LevelDocument
    {
        private static readonly LevelData EmptyLevel = new LevelData();

        [SerializeField] private LevelAsset _asset;
        [SerializeField] private bool _isNew;
        [SerializeField] private LevelDraft _draft;

        public LevelAsset Asset => _asset;
        public LevelDraft Draft => _draft;
        public bool IsOpen => _asset != null || _isNew;
        // A new level counts as changed once something is on its board, so leaving an untouched one asks nothing.
        public bool HasUnsavedChanges => IsOpen && !_draft.Matches(_isNew ? EmptyLevel : _asset.Level);
        public bool CanRevert => !_isNew && HasUnsavedChanges;

        // The working copy is never saved, so after an editor restart the level is loaded again from its asset.
        public void EnsureDraft()
        {
            if (_draft != null)
                return;
            _draft = LevelDraft.Create();
            if (_asset != null)
                _draft.Load(_asset.Level);
        }

        // Opening the open level again reloads it from its asset, which is how Revert works.
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

        public void Duplicate()
        {
            if (!IsOpen)
                return;
            _asset = null;
            _isNew = true;
            _draft.Load(_draft.Level);
        }

        public void Unload()
        {
            _asset = null;
            _isNew = false;
            _draft.Load(EmptyLevel);
        }

        public string Name(LevelLibrary library)
        {
            if (!_isNew)
                return _asset != null ? _asset.name : string.Empty;
            return library != null ? $"{library.NextName()} (new)" : "New level";
        }

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
