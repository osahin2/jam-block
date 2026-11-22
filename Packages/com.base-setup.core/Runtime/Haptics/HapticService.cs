using System;
using Core.Save;

namespace Core.Haptics
{
    internal sealed class HapticService : IHapticService
    {
        internal const string SaveKey = "haptic-settings";

        [Serializable]
        internal sealed class Document
        {
            public bool enabled = true;
        }

        private readonly ISaveService _save;
        private readonly IHapticBackend _backend;
        private readonly Document _document;

        public bool Enabled
        {
            get => _document.enabled;
            set
            {
                if (value == _document.enabled)
                    return;

                _document.enabled = value;
                _save.Save(SaveKey, _document);
            }
        }

        public HapticService(ISaveService save, IHapticBackend backend)
        {
            _save = save;
            _backend = backend;
            _document = save.Load<Document>(SaveKey);
        }

        public void Play(HapticPreset preset)
        {
            if (_document.enabled)
                _backend.Play(preset);
        }
    }
}
