// ***********************************************************************
// Assembly         : Assembly-CSharp-Editor
// Author           : Nguyen Khac Trieu (trieunk@yahoo.com)
// Created          : 02-20-2017
// Last Modified By : Nguyen Khac Trieu (trieunk@yahoo.com)
// Last Modified On : 02-20-2017
// ***********************************************************************
// <copyright file="TeamCityReporter.cs" company="FlexiData Co., Ltd Vietnam">
//     Copyright (c) 2016 FlexiData Co., Ltd Vietnam. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// ReSharper disable once CheckNamespace
namespace Nordeus.Build.Reporters
{
    using UnityEngine;

    /// <summary>
    ///     Reports messages to TeamCity.
    /// </summary>
    /// <seealso cref="Nordeus.Build.Reporters.UnityReporter" />
    public class TeamCityReporter : UnityReporter
    {
        /// <summary>
        ///     Indicates the successful build.
        /// </summary>
        public override void IndicateSuccessfulBuild()
        {
            base.IndicateSuccessfulBuild();

            // Magic string to indicate successful build.
            Debug.Log("Successful build ~0xDEADBEEF");
        }

        /// <summary>
        ///     Logs the internal.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        protected override void LogInternal(string message, MessageSeverity severity = MessageSeverity.Info)
        {
            base.LogInternal(message, severity);

            if (severity == MessageSeverity.Error)
            {
                Debug.Log("##teamcity[message text='" + message + "'" + "status='ERROR']");
            }
        }
    }
}