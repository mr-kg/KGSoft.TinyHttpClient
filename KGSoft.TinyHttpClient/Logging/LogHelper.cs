using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KGSoft.TinyHttpClient.Logging
{
    public class LogHelper
    {
        public static void LogMessage(string message)
        {
            if (HttpConfig.Logger != null)
                HttpConfig.Logger.LogMessage(message);
            else
                Debug.WriteLine("HttpConfig.Logger has not been set. Consider implementing ILogger.");
        }

        public static void LogException(Exception ex)
        {
            if (HttpConfig.Logger != null)
                HttpConfig.Logger.LogException(ex);
            else
                Debug.WriteLine("HttpConfig.Logger has not been set. Consider implementing ILogger.");
        }
    }
}
