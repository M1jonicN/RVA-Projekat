using Client.ViewModels;
using System;
using System.Windows;

namespace Client.Views
{
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var viewModel = DataContext as DashboardViewModel;
            viewModel?.Logout();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
