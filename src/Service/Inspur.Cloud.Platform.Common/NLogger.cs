using System;
using System.Collections.Generic;
using System.Text;
using log4net.Repository;
using log4net;
using log4net.Core;

namespace Inspur.ECP.Rtf.Common
{
    public static class NLogger
    {

        public static void Debug(object message)
        {
            var log = LogManager.GetLogger("NetCorelogger", typeof(NLogger));
            log.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            var log = LogManager.GetLogger("NetCorelogger", typeof(NLogger));
            log.Debug(message, exception);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            var log = LogManager.GetLogger("NetCorelogger", typeof(NLogger));
            log.DebugFormat(format, args);
        }


        public static void Error(object message)
        {
            var logger = LogManager.GetLogger("NetCorelogger", "errorLogger");
            logger.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            var logger = LogManager.GetLogger("NetCorelogger", "errorLogger");
            logger.Error(message, exception);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            var logger = LogManager.GetLogger("NetCorelogger", "errorLogger");
            logger.ErrorFormat(format, args);
        }

        public static void Info(object message)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.Info(message, exception);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.InfoFormat(format, args);
        }

        public static void Warn(object message)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.Warn(message, exception);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            var infoLogger = LogManager.GetLogger("NetCorelogger", "infoLogger");
            infoLogger.WarnFormat(format, args);
        }
    }
}
