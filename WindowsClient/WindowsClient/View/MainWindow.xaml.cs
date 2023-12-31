﻿using System;
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

namespace WindowsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = viewModel;
            InitializeComponent();
            ledTestButton.Click += viewModel.LedTestButton_Click;
            LedBrightness.ValueChanged += viewModel.LedBrightness_ValueChanged;
            ReConnectButton.Click += viewModel.ReConnectButton_Click;
        }

        private ViewModel viewModel = new ViewModel();

        private void Window_Closed(object sender, EventArgs e)
        {
            viewModel.Close();
        }
    }
}
