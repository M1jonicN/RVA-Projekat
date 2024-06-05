using Client.ViewModels;
using log4net;
using log4net.Config;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            log.Info("MainWindow initialized.");
            DataContext = new MainViewModel();
        }
    }
}
