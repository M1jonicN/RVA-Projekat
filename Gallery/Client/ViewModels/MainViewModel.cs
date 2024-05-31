using log4net;

namespace Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        public LoginViewModel LoginViewModel { get; set; }

        public MainViewModel()
        {
            LoginViewModel = new LoginViewModel();
            log.Info("MainViewModel initialized.");
        }
    }
}
