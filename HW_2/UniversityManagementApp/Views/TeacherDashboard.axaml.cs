using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UniversityManagementApp.Models;
using UniversityManagementApp.ViewModels;

namespace UniversityManagementApp.Views;
    
public partial class TeacherDashboard : Window
{
    private readonly Models.Teacher? _teacher;

    public TeacherDashboard(Teacher teacher)
    {
        InitializeComponent();
        _teacher = teacher;
        DataContext = new TeacherDashboardViewModel(teacher);
        var nameText = this.FindControl<TextBlock>("TeacherNameText");
        if (nameText != null)
        {
            nameText.Text = $"Welcome, {teacher.Name}!";
        }
    }

    public TeacherDashboard()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
