using Client.ViewModels;
using Common.DbModels;
using Common.Services;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
