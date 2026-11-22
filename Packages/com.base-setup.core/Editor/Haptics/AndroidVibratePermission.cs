using System.IO;
using System.Xml;
using UnityEditor.Android;

namespace Core.Haptics.Editor
{
    internal sealed class AndroidVibratePermission : IPostGenerateGradleAndroidProject
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";
        private const string Permission = "android.permission.VIBRATE";
        private const string UsesPermission = "uses-permission";

        public int callbackOrder => 0;

        // Unity adds VIBRATE on its own only when the scripts call Handheld.Vibrate; the backend calls Vibrator directly.
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            var manifest = new XmlDocument();
            manifest.Load(manifestPath);
            if (Declares(manifest))
                return;

            manifest.DocumentElement.AppendChild(PermissionElement(manifest));
            manifest.Save(manifestPath);
        }

        private static bool Declares(XmlDocument manifest)
        {
            foreach (XmlElement element in manifest.DocumentElement.GetElementsByTagName(UsesPermission))
            {
                if (element.GetAttribute("name", AndroidNamespace) == Permission)
                    return true;
            }
            return false;
        }

        private static XmlElement PermissionElement(XmlDocument manifest)
        {
            var element = manifest.CreateElement(UsesPermission);
            var name = manifest.CreateAttribute("android", "name", AndroidNamespace);
            name.Value = Permission;
            element.Attributes.Append(name);
            return element;
        }
    }
}
