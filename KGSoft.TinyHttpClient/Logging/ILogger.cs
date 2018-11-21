using System;
using System.Collections.Generic;
using System.Text;

namespace KGSoft.TinyHttpClient.Logging
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogException(Exception ex);
    }
}
