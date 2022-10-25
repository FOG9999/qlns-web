using log4net;

namespace VIETTEL.Helpers
{
    public static class AppLog
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogInfo(string sClassName, string sFunctionName, string sMessage)
        {
            log.Info(string.Format("[Controller {0}][Function {1}]: {2}\n", sClassName, sFunctionName, sMessage));
        }

        public static void LogError(string sClassName, string sFunctionName, string sMessage)
        {
            log.Error(string.Format("[Controller {0}][Function {1}]: {2}\n", sClassName, sFunctionName, sMessage));
        }
    }
}