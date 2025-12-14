using UniversityManagementApp.Services;
using System;
using UniversityManagementApp.Models;
using UniversityManagementApp.Views;
using BCrypt.Net;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UniversityManagementApp.ViewModels;

public partial class LoginViewModel 
{
    private readonly DataService _dataService;
    private readonly UserManager _userManager;

    public LoginViewModel()
    {
        _dataService = new DataService();
        _userManager = new UserManager(_dataService);
        ShowErrorEvent = null!;
        ShowErrorEventCA = null!;
        ShowStudentDashboardEvent = null!;
        ShowTeacherDashboardEvent = null!;
    }

    public bool IsStudent { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Name { get; set; } // New line

    public event Action<string> ShowErrorEvent;
    public event Action<string> ShowErrorEventCA;
    public event Action<StudentDashboard> ShowStudentDashboardEvent;
    public event Action<TeacherDashboard> ShowTeacherDashboardEvent;
    public static event Action<string>? LoginSuccessful;  // Modified to include role
    public static event Action? DashboardClosed;

    public void Login()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ShowErrorEvent?.Invoke("Please enter both username and password.");
            return;
        }

        if (IsStudent)
        {
            var student = _userManager.AuthenticateStudent(Username, Password);
            if (student != null)
            {
                var dashboard = new StudentDashboard(student);
                dashboard.Closed += (s, e) => DashboardClosed?.Invoke();
                LoginSuccessful?.Invoke("Student");  // Pass role
                ShowStudentDashboardEvent?.Invoke(dashboard);
            }
            else
            {
                ShowErrorEvent?.Invoke("Invalid credentials or unexisting account.");
            }
        }
        else
        {
            var teacher = _userManager.AuthenticateTeacher(Username, Password);
            if (teacher != null)
            {
                var dashboard = new TeacherDashboard(teacher);
                dashboard.Closed += (s, e) => DashboardClosed?.Invoke();
                LoginSuccessful?.Invoke("Teacher");  // Pass role
                ShowTeacherDashboardEvent?.Invoke(dashboard);
            }
            else
            {
                ShowErrorEvent?.Invoke("Invalid credentials or existing account.");
            }
        }
    }

    public void CreateAccount()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
        {
            ShowErrorEventCA?.Invoke("Please fill all fields.");
            return;
        }

        if (Password != ConfirmPassword)
        {
            ShowErrorEventCA?.Invoke("Passwords do not match.");
            return;
        }

        if (IsStudent)
        {
            var student = _userManager.CreateStudentAccount(Name, Username, Password, ConfirmPassword); // Modified line
            if (student != null)
            {
                var dashboard = new StudentDashboard(student);
                dashboard.Closed += (s, e) => DashboardClosed?.Invoke();
                LoginSuccessful?.Invoke("Student");  // Pass role
                ShowStudentDashboardEvent?.Invoke(dashboard);
            }
            else
            {
                ShowErrorEventCA?.Invoke("Invalid credentials or unexisting account.");
            }
        }
        else
        {
            var teacher = _userManager.CreateTeacherAccount(Name, Username, Password, ConfirmPassword); // Modified line
            if (teacher != null)
            {
                var dashboard = new TeacherDashboard(teacher);
                dashboard.Closed += (s, e) => DashboardClosed?.Invoke();
                LoginSuccessful?.Invoke("Teacher");  // Pass role
                ShowTeacherDashboardEvent?.Invoke(dashboard);
            }
            else
            {
                ShowErrorEventCA?.Invoke("Invalid credentials or existing account.");
            }
        }
    }
}
