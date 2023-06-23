using UnityEditor.Build.Reporting;

// ReSharper disable once CheckNamespace
namespace Nordeus.Build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Nordeus.Build.Reporters;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    ///     The main build logic.
    /// </summary>
    internal static class Builder
    {
        /// <summary>
        ///     Builds the specified build target.
        /// </summary>
        /// <param name="parsedBuildTarget">Build target to build.</param>
        /// <param name="buildPath">Output path for the build.</param>
        /// <param name="parsedTextureSubtarget">Texture compression subtarget for Android.</param>
        /// <exception cref="ArgumentException"><paramref name="parsedBuildTarget" /> is not a supported build target.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">
        ///     The directory specified by <paramref name="buildPath" /> is a file.-or-The network name
        ///     is not known.
        /// </exception>
        public static void Build(
            BuildTarget parsedBuildTarget,
            string buildPath,
            MobileTextureSubtarget? parsedTextureSubtarget = null)
        {
            Directory.CreateDirectory(buildPath);

            switch (parsedBuildTarget)
            {
                case BuildTarget.Android:
                    BuildAndroid(buildPath, parsedTextureSubtarget);
                    break;
                case BuildTarget.iOS:
                    BuildIos(buildPath);
                    break;
                default:
                    throw new ArgumentException(parsedBuildTarget + " is not a supported build target.");
            }
        }

        /// <summary>
        ///     Builds an APK at the specified path with the specified texture compression.
        /// </summary>
        /// <param name="buildPath">Path for the output APK.</param>
        /// <param name="textureCompression">If not null, will override the texture compression sub-target.</param>
        private static void BuildAndroid(string buildPath, MobileTextureSubtarget? textureCompression = null)
        {
            if (textureCompression != null)
            {
                EditorUserBuildSettings.androidBuildSubtarget = textureCompression.Value;
            }

            var buildReport = BuildPipeline.BuildPlayer(
                GetEnabledScenePaths().ToArray(),
                buildPath,
                BuildTarget.Android,
                BuildOptions.StrictMode);

            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                BuildReporter.Current.IndicateSuccessfulBuild();
            }
            else
            {
                BuildReporter.Current.Log("Build Failed", BuildReporter.MessageSeverity.Error);
            }
        }

        /// <summary>
        ///     Builds an XCode project at the specified path.
        /// </summary>
        /// <param name="buildPath">Path for the XCode project.</param>
        private static void BuildIos(string buildPath)
        {
            var options = BuildOptions.StrictMode | BuildOptions.AcceptExternalModificationsToPlayer;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                options |= BuildOptions.SymlinkSources;
            }

            var buildReport = BuildPipeline.BuildPlayer(
                GetEnabledScenePaths().ToArray(),
                buildPath,
                BuildTarget.iOS,
                options);

            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                if (BuildReporter.Current != null)
                {
                    BuildReporter.Current.IndicateSuccessfulBuild();
                }
            }
            else
            {
                BuildReporter.Current.Log("Build Failed", BuildReporter.MessageSeverity.Error);
            }
        }

        /// <summary>
        ///     Returns a list of all the enabled scenes.
        /// </summary>
        /// <returns>The List&lt;System.String&gt;.</returns>
        private static List<string> GetEnabledScenePaths()
        {
            return EditorBuildSettings.scenes.Select(scene => scene.path).ToList();
        }
    }
}