using Client.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Client.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text == "username")
            {
                errorMessage.Text = "";
                tbUsername.Text = "";
                userBorder.BorderBrush = Brushes.Red;
                userBorder.BorderThickness = new Thickness(0);
            }
        }

        private void tbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text.Trim() == "")
            {
                errorMessage.Text = "Invalid username or password";
                userBorder.BorderBrush = Brushes.Red;
                userBorder.BorderThickness = new Thickness(1);
                tbUsername.Text = "username";

            }
        }

        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password == "" || lblPassword.Visibility == Visibility.Visible)
            {
                lblPassword.Visibility = Visibility.Hidden;
                errorMessage.Text = "";
                passBorder.BorderBrush = Brushes.Red;
                passBorder.BorderThickness = new Thickness(0);
            }
        }

        private void pbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password == "")
            {
                errorMessage.Text = "Invalid username or password";
                passBorder.BorderBrush = Brushes.Red;
                passBorder.BorderThickness = new Thickness(1);
                lblPassword.Visibility = Visibility.Visible;
            }
        }

        private void lblPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lblPassword.Visibility = Visibility.Hidden;
            pbPassword.Focus();

        }
    }
}
