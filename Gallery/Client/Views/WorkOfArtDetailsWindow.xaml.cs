using Client.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for WorkOfArtDetailsWindow.xaml
    /// </summary>
    public partial class WorkOfArtDetailsWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WorkOfArtDetailsWindow));
        public WorkOfArtDetailsWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while initializing the window: {ex.Message}");
                log.Error("Error occurred while initializing WorkOfArtDetailsWindow.", ex);
                Close(); // Close the window if initialization fails
            }
        }
    }
}
