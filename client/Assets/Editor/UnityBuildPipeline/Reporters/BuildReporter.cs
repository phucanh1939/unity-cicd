// ***********************************************************************
// Assembly         : Assembly-CSharp-Editor
// Author           : Nguyen Khac Trieu (trieunk@yahoo.com)
// Created          : 02-20-2017
// Last Modified By : Nguyen Khac Trieu (trieunk@yahoo.com)
// Last Modified On : 02-20-2017
// ***********************************************************************
// <copyright file="BuildReporter.cs" company="FlexiData Co., Ltd Vietnam">
//     Copyright (c) 2016 FlexiData Co., Ltd Vietnam. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// ReSharper disable once CheckNamespace
namespace Nordeus.Build.Reporters
{
    using JetBrains.Annotations;

    /// <summary>
    ///     Base class for reporting messages to the build environment.
    /// </summary>
    [PublicAPI]
    public abstract class BuildReporter
    {
        /// <summary>
        ///     The _current reporter
        /// </summary>
        private static BuildReporter _currentReporter;

        /// <summary>
        ///     Enum MessageSeverity
        /// </summary>
        public enum MessageSeverity
        {
            /// <summary>
            ///     The debug
            /// </summary>
            Debug,

            /// <summary>
            ///     The information
            /// </summary>
            Info,

            /// <summary>
            ///     The warning
            /// </summary>
            Warning,

            /// <summary>
            ///     The error
            /// </summary>
            Error
        }

        /// <summary>
        ///     Gets or sets the current global build reporter. By default, it's the Unity (Debug.Log) reporter.
        /// </summary>
        /// <value>The current.</value>
        public static BuildReporter Current
        {
            get
            {
                return _currentReporter ?? (_currentReporter = new UnityReporter());
            }

            set
            {
                _currentReporter = value;
            }
        }

        /// <summary>
        ///     Gets or sets minimum severity the build reporter should report.
        /// </summary>
        /// <value>The minimum severity.</value>
        [UsedImplicitly]
        public MessageSeverity MinimumSeverity { get; set; }

        /// <summary>
        ///     Creates a build reporter by name. Returns null in case it cannot create one.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The BuildReporter.</returns>
        public static BuildReporter CreateReporterByName(string name)
        {
            switch (name)
            {
                case "Unity":
                    return new UnityReporter();
                case "TeamCity":
                    return new TeamCityReporter();
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Reports a message to the build environment.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        public virtual void Log(string message, MessageSeverity severity = MessageSeverity.Info)
        {
            if (severity >= this.MinimumSeverity)
            {
                this.LogInternal(message, severity);
            }
        }

        /// <summary>
        ///     Indicates a successful build to the build environment.
        /// </summary>
        public abstract void IndicateSuccessfulBuild();

        /// <summary>
        ///     Logs the internal.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        protected abstract void LogInternal(string message, MessageSeverity severity);
    }
}