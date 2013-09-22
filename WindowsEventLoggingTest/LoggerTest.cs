using WindowsEventLogging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;

namespace WindowsEventLoggingTest
{
    
    
    /// <summary>
    ///This is a test class for LoggerTest and is intended
    ///to contain all LoggerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LoggerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Warning
        /// <summary>
        /// Empty string test for WriteWarning
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteWarningTest1()
        {
            string Message = string.Empty;
            Logger.WriteWarning(Message);
        }

        /// <summary>
        /// Null string test for WriteWarning
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteWarningTest2()
        {
            string Message = null;
            Logger.WriteWarning(Message);
        }

        /// <summary>
        /// Pass in correct string
        /// </summary>
        [TestMethod()]
        public void WriteWarningTest3()
        {
            string Message = "WarningTest3";
            Assert.IsTrue(Logger.WriteWarning(Message));
        }

        /// <summary>
        ///A test for WriteWarning
        ///</summary>
        [TestMethod()]
        public void WriteWarningTest()
        {
            string Message = "WARNING WARNING!";
            int EventId = 0;
            Assert.IsTrue(Logger.WriteWarning(Message, EventId));
        }
        #endregion

        #region Information
        /// <summary>
        ///A test for WriteInfo
        ///</summary>
        [TestMethod()]
        public void WriteInfoTest1()
        {
            string Message = "For informational purposes";
            int EventId = 0;
            Assert.IsTrue(Logger.WriteInfo(Message, EventId));
        }

        /// <summary>
        ///A test for WriteInfo
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteInfoTest()
        {
            string Message = string.Empty;
            Logger.WriteInfo(Message);
        }

        /// <summary>
        ///A test for WriteInfo
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteInfoTest2()
        {
            string Message = null;
            Logger.WriteInfo(Message);
        }

        /// <summary>
        ///A test for WriteInfo
        ///</summary>
        [TestMethod()]
        public void WriteInfoTest3()
        {
            string Message = "Information";
            Assert.IsTrue(Logger.WriteInfo(Message));
        }
        #endregion

        #region Error
        /// <summary>
        ///A test for WriteError
        ///</summary>
        [TestMethod()]
        public void WriteErrorTest1()
        {
            string ShortDescription = "problem description here";
            string StackTrace = "stack trace goes here";
            int EventId = 0;
            Assert.IsTrue(Logger.WriteError(ShortDescription, StackTrace, EventId));
        }

        /// <summary>
        ///A test for WriteError
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteErrorTest()
        {
            string ShortDescription = string.Empty;
            string StackTrace = string.Empty;
            Logger.WriteError(ShortDescription, StackTrace);
        }

        /// <summary>
        ///A test for WriteError
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteErrorTest2()
        {
            string ShortDescription = "This is an error";
            string StackTrace = string.Empty;
            Logger.WriteError(ShortDescription, StackTrace);
        }

        /// <summary>
        ///A test for WriteError
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteErrorTest3()
        {
            string ShortDescription = string.Empty;
            string StackTrace = "Some stacktrace";
            Logger.WriteError(ShortDescription, StackTrace);
        }

        /// <summary>
        ///A test for WriteError
        ///</summary>
        [TestMethod()]
        public void WriteErrorTest4()
        {
            string ShortDescription = "This is an error";
            string StackTrace = "A stack trace";
            Assert.IsTrue(Logger.WriteError(ShortDescription, StackTrace));
        }
        #endregion
    }
}
