// ***********************************************************************
// Assembly         : Assembly-CSharp-Editor
// Author           : Nguyen Khac Trieu (trieunk@yahoo.com)
// Created          : 02-20-2017
// Last Modified By : Nguyen Khac Trieu (trieunk@yahoo.com)
// Last Modified On : 02-20-2017
// ***********************************************************************
// <copyright file="UnityReporter.cs" company="FlexiData Co., Ltd Vietnam">
//     Copyright (c) 2016 FlexiData Co., Ltd Vietnam. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// ReSharper disable once CheckNamespace
namespace Nordeus.Build.Reporters
{
    using System;

    using UnityEngine;

    /// <summary>
    ///     Reports messages to Unity console.
    /// </summary>
    /// <seealso cref="Nordeus.Build.Reporters.BuildReporter" />
    public class UnityReporter : BuildReporter
    {
        /// <summary>
        ///     Indicates a successful build to the build environment.
        /// </summary>
        public override void IndicateSuccessfulBuild()
        {
        }

        /// <summary>
        ///     Logs the internal.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Severity out of bounds</exception>
        protected override void LogInternal(string message, MessageSeverity severity = MessageSeverity.Info)
        {
            switch (severity)
            {
                case MessageSeverity.Debug:
                    Debug.Log(message);
                    break;
                case MessageSeverity.Info:
                    Debug.Log(message);
                    break;
                case MessageSeverity.Warning:
                    Debug.LogWarning(message);
                    break;
                case MessageSeverity.Error:
                    Debug.LogError(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("severity", "Severity out of bounds");
            }
        }
    }
}