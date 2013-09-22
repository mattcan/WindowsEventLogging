using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace WindowsEventLogging
{
    /// <summary>
    /// Contains methods for writing to the
    /// Windows Event Log
    /// </summary>
    public static class Logger
    {
        #region Informational
        /// <summary>
        /// Writes an informational message to the log
        /// Note: uses a random event id between 0 and 100
        /// </summary>
        /// <param name="Message">Message you would like to send eg "User x logged in at yyyy/mm/dd"</param>
        public static bool WriteInfo(string Message)
        {
            return Logger.WriteInfo(Message, new Random().Next(0, 99));
        }

        /// <summary>
        /// Writes an informational message to the log
        /// These come in handy when running in debug mode so we suggest you wrap this in a compiler flag
        /// </summary>
        /// <param name="Message">Message you would like to send eg "User x logged in at yyyy/mm/dd"</param>
        /// <param name="EventId">An ID number to manage types of events</param>
        public static bool WriteInfo(string Message, int EventId)
        {
            if (string.IsNullOrEmpty(Message)) { throw new ArgumentException("Should not be null or empty", "Message"); }

            return Logger.WriteEvent(Message, EventLogEntryType.Information, EventId);
        }
        #endregion

        #region Warning
        /// <summary>
        /// Writes a warning to the log
        /// Note: Uses a random Event ID between 100 and 200
        /// </summary>
        /// <param name="Message">Message to give you an idea about what happened eg "User x failed password check at yyyy/mm/dd"</param>
        public static bool WriteWarning(string Message)
        {
            return Logger.WriteWarning(Message, new Random().Next(100, 199));
        }

        /// <summary>
        /// Writes a warning to the log.
        /// These are types of events that you might want to check once a day or once a week
        /// </summary>
        /// <param name="Message">Message to give you an idea about what happened eg "User x failed password check at yyyy/mm/dd"</param>
        /// <param name="EventId">An ID number to manage types of events</param>
        public static bool WriteWarning(string Message, int EventId)
        {
            if (string.IsNullOrEmpty(Message)) { throw new ArgumentException("Should not be null or empty", "Message"); }

            return Logger.WriteEvent(Message, EventLogEntryType.Warning, EventId);
        }
        #endregion

        #region Error
        /// <summary>
        /// Writes an error message to the event log
        /// Note: uses a random Event ID between 200 and 300
        /// </summary>
        /// <param name="ShortDescription"></param>
        /// <param name="StackTrace"></param>
        public static bool WriteError(string ShortDescription, string StackTrace)
        {
            return Logger.WriteError(ShortDescription, StackTrace, new Random().Next(200, 299));
        }

        /// <summary>
        /// Writes an error message to the event log
        /// Make sure you have some notification around these
        /// </summary>
        /// <param name="ShortDescription">A short description of what occurred</param>
        /// <param name="StackTrace">The stack trace for later debugging</param>
        /// <param name="EventId">An ID number to manage types of events</param>
        public static bool WriteError(string ShortDescription, string StackTrace, int EventId)
        {
            if (string.IsNullOrEmpty(ShortDescription)) { throw new ArgumentException("Should not be null or empty", "ShortDescription"); }
            if (string.IsNullOrEmpty(StackTrace)) { throw new ArgumentException("Should not be null or empty", "StackTrace"); }

            string message = string.Format(
                "Description: {1}{0}--------Stack Trace---------{0}{2}",
                Environment.NewLine,
                ShortDescription,
                StackTrace
            );

            return Logger.WriteEvent(message, EventLogEntryType.Error, EventId);
        }
        #endregion

        /// <summary>
        /// The main method for writing to the event log
        /// </summary>
        /// <param name="Message">The message displayed in the event body</param>
        /// <param name="EventType">The type of event occurring (Info, Error, Warning)</param>
        /// <param name="EventId">An ID number that is used for tracking specific types of errors or to launch processes</param>
        private static bool WriteEvent(string Message, EventLogEntryType EventType, int EventId)
        {
            // set source and log
            string source = LoggerConfiguration.Current.Source;
            string log = LoggerConfiguration.Current.Log;

            // check for source and create if non-existant
            VerifySource(source, log);

            // write the event
            try
            {
                EventLog.WriteEntry(source, Message, EventType, EventId);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private static void VerifySource(string Source, string Log)
        {
            string eventLogName = Log;
            string sourceName = Source;
            EventLog winEventLog;
            winEventLog = new EventLog();
            winEventLog.Log = eventLogName;

            // set default event source (to be same as event log name) if not passed in
            if ((sourceName == null) || (sourceName.Trim().Length == 0))
            {
                sourceName = eventLogName;
            }

            winEventLog.Source = sourceName;

            // Extra Raw event data can be added (later) if needed
            byte[] rawEventData = Encoding.ASCII.GetBytes("");

            /// Check whether the Event Source exists. It is possible that this may
            /// raise a security exception if the current process account doesn't
            /// have permissions for all sub-keys under 
            /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog

            // Check whether registry key for source exists

            string keyName = @"SYSTEM\CurrentControlSet\Services\EventLog\" + eventLogName;

            RegistryKey rkEventSource = Registry.LocalMachine.OpenSubKey(keyName + @"\" + sourceName);

            // Check whether key exists
            if (rkEventSource == null)
            {
                /// Key does not exist. Create key which represents source
                Registry.LocalMachine.CreateSubKey(keyName + @"\" + sourceName);
            }

            /// Now validate that the .NET Event Message File, EventMessageFile.dll (which correctly
            /// formats the content in a Log Message) is set for the event source
            object eventMessageFile = rkEventSource.GetValue("EventMessageFile");

            /// If the event Source Message File is not set, then set the Event Source message file.
            if (eventMessageFile == null)
            {
                /// Source Event File Doesn't exist - determine .NET framework location,
                /// for Event Messages file.
                RegistryKey dotNetFrameworkSettings = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\.NetFramework\"
                    );

                if (dotNetFrameworkSettings != null)
                {
                    object dotNetInstallRoot = dotNetFrameworkSettings.GetValue(
                        "InstallRoot",
                        null,
                        RegistryValueOptions.None
                        );

                    if (dotNetInstallRoot != null)
                    {
                        string eventMessageFileLocation =
                            dotNetInstallRoot.ToString() + "v" +
                            System.Environment.Version.Major.ToString() + "." +
                            System.Environment.Version.Minor.ToString() + "." +
                            System.Environment.Version.Build.ToString() +
                            @"\EventLogMessages.dll";

                        /// Validate File exists
                        if (System.IO.File.Exists(eventMessageFileLocation))
                        {
                            /// The Event Message File exists in the anticipated location on the
                            /// machine. Set this value for the new Event Source

                            // Re-open the key as writable
                            rkEventSource = Registry.LocalMachine.OpenSubKey(
                                keyName + @"\" + sourceName,
                                true
                                );

                            // Set the "EventMessageFile" property
                            rkEventSource.SetValue(
                                "EventMessageFile",
                                eventMessageFileLocation,
                                RegistryValueKind.String
                                );
                        }
                    }
                }

                dotNetFrameworkSettings.Close();
            }

            rkEventSource.Close();
        }
    }
}
