using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using UniversityManagementApp.ViewModels;

namespace UniversityManagementApp.Views;

public partial class LoginView : Window
{
    private LoginViewModel _viewModel;

    public LoginView()
    {
        InitializeComponent();
        _viewModel = new LoginViewModel();

        // Binding for events
        _viewModel.ShowErrorEvent += ShowError;
        _viewModel.ShowErrorEventCA += ShowErrorCA;
        _viewModel.ShowStudentDashboardEvent += ShowStudentDashboard;
        _viewModel.ShowTeacherDashboardEvent += ShowTeacherDashboard;

        this.DataContext = _viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("UserTypeComboBox");
        var usernameBox = this.FindControl<TextBox>("UsernameTextBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");

        _viewModel.IsStudent = comboBox?.SelectedIndex == 0;
        _viewModel.Username = usernameBox?.Text ?? string.Empty;
        _viewModel.Password = passwordBox?.Text ?? string.Empty;

        _viewModel.Login();
    }

    private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("UserTypeComboBoxCA");
        var nameBox = this.FindControl<TextBox>("NameTextBoxCA"); // New line
        var usernameBox = this.FindControl<TextBox>("UsernameTextBoxCA");
        var passwordBox = this.FindControl<TextBox>("PasswordBoxCA");
        var confirmPasswordBox = this.FindControl<TextBox>("ConfirmPasswordBoxCA");

        _viewModel.IsStudent = comboBox?.SelectedIndex == 0;
        _viewModel.Name = nameBox?.Text ?? string.Empty; // New line
        _viewModel.Username = usernameBox?.Text ?? string.Empty;
        _viewModel.Password = passwordBox?.Text ?? string.Empty;
        _viewModel.ConfirmPassword = confirmPasswordBox?.Text ?? string.Empty;

        _viewModel.CreateAccount();
    }

    private void ShowError(string message)
    {
        var errorTextBlock = this.FindControl<TextBlock>("ErrorMessage");
        if (errorTextBlock != null)
        {
            errorTextBlock.Text = message;
            errorTextBlock.IsVisible = true;
        }
    }

    private void ShowErrorCA(string message)
    {
        var errorTextBlock = this.FindControl<TextBlock>("ErrorMessageCA");
        if (errorTextBlock != null)
        {
            errorTextBlock.Text = message;
            errorTextBlock.IsVisible = true;
        }
    }

    private void ShowStudentDashboard(StudentDashboard dashboard)
    {
        dashboard.Show();
        Close();
    }

    private void ShowTeacherDashboard(TeacherDashboard dashboard)
    {
        dashboard.Show();
        Close();
    }
}