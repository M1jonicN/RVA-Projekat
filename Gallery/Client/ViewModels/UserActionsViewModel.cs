using System;
using System.Collections.ObjectModel;
using Client.Services;
using log4net;

namespace Client.ViewModels
{
    public class UserActionsViewModel : BaseViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserActionsViewModel));

        public UserActionsViewModel()
        {
            UserActions = UserActionLoggerService.Instance.LogMessages;
            UserActionLoggerService.Instance.LogMessageAdded += OnLogMessageAdded;

            log.Info("UserActionsViewModel initialized.");
        }

        public ObservableCollection<string> UserActions { get; }

        private void OnLogMessageAdded(string message)
        {
            log.Info($"Log message added: {message}");
            // Already added to UserActions due to binding
        }
    }
}
