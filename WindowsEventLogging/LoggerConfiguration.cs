using System.Configuration;

namespace WindowsEventLogging
{
    public class LoggerConfiguration : ConfigurationSection
    {
        public static readonly LoggerConfiguration Current =
            (LoggerConfiguration)ConfigurationManager.GetSection("windowsEventLoggerConfig");

        [ConfigurationProperty("source", IsRequired=true)]
        public string Source
        {
            get { return (string)base["source"]; }
            set { base["source"] = value; }
        }

        [ConfigurationProperty("log", DefaultValue="Application",IsRequired=true)]
        public string Log
        {
            get { return (string)base["log"]; }
            set { base["log"] = value; }
        }
    }
}
