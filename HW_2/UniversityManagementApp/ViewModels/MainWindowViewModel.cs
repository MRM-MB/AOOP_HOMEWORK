namespace UniversityManagementApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversityManagementApp.Views;
using Avalonia.Controls;
using System;

public partial class MainWindowViewModel : ViewModelBase
{
    private LoginView? _currentLoginView;

    [ObservableProperty]
    private bool _canLogin = true;

    [ObservableProperty]
    private string _loginStatusMessage = "";

    public MainWindowViewModel()
    {
        LoginViewModel.LoginSuccessful += OnLoginSuccessful;
        LoginViewModel.DashboardClosed += OnDashboardClosed;
    }

    private void OnDashboardClosed()
    {
        CanLogin = true;
        LoginStatusMessage = "";
    }

    private void OnLoginSuccessful(string role)
    {
        CanLogin = false;
        LoginStatusMessage = $"You are already logged in as a {role}!";
        _currentLoginView = null;
    }

    [RelayCommand]
    public void Login()
    {
        if (CanLogin)  // Only check if we can login, removed _currentLoginView check
        {
            Console.WriteLine("Login button clicked.");
            _currentLoginView?.Close();  // Close any existing login window
            _currentLoginView = new LoginView();
            _currentLoginView.Closed += (s, e) => 
            {
                _currentLoginView = null;
                if (CanLogin) // Only update message if not logged in
                {
                    LoginStatusMessage = "";
                }
            };
            LoginStatusMessage = "Login form is open...";
            _currentLoginView.Show();
        }
    }
}
