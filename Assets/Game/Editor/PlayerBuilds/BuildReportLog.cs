using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.PlayerBuilds
{
    public static class BuildReportLog
    {
        private const int LargestAssetCount = 15;

        public static void Write(BuildReport report)
        {
            BuildSummary summary = report.summary;
            var text = new StringBuilder();
            text.AppendLine($"Build: {summary.result}, {summary.platform}, {summary.outputPath}");
            text.AppendLine($"Build: output {Megabytes(OutputSize(summary))}, time {summary.totalTime:mm\\:ss}, "
                + $"errors {summary.totalErrors}, warnings {summary.totalWarnings}");
            foreach (KeyValuePair<string, ulong> asset in LargestAssets(report))
                text.AppendLine($"Build:   {Megabytes(asset.Value),9}  {asset.Key}");
            Debug.Log(text.ToString());
        }

        private static ulong OutputSize(BuildSummary summary) =>
            File.Exists(summary.outputPath) ? (ulong)new FileInfo(summary.outputPath).Length : summary.totalSize;

        private static IEnumerable<KeyValuePair<string, ulong>> LargestAssets(BuildReport report) =>
            report.packedAssets
                .SelectMany(packed => packed.contents)
                .GroupBy(content => content.sourceAssetPath)
                .Select(group => new KeyValuePair<string, ulong>(group.Key, (ulong)group.Sum(content => (long)content.packedSize)))
                .OrderByDescending(asset => asset.Value)
                .Take(LargestAssetCount);

        private static string Megabytes(ulong bytes) => $"{bytes / (1024f * 1024f):0.00} MB";
    }
}
