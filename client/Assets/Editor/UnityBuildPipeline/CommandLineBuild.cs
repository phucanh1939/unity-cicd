// ***********************************************************************
// Assembly         : Assembly-CSharp-Editor
// Author           : Nguyen Khac Trieu (trieunk@yahoo.com)
// Created          : 02-20-2017
// Last Modified By : Nguyen Khac Trieu (trieunk@yahoo.com)
// Last Modified On : 03-21-2017
// ***********************************************************************
// <copyright file="CommandLineBuild.cs" company="FlexiData Co., Ltd Vietnam">
//     Copyright (c) 2016 FlexiData Co., Ltd Vietnam. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// ReSharper disable once CheckNamespace

namespace Nordeus.Build
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Nordeus.Build.Reporters;

    using UnityEditor;

    using Utility;

    /// <summary>
    ///     The entry point to invoke build commands.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class CommandLineBuild
    {
        /// <summary>
        ///     The publish path command
        /// </summary>
        private const string PublishPathCommand = "-out";

        /// <summary>
        ///     The build number command
        /// </summary>
        private const string BuildNumberCommand = "-buildNumber";

        /// <summary>
        ///     The build version command
        /// </summary>
        private const string BuildVersionCommand = "-buildVersion";

        /// <summary>
        ///     The build reporter command
        /// </summary>
        private const string BuildReporterCommand = "-reporter";

        /// <summary>
        ///     The build target command
        /// </summary>
        private const string BuildTargetCommand = "-buildTarget";

        /// <summary>
        ///     The android texture compression command
        /// </summary>
        private const string AndroidTextureCompressionCommand = "-textureCompression";

        /// <summary>
        ///     The command start character
        /// </summary>
        private const char CommandStartCharacter = '-';

        /// <summary>
        ///     Gets or sets the android SDK root.
        /// </summary>
        /// <value>The android SDK root.</value>
        public static string AndroidSdkRoot
        {
            get
            {
                return EditorPrefs.GetString("AndroidSdkRoot");
            }

            set
            {
                EditorPrefs.SetString("AndroidSdkRoot", value);
            }
        }

        /// <summary>
        ///     Gets or sets the android NDK root.
        /// </summary>
        /// <value>The android NDK root.</value>
        public static string AndroidNdkRoot
        {
            get
            {
                return EditorPrefs.GetString("AndroidNdkRoot");
            }

            set
            {
                EditorPrefs.SetString("AndroidNdkRoot", value);
            }
        }

        /// <summary>
        ///     Commands the line build iOS.
        /// </summary>
        [MenuItem("Perform Build/iOS Command Line Build")]
        private static void CommandLineBuildiOs()
        {
            BuildReporter.Current.Log("Command line build\n------------------\n------------------");
            BuildInternal(BuildTargetGroup.iOS, BuildTarget.iOS);
        }

        /// <summary>
        ///     Commands the line build android.
        /// </summary>
        [MenuItem("Perform Build/Android Command Line Build")]
        private static void CommandLineBuildAndroid()
        {
            BuildReporter.Current.Log("Command line build\n------------------\n------------------");
            BuildInternal(BuildTargetGroup.Android, BuildTarget.Android);
        }

        /// <summary>
        ///     Performs the command line build by using the passed command line arguments.
        /// </summary>
        private static void Build()
        {
            EnsureValidSdkPaths();

            string publishPath, buildNumber, buildVersion, buildReporter, buildTarget, androidTextureCompression;

            var commandToValueDictionary = GetCommandLineArguments();

            // Extract our arguments from dictionary
            commandToValueDictionary.TryGetValue(PublishPathCommand, out publishPath);
            commandToValueDictionary.TryGetValue(BuildNumberCommand, out buildNumber);
            commandToValueDictionary.TryGetValue(BuildVersionCommand, out buildVersion);
            commandToValueDictionary.TryGetValue(BuildReporterCommand, out buildReporter);
            commandToValueDictionary.TryGetValue(BuildTargetCommand, out buildTarget);
            commandToValueDictionary.TryGetValue(AndroidTextureCompressionCommand, out androidTextureCompression);

            if (!string.IsNullOrEmpty(buildReporter))
            {
                BuildReporter.Current = BuildReporter.CreateReporterByName(buildReporter);
            }

            if (string.IsNullOrEmpty(buildTarget))
            {
                BuildReporter.Current.Log(
                    "No target was specified for this build.",
                    BuildReporter.MessageSeverity.Error);
            }
            else
            {
                var parsedBuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTarget);

                BuildInternal(
                    parsedBuildTarget == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS,
                    parsedBuildTarget,
                    publishPath: publishPath,
                    buildVersion: buildVersion,
                    buildNumber: buildNumber,
                    androidTextureCompression: androidTextureCompression);
            }
        }

        /// <summary>
        ///     Ensures the valid Android SDK paths is set: path should not contains white-space.
        ///     The path from environment variable is prefer since NETWORK SERVICE account use path from installed Android Studio.
        /// </summary>
        private static void EnsureValidSdkPaths()
        {
            var androidSdk = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(androidSdk) && !androidSdk.Equals(AndroidSdkRoot))
            {
                AndroidSdkRoot = androidSdk;
            }

            var androidNdk = Environment.GetEnvironmentVariable("ANDROID_NDK_HOME");
            if (!string.IsNullOrEmpty(androidNdk) && !androidNdk.Equals(AndroidNdkRoot))
            {
                AndroidNdkRoot = androidNdk;
            }
        }

        /// <summary>
        ///     Builds the internal.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group.</param>
        /// <param name="buildTarget">The build target.</param>
        /// <param name="publishPath">The publish path.</param>
        /// <param name="buildVersion">The build version.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <param name="androidTextureCompression">The android texture compression.</param>
        private static void BuildInternal(
            BuildTargetGroup buildTargetGroup,
            BuildTarget buildTarget,
            string publishPath = null,
            string buildVersion = null,
            string buildNumber = null,
            string androidTextureCompression = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(buildNumber))
                {
                    BundleVersionResolver.BuildNumber = int.Parse(buildNumber);
                    BundleVersionChecker.CreateNewBuildVersionClassFile(BundleVersionResolver.BuildNumber.Value);
                }

                if (!string.IsNullOrEmpty(buildVersion))
                {
                    BundleVersionResolver.PrettyVersion = buildVersion;
                }

                MobileTextureSubtarget? parsedTextureSubtarget = null;
                if (!string.IsNullOrEmpty(androidTextureCompression))
                {
                    parsedTextureSubtarget =
                        (MobileTextureSubtarget)Enum.Parse(typeof(MobileTextureSubtarget), androidTextureCompression);
                }

                if (string.IsNullOrEmpty(publishPath))
                {
                    publishPath = EditorUserBuildSettings.GetBuildLocation(buildTarget);
                    BuildReporter.Current.Log(
                        string.Format(
                            "Build Location for {0} not set. Defaulting to {1}",
                            buildTarget,
                            publishPath));
                }

                BundleVersionResolver.Setup(buildTarget);

                BuildReporter.Current.Log(string.Format("Switching Build Target to {0}", buildTarget));
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);

                BuildReporter.Current.Log("Starting Build!");

                Builder.Build(buildTarget, publishPath, parsedTextureSubtarget);
            }
            catch (Exception e)
            {
                BuildReporter.Current.Log(e.Message, BuildReporter.MessageSeverity.Error);
            }
        }

        /// <summary>
        ///     Gets all the command line arguments relevant to the build process.
        ///     All commands that don't have a value after them have their value at string.Empty.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        private static Dictionary<string, string> GetCommandLineArguments()
        {
            var commandToValueDictionary = new Dictionary<string, string>();
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith(CommandStartCharacter.ToString()))
                {
                    continue;
                }

                var command = args[i];
                var value = string.Empty;

                if (i < args.Length - 1 && !args[i + 1].StartsWith(CommandStartCharacter.ToString()))
                {
                    value = args[i + 1];
                    i++;
                }

                if (!commandToValueDictionary.ContainsKey(command))
                {
                    commandToValueDictionary.Add(command, value);
                }
                else
                {
                    BuildReporter.Current.Log(
                        "Duplicate command line argument " + command,
                        BuildReporter.MessageSeverity.Warning);
                }
            }

            return commandToValueDictionary;
        }
    }
}
