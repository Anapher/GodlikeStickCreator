using System;

namespace GodlikeStickCreator.Controls
{
    public class Logger
    {
        public void Status(string message)
        {
            NewLogMessage?.Invoke(this, new NewLogMessageEventArgs(message, LogType.Status));
        }

        public void Warn(string message)
        {
            NewLogMessage?.Invoke(this, new NewLogMessageEventArgs(message, LogType.Warning));
        }

        public void Error(string message)
        {
            NewLogMessage?.Invoke(this, new NewLogMessageEventArgs(message, LogType.Error));
        }

        public void Success(string message)
        {
            NewLogMessage?.Invoke(this, new NewLogMessageEventArgs(message, LogType.Success));
        }

        public event EventHandler<NewLogMessageEventArgs> NewLogMessage;
    }

    public enum LogType
    {
        Status,
        Warning,
        Error,
        Success
    }

    public class NewLogMessageEventArgs : EventArgs
    {
        public NewLogMessageEventArgs(string content, LogType logType)
        {
            Content = content;
            LogType = logType;
        }

        public LogType LogType { get; }
        public string Content { get; }
    }
}