using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UniversityManagementApp.Models;
using UniversityManagementApp.ViewModels;

namespace UniversityManagementApp.Views;

public partial class StudentDashboard : Window
{
    private readonly Student? _student;    

    public StudentDashboard(Student student)
    {
        InitializeComponent();
        _student = student;
        DataContext = new StudentDashboardViewModel(student);
        var nameText = this.FindControl<TextBlock>("StudentNameText");
        if (nameText != null)
        {
            nameText.Text = $"Welcome, {student.Name}! 🎓";
        }
    }


    public StudentDashboard()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
