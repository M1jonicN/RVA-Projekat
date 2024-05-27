using System;
using System.Collections.ObjectModel;
using Client.Services;

namespace Client.ViewModels
{
    public class UserActionsViewModel : BaseViewModel
    {
        public UserActionsViewModel()
        {
            UserActions = UserActionLoggerService.Instance.LogMessages;
            UserActionLoggerService.Instance.LogMessageAdded += OnLogMessageAdded;
        }

        public ObservableCollection<string> UserActions { get; }

        private void OnLogMessageAdded(string message)
        {
            // Already added to UserActions due to binding
        }
    }
}
