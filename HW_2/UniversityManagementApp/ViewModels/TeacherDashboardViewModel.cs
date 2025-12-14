using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.IO;
using UniversityManagementApp.Models;
using UniversityManagementApp.Services;

namespace UniversityManagementApp.ViewModels;

public partial class TeacherDashboardViewModel : ObservableObject
{
    private readonly DataService _dataService;
    private readonly Teacher _currentTeacher;

    [ObservableProperty]
    private ObservableCollection<Subject> mySubjects = new();

    [ObservableProperty]
    private int selectedTabIndex;

    [ObservableProperty]
    private ObservableCollection<Subject>? subjects;

    [ObservableProperty]
    private Subject? selectedSubject;

    [ObservableProperty]
    private string newSubjectName = "";

    [ObservableProperty]
    private string newSubjectDescription = "";

    [ObservableProperty]
    private string selectedDescription = "Select a subject to see the description.";

    [ObservableProperty]
    private string confirmationMessage = "";

    [ObservableProperty]
    private string selectedTeacherInfo = "";

    [ObservableProperty]
    private string editingDescription = "";

    [ObservableProperty]
    private string editingTitle = "Create New Subject";

    [ObservableProperty]
    private string saveButtonText = "💾 Create Subject";

    public bool CanDeleteSubject => SelectedSubject != null;

    public ICommand SaveSubjectCommand { get; }
    public ICommand DeleteSubjectCommand { get; }
    public ICommand ClearFormCommand { get; }

    public TeacherDashboardViewModel(Teacher teacher)
    {
        _dataService = new DataService();
        _currentTeacher = teacher;
        LoadSubjects();

        SaveSubjectCommand = new RelayCommand(SaveSubject);
        DeleteSubjectCommand = new RelayCommand(DeleteSubject);
        ClearFormCommand = new RelayCommand(ClearForm);
    }

    private void LoadSubjects()
    {
        // Load every subject from the JSON file
        var allSubjects = _dataService.GetSubjects() ?? new ObservableCollection<Subject>();

        // Find the IDs of the subjects that the teacher is teaching
        var teacherSubjectIds = _currentTeacher.Subjects ?? new List<int>();

        // Filter only subjects that match teacher IDs
        Subjects = new ObservableCollection<Subject>(allSubjects.Where(s => teacherSubjectIds.Contains(s.Id)));
    }

    private void SaveSubject()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewSubjectName) || string.IsNullOrWhiteSpace(NewSubjectDescription))
            {
                ConfirmationMessage = "⚠️ Please fill in both fields.";
                return;
            }

            if (SelectedSubject != null)
            {
                // Update existing subject
                SelectedSubject.Name = NewSubjectName;
                SelectedSubject.Description = $"{NewSubjectDescription}\n\nTeacher: {_currentTeacher.Name}";
                
                // Save changes to JSON
                SaveSubjectsToJson();
                
                ConfirmationMessage = "✅ Subject updated successfully!";
            }
            else
            {
                // Create new subject
                // Create a new unique ID for the subject
                int newId = GenerateNewSubjectId();

                var newSubject = new Subject
                {
                    Id = newId,
                    Name = NewSubjectName,
                    Description = $"{NewSubjectDescription}\n\nTeacher: {_currentTeacher.Name}",
                    TeacherId = _currentTeacher.Id
                };

                MySubjects.Add(newSubject);
                Subjects?.Add(newSubject);
                _currentTeacher.Subjects.Add(newId);

                SaveTeachersToJson();
                AddSubjectToJson(newSubject);
                OnPropertyChanged(nameof(CanDeleteSubject));

                ConfirmationMessage = $"✅ Subject saved successfully!";
            }

            NewSubjectName = "";
            NewSubjectDescription = "";
        }
        catch (Exception ex)
        {
            ConfirmationMessage = $"❌ Error saving subject: {ex.Message}";
        }
    }

    private void DeleteSubject()
    {
        if (SelectedSubject == null)
        {
            ConfirmationMessage = "⚠️ Please select a subject to delete.";
            return;
        }

        Console.WriteLine($"❌ Deleting subject: {SelectedSubject.Name}");

        // Remove the subject from the teacher's subjects
        MySubjects.Remove(SelectedSubject);
        _currentTeacher.Subjects.Remove(SelectedSubject.Id);

        // Update the JSON files
        SaveTeachersToJson();
        SaveSubjectsToJson();
        SaveStudentsToJson();

        // Reload the subjects
        LoadSubjects();

        // Reset the selected subject
        SelectedSubject = null;
        OnPropertyChanged(nameof(MySubjects));
        OnPropertyChanged(nameof(Subjects));
        OnPropertyChanged(nameof(SelectedSubject));

        ConfirmationMessage = $"✅ Subject '{SelectedSubject?.Name}' deleted successfully!";
    }

    private void SaveTeachersToJson()
    {
        try
        {
            string filePath = "Data/teachers.json";

            List<Teacher> teachers = File.Exists(filePath)
                ? JsonConvert.DeserializeObject<List<Teacher>>(File.ReadAllText(filePath)) ?? new List<Teacher>()
                : new List<Teacher>();

            var teacherToUpdate = teachers.FirstOrDefault(t => t.Id == _currentTeacher.Id);
            if (teacherToUpdate != null)
            {
                teacherToUpdate.Subjects = new List<int>(_currentTeacher.Subjects);
            }
            else
            {
                teachers.Add(_currentTeacher);
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(teachers, Formatting.Indented));
            Console.WriteLine("📁 Teachers JSON updated successfully.");
        }
        catch (Exception ex)
        {
            ConfirmationMessage = $"❌ Error saving teachers JSON: {ex.Message}";
        }
    }

    private void SaveSubjectsToJson()
    {
        try
        {
            var allSubjects = _dataService.GetSubjects().ToList();
            var subjectToUpdate = allSubjects.FirstOrDefault(s => s.Id == SelectedSubject?.Id);
            
            if (subjectToUpdate != null)
            {
                subjectToUpdate.Name = SelectedSubject!.Name;
                subjectToUpdate.Description = SelectedSubject.Description;
            }

            _dataService.SaveSubjectsData(new ObservableCollection<Subject>(allSubjects));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving subjects to JSON: {ex.Message}");
        }
    }

    private void AddSubjectToJson(Subject newSubject)
    {
        try
        {
            string filePath = "Data/subjects.json";

            // Load subjects from the JSON file
            List<Subject> subjects = File.Exists(filePath)
                ? JsonConvert.DeserializeObject<List<Subject>>(File.ReadAllText(filePath)) ?? new List<Subject>()
                : new List<Subject>();

            // Add the new subject to the list
            subjects.Add(newSubject);

            // Update the JSON file
            File.WriteAllText(filePath, JsonConvert.SerializeObject(subjects, Formatting.Indented));
            Console.WriteLine($"📁 Subject '{newSubject.Name}' added to JSON successfully.");
        }
        catch (Exception ex)
        {
            ConfirmationMessage = $"❌ Error saving new subject to JSON: {ex.Message}";
        }
    }

    private void SaveStudentsToJson()
    {
        try
        {
            string filePath = "Data/students.json";

            List<Student> students = File.Exists(filePath)
                ? JsonConvert.DeserializeObject<List<Student>>(File.ReadAllText(filePath)) ?? new List<Student>()
                : new List<Student>();

            if (SelectedSubject != null)
            {
                // Remove the subject from the enrolled subjects of all students
                foreach (var student in students)
                {
                    student.EnrolledSubjects.Remove(SelectedSubject.Id);
                }
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(students, Formatting.Indented));
            Console.WriteLine($"📁 Students JSON updated after subject removal: {SelectedSubject?.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating students JSON: {ex.Message}");
        }
    }

    private int GenerateNewSubjectId()
    {
        // Read all subjects from the JSON file
        var allSubjects = _dataService.GetSubjects() ?? new ObservableCollection<Subject>();

        // Find the maximum ID
        int maxId = allSubjects.Any() ? allSubjects.Max(s => s.Id) : 0;

        // Return the next available ID
        return maxId + 1;
    }

    private void UpdateDeleteButtonState()
    {
        OnPropertyChanged();
        (DeleteSubjectCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }   

    private void ClearForm()
    {
        SelectedSubject = null;
        NewSubjectName = "";
        NewSubjectDescription = "";
        EditingTitle = "Create New Subject";
        SaveButtonText = "💾 Create Subject";
        ConfirmationMessage = "";
    }

    partial void OnSelectedSubjectChanged(Subject? value)
    {
        if (value != null)
        {
            var descriptionParts = value.Description?.Split("\n\nTeacher: ") ?? new[] { "", "" };
            SelectedDescription = descriptionParts[0];
            SelectedTeacherInfo = descriptionParts.Length > 1 ? $"Teacher: {descriptionParts[1]}" : "";
            
            // Set the editing fields with current values
            NewSubjectName = value.Name;
            NewSubjectDescription = SelectedDescription;
            EditingTitle = "Edit Subject";
            SaveButtonText = "💾 Update Subject";
        }
        else
        {
            SelectedDescription = "Select a subject to see the description.";
            SelectedTeacherInfo = "";
            EditingTitle = "Create New Subject";
            SaveButtonText = "💾 Create Subject";
        }
        
        UpdateDeleteButtonState();
        OnPropertyChanged(nameof(CanDeleteSubject));
    }
}