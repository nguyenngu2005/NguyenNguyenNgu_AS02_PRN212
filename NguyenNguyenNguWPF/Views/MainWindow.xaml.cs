﻿
using NguyenNguyenNguWPF.ViewModels;
using System.Windows;

namespace NguyenNguyenNguWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}