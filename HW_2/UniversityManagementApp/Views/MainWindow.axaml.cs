using Avalonia.Controls;
using UniversityManagementApp.ViewModels;

namespace UniversityManagementApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}