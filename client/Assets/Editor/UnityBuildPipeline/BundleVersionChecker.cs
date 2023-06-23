namespace Utility
{
    using System.IO;

    internal class BundleVersionChecker
    {
        /// <summary>
        /// Class name to use when referencing from code.
        /// </summary>
        const string ClassName = "CurrentBundleVersion";

        const string TargetCodeFile = "Assets/Scripts/Utility/" + ClassName + ".cs";

        public static string CreateNewBuildVersionClassFile(int buildNumber)
        {
            using (StreamWriter writer = new StreamWriter(TargetCodeFile, false))
            {
                string code = GenerateCode(buildNumber);
                writer.WriteLine("{0}", code);
            }
            return TargetCodeFile;
        }

        /// <summary>
        /// Regenerates (and replaces) the code for ClassName with new bundle version id.
        /// </summary>
        /// <returns>
        /// Code to write to file.
        /// </returns>
        /// <param name='buildNumber'>
        /// New bundle version.
        /// </param>
        private static string GenerateCode(int buildNumber)
        {
            var code = "public static class " + ClassName + "\r\n{\r\n";
            code += string.Format("\tpublic static readonly int BuildNumber = {0};", buildNumber);
            code += "\r\n}\r\n";
            return code;
        }
    }
}