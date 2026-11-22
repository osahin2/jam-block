using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.PlayerBuilds
{
    public static class PlayerBuild
    {
        private const string OutputRoot = "Build";
        private const string FileName = "RollicCaseStudy";

        [MenuItem("Build/Android APK (Development)")]
        public static void AndroidDevelopment() =>
            BuildAndroid("dev", BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.CompressWithLz4);

        [MenuItem("Build/Android APK (Release)")]
        public static void AndroidRelease() => BuildAndroid("release", BuildOptions.CompressWithLz4HC);

        [MenuItem("Build/iOS Xcode Project")]
        public static void IosXcodeProject() =>
            Run(BuildTarget.iOS, Path.Combine(OutputRoot, "iOS"), BuildOptions.CompressWithLz4HC);

        private static void BuildAndroid(string flavor, BuildOptions options)
        {
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            string apk = $"{FileName}-{PlayerSettings.bundleVersion}-{flavor}.apk";
            Run(BuildTarget.Android, Path.Combine(OutputRoot, "Android", apk), options);
        }

        private static void Run(BuildTarget target, string location, BuildOptions options)
        {
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                target = target,
                targetGroup = BuildPipeline.GetBuildTargetGroup(target),
                locationPathName = location,
                options = options,
            });
            BuildReportLog.Write(report);
            if (Application.isBatchMode && report.summary.result != BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }
    }
}
