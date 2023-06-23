// ***********************************************************************
// Assembly         : Assembly-CSharp-Editor
// Author           : Nguyen Khac Trieu (trieunk@yahoo.com)
// Created          : 02-20-2017
// Last Modified By : Nguyen Khac Trieu (trieunk@yahoo.com)
// Last Modified On : 02-22-2017
// ***********************************************************************
// <copyright file="BundleVersionResolver.cs" company="FlexiData Co., Ltd Vietnam">
//     Copyright (c) 2016 FlexiData Co., Ltd Vietnam. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// ReSharper disable once CheckNamespace

namespace Nordeus.Build
{
    using JetBrains.Annotations;

    using UnityEditor;

    /// <summary>
    ///     Sets up the appropriate player settings to properly represent the game version
    ///  and build number for different platforms and versions of Unity.
    /// </summary>
    [PublicAPI]
    internal static class BundleVersionResolver
    {
        /// <summary>
        ///     The iOS target
        /// </summary>
        private const BuildTarget IosTarget = BuildTarget.iOS;

        /// <summary>
        ///     Gets or sets the pretty version of the game, for example 0.123f
        /// </summary>
        /// <value>The pretty version.</value>
        public static string PrettyVersion { get; set; }

        /// <summary>
        ///     Gets or sets the number of the build. Corresponds to bundle version code for Android and build number for iOS.
        /// </summary>
        /// <value>The build number.</value>
        public static int? BuildNumber { get; set; }

        /// <summary>
        ///     Setups player settings with the specified pretty version and build number.
        /// </summary>
        /// <param name="target">The target.</param>
        public static void Setup(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                SetupAndroid();
            }
            else if (target == IosTarget)
            {
                SetupIos();
            }
        }

        /// <summary>
        ///     Setups the iOS version for Unity 5.3+.
        /// </summary>
        private static void SetupIos()
        {
            if (PrettyVersion != null)
            {
                PlayerSettings.bundleVersion = PrettyVersion;
            }

            if (BuildNumber != null)
            {
                PlayerSettings.iOS.buildNumber = BuildNumber.Value.ToString();
            }
        }

        /// <summary>
        ///     Setups the android.
        /// </summary>
        private static void SetupAndroid()
        {
            if (PrettyVersion != null)
            {
                PlayerSettings.bundleVersion = PrettyVersion;
            }

            if (BuildNumber != null)
            {
                PlayerSettings.Android.bundleVersionCode = BuildNumber.Value;
            }
        }
    }
}