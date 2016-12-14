namespace Microsoft.Practices.WindowsAzure.MSBuildTasks
{
    using System;
    using Build.Framework;
    using Build.Utilities;

    public class GetDateTime : Task
    {
        DateTime dateTime;

        public GetDateTime()
        {
            Format = "g";
        }

        public string Format { get; set; }

        [Output]
        public string Text
        {
            get { return dateTime.ToString(Format); }
        }

        public bool Utc { get; set; }

        public override bool Execute()
        {
            dateTime = Utc ? DateTime.UtcNow : DateTime.Now;
            return true;
        }
    }
}