using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using UniversityManagementApp.Models;
using UniversityManagementApp.Services;
using Newtonsoft.Json;
using System.IO;

namespace UniversityManagementApp.ViewModels;

public partial class StudentDashboardViewModel : ObservableObject
{
    private readonly DataService _dataService;
    private readonly Student _currentStudent;

    [ObservableProperty]
    private Subject? selectedSubject;

    [ObservableProperty]
    private ObservableCollection<Subject> availableSubjects = new();

    [ObservableProperty]
    private ObservableCollection<Subject> enrolledSubjects = new();

    [ObservableProperty]
    private string selectedDescription = "Select a subject to see the description.";

    [ObservableProperty]
    private string selectedTeacherInfo = "";

    [ObservableProperty]
    private int selectedTabIndex;

    [ObservableProperty]
    private string confirmationMessage = "";

    public ICommand EnrollCommand { get; }
    public ICommand UnenrollCommand { get; }

    public bool CanEnroll => SelectedSubject != null && !EnrolledSubjects.Contains(SelectedSubject);
    public bool CanUnenroll => SelectedSubject != null && EnrolledSubjects.Contains(SelectedSubject);

    public StudentDashboardViewModel(Student student)
    {
        _dataService = new DataService();
        _currentStudent = student;

        LoadSubjects();
        EnrollCommand = new RelayCommand(EnrollInSubject, () => CanEnroll);
        UnenrollCommand = new RelayCommand(UnenrollFromSubject, () => CanUnenroll);
    }

    private void LoadSubjects()
    {
        var allSubjects = _dataService.GetSubjects() ?? new ObservableCollection<Subject>();

        // Process each subject to add teacher name
        foreach (var subject in allSubjects)
        {
            var descriptionParts = subject.Description?.Split("\n\nTeacher: ") ?? new[] { "", "" };
            if (descriptionParts.Length > 1)
            {
                subject.TeacherName = $"Teacher: {descriptionParts[1]}";
            }
        }

        var enrolledIds = _currentStudent.EnrolledSubjects ?? new List<int>();
        EnrolledSubjects = new ObservableCollection<Subject>(allSubjects.Where(s => enrolledIds.Contains(s.Id)));
        AvailableSubjects = new ObservableCollection<Subject>(allSubjects.Where(s => !enrolledIds.Contains(s.Id)));
    }

    partial void OnSelectedSubjectChanged(Subject? value)
    {
        if (value != null)
        {
            var descriptionParts = value.Description?.Split("\n\nTeacher: ") ?? new[] { "", "" };
            SelectedDescription = descriptionParts[0];
            SelectedTeacherInfo = descriptionParts.Length > 1 ? $"Teacher: {descriptionParts[1]}" : "";
        }
        else
        {
            SelectedDescription = "Select a subject to see the description.";
            SelectedTeacherInfo = "";
        }
        OnPropertyChanged(nameof(CanEnroll));
        OnPropertyChanged(nameof(CanUnenroll));
        (EnrollCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (UnenrollCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        SelectedSubject = null;
        SelectedDescription = "Select a subject to see the description.";
        ConfirmationMessage = "";
    }

    private void EnrollInSubject()
    {
        try
        {
            if (SelectedSubject == null || EnrolledSubjects.Contains(SelectedSubject)) return;
            
            // Create a copy of the object to avoid reference issues
            var subjectToEnroll = new Subject
            {
                Id = SelectedSubject.Id,
                Name = SelectedSubject.Name,
                Description = SelectedSubject.Description,
                TeacherName = SelectedSubject.TeacherName  // Copy teacher name
            };

            EnrolledSubjects.Add(subjectToEnroll);
            AvailableSubjects.Remove(SelectedSubject);
            _currentStudent.EnrolledSubjects.Add(subjectToEnroll.Id);

            // Save to JSON
            SaveStudentsToJson();

            ConfirmationMessage = "✅ Enrolled successfully!";

            // Notify the UI
            OnPropertyChanged(nameof(CanEnroll));
            OnPropertyChanged(nameof(CanUnenroll));
            OnPropertyChanged(nameof(AvailableSubjects));
            OnPropertyChanged(nameof(EnrolledSubjects));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Enrollment error: {ex.Message}");
        }
    }

    private void UnenrollFromSubject()
    {
        try
        {
            if (SelectedSubject == null || !EnrolledSubjects.Contains(SelectedSubject)) return;

            // Create a copy of the object to avoid reference issues
            var subjectToUnenroll = new Subject
            {
                Id = SelectedSubject.Id,
                Name = SelectedSubject.Name,
                Description = SelectedSubject.Description,
                TeacherName = SelectedSubject.TeacherName  // Add this line to copy the teacher name
            };

            EnrolledSubjects.Remove(SelectedSubject);
            AvailableSubjects.Add(subjectToUnenroll);
            _currentStudent.EnrolledSubjects.Remove(subjectToUnenroll.Id);

            // Save to JSON
            SaveStudentsToJson();

            ConfirmationMessage = "✅ Unenrolled successfully!";

            // Notify the UI
            OnPropertyChanged(nameof(CanEnroll));
            OnPropertyChanged(nameof(CanUnenroll));
            OnPropertyChanged(nameof(AvailableSubjects));
            OnPropertyChanged(nameof(EnrolledSubjects));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Enenrollment error: {ex.Message}");
        }
    }

    
    private void SaveStudentsToJson()
    {
        try
        {
            string filePath = "Data/students.json";

            List<Student> students = new List<Student>();

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                students = JsonConvert.DeserializeObject<List<Student>>(json) ?? new List<Student>();
            }

            var studentToUpdate = students.FirstOrDefault(s => s.Id == _currentStudent.Id);

            if (studentToUpdate != null)
            {
                // Convert the list to a new one to avoid reference issues
                studentToUpdate.EnrolledSubjects = _currentStudent.EnrolledSubjects.Select(s => s).ToList();
            }
            else
            {
                students.Add(_currentStudent);
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(students, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Saving JSON error: {ex.Message}");
        }
    }
}
