namespace Microsoft.Practices.WindowsAzure.MSBuildTasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Build.Framework;
    using Build.Utilities;

    public class RegexReplace : Task
    {
        private ITaskItem[] files;
        private bool warnOnNoMatch = true;

        [Required]
        public ITaskItem[] Files
        {
            get { return files; }
            set { files = value; }
        }

        public bool IgnoreCase { get; set; }

        public bool IgnorePatternWhitespace { get; set; }

        private RegexOptions Options
        {
            get
            {
                RegexOptions result = RegexOptions.Compiled | RegexOptions.Multiline;

                if (IgnoreCase)
                    result |= RegexOptions.IgnoreCase;
                if (IgnorePatternWhitespace)
                    result |= RegexOptions.IgnorePatternWhitespace;
                if (RightToLeft)
                    result |= RegexOptions.RightToLeft;

                return result;
            }
        }

        [Required]
        public string Pattern { get; set; }

        [Required]
        public string Replacement { get; set; }

        public bool RightToLeft { get; set; }

        public bool WarnOnNoMatch
        {
            get { return warnOnNoMatch; }
            set { warnOnNoMatch = value; }
        }

        public override bool Execute()
        {
            Regex regex;

            Log.LogMessage(MessageImportance.Low, "Pattern = {0}", Pattern);
            Log.LogMessage(MessageImportance.Low, "Replacement = {0}", Replacement);

            try
            {
                regex = new Regex(Pattern, Options);
            }
            catch (Exception ex)
            {
                Log.LogError("Pattern error: {0}", ex.Message);
                return false;
            }

            bool errors = false;

            foreach (ITaskItem file in files)
            {
                try
                {
                    string fileSpec = Path.GetFullPath(file.ItemSpec);
                    string originalText = File.ReadAllText(fileSpec);
                    string replacementText = regex.Replace(originalText, Replacement);

                    if (WarnOnNoMatch && !regex.IsMatch(originalText))
                        Log.LogWarning("No matches in '{0}'.", fileSpec);

                    if (originalText != replacementText)
                    {
                        File.WriteAllText(fileSpec, replacementText);
                        Log.LogMessage("Changed '{0}'.", fileSpec);
                    }
                    else
                        Log.LogMessage("Skipped '{0}' (no changes)", fileSpec);
                }
                catch (Exception ex)
                {
                    Log.LogError("File error: {0}", ex.Message);
                    errors = true;
                }
            }

            return !errors;
        }
    }
}