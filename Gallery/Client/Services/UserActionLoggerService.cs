using System;
using System.Collections.ObjectModel;

namespace Client.Services
{
    public class UserActionLoggerService : ILogInterface
    {
        private static UserActionLoggerService _instance;
        private readonly ObservableCollection<string> _logMessages;

        public event Action<string> LogMessageAdded;

        private UserActionLoggerService()
        {
            _logMessages = new ObservableCollection<string>();
        }

        public static UserActionLoggerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserActionLoggerService();
                }
                return _instance;
            }
        }

        public ObservableCollection<string> LogMessages => _logMessages;

        public void Log(string username, string message)
        {
            var logMessage = $"{username}: {message}";
            _logMessages.Add(logMessage);
            LogMessageAdded?.Invoke(logMessage);
        }
    }
}
