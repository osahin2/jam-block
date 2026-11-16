using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Core.Save.Tests
{
    public sealed class JsonFileSaveServiceTests
    {
        [Serializable]
        private sealed class Document
        {
            public int number;
            public string text;
        }

        private string _directory;
        private JsonFileSaveService _save;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), "CoreSaveTests_" + Guid.NewGuid().ToString("N"));
            _save = new JsonFileSaveService(_directory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
        }

        [Test]
        public void MissingDocument_LoadsAsNew()
        {
            Document loaded = _save.Load<Document>("missing");

            Assert.That(loaded.number, Is.EqualTo(0));
            Assert.That(loaded.text, Is.Null);
        }

        [Test]
        public void SavedDocument_LoadsBack()
        {
            _save.Save("doc", new Document { number = 7, text = "seven" });

            Document loaded = _save.Load<Document>("doc");

            Assert.That(loaded.number, Is.EqualTo(7));
            Assert.That(loaded.text, Is.EqualTo("seven"));
        }

        [Test]
        public void SavingAgain_ReplacesTheDocumentAndLeavesNoTemporaryFile()
        {
            _save.Save("doc", new Document { number = 1 });
            _save.Save("doc", new Document { number = 2 });

            Assert.That(_save.Load<Document>("doc").number, Is.EqualTo(2));
            Assert.That(Directory.GetFiles(_directory), Has.Length.EqualTo(1));
        }

        [Test]
        public void CorruptDocument_LoadsAsNewWithAWarning()
        {
            Directory.CreateDirectory(_directory);
            File.WriteAllText(Path.Combine(_directory, "doc.json"), "{ not json");

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Could not read 'doc'"));
            Document loaded = _save.Load<Document>("doc");

            Assert.That(loaded.number, Is.EqualTo(0));
        }
    }
}
