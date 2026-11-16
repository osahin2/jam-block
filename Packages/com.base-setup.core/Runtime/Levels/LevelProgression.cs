using System;
using Core.Save;

namespace Core.Levels
{
    internal sealed class LevelProgression : ILevelProgression
    {
        internal const string SaveKey = "level-progress";

        [Serializable]
        internal sealed class Document
        {
            public int completedLevels;
        }

        private readonly ISaveService _save;
        private readonly LevelLoop _loop;
        private readonly Document _document;

        public int CurrentLevelNumber => _document.completedLevels + 1;

        public int CurrentLevelIndex
        {
            get
            {
                int completed = _document.completedLevels;
                if (completed < _loop.LevelCount) return completed;

                int loopLength = _loop.LevelCount - _loop.LoopStartIndex;
                return _loop.LoopStartIndex + (completed - _loop.LevelCount) % loopLength;
            }
        }

        public LevelProgression(ISaveService save, LevelLoop loop)
        {
            _save = save;
            _loop = loop;
            _document = save.Load<Document>(SaveKey);
        }

        public void CompleteCurrentLevel()
        {
            _document.completedLevels++;
            _save.Save(SaveKey, _document);
        }
    }
}
