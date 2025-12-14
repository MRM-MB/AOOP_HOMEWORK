using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using UniversityManagementApp.Models;

namespace UniversityManagementApp.Services;

public class DataService
{
    private const string StudentsFilePath = "Data/students.json";
    private const string TeachersFilePath = "Data/teachers.json";
    private const string SubjectsFilePath = "Data/subjects.json";

    public List<Student> Students { get; private set; } = [];
    public List<Teacher> Teachers { get; private set; } = [];
    public List<Subject> Subjects { get; private set; } = [];


    public List<Student> LoadStudentsData()
    {
        if (!File.Exists(StudentsFilePath))
        {
            Console.WriteLine("File not found: " + StudentsFilePath);
            return new List<Student>();
        }

        string json = File.ReadAllText(StudentsFilePath);
        return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
    }

    public List<Teacher> LoadTeachersData()
    {
        if (!File.Exists(TeachersFilePath))
        {
            Console.WriteLine("File not found: " + TeachersFilePath);
            return new List<Teacher>();
        }

        string json = File.ReadAllText(TeachersFilePath);
        return JsonSerializer.Deserialize<List<Teacher>>(json) ?? new List<Teacher>();
    }

    public List<Subject> LoadSubjectsData()
    {
        if (!File.Exists(SubjectsFilePath))
        {
            Console.WriteLine("File not found: " + SubjectsFilePath);
            return new List<Subject>();
        }

        string json = File.ReadAllText(SubjectsFilePath);
        return JsonSerializer.Deserialize<List<Subject>>(json) ?? new List<Subject>();
    }

    public void SaveStudentsData(List<Student> students)
    {
        string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(StudentsFilePath, json);
    }

    public void SaveTeachersData(List<Teacher> teachers)
    {
        string json = JsonSerializer.Serialize(teachers, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TeachersFilePath, json);
    }

    public void SaveSubjectsData(ObservableCollection<Subject> subjects)
    {
        string json = JsonSerializer.Serialize(subjects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SubjectsFilePath, json);
    }
    
    public ObservableCollection<Subject> GetSubjects()
    {
        try
        {
            if (!File.Exists(SubjectsFilePath))
            {
                Console.WriteLine($"⚠ File not found: {SubjectsFilePath}");
                return new ObservableCollection<Subject>();
            }

            string json = File.ReadAllText(SubjectsFilePath);
            var subjects = JsonSerializer.Deserialize<List<Subject>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (subjects == null)
            {
                Console.WriteLine("⚠ Failed to deserialize subjects.json");
                return new ObservableCollection<Subject>();
            }

            return new ObservableCollection<Subject>(subjects);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading the subject: {ex.Message}");
            return new ObservableCollection<Subject>();
        }
    }

}
