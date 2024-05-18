namespace Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }

        public MainViewModel()
        {
            LoginViewModel = new LoginViewModel();
        }
    }
}
