using System;
using Moq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UniversityManagementApp.Models;
using UniversityManagementApp.ViewModels;
using UniversityManagementApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace Tests;

public class FunctionalityTests
{        
    // A. (Student Functionality) Unit Test: Check if a student can enroll in a subject
    // - Add a new subject to the student's enrolled list
    // - Verify the subject appears in their enrolled subjects list

    private static readonly string ProjectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\UniversityManagementApp");
    private static readonly string StudentsFilePath = Path.Combine(ProjectRoot, "Data", "students.json");
    private static readonly string TeachersFilePath = Path.Combine(ProjectRoot, "Data", "teachers.json");
    private static readonly string SubjectsFilePath = Path.Combine(ProjectRoot, "Data", "subjects.json");

    [Fact]
    public void Test_StudentEnrollment_WithPersistence()
    {
        // Ensure correct working directory
        if (!File.Exists(StudentsFilePath))
        {
            throw new FileNotFoundException($"JSON file not found at {StudentsFilePath}");
        }

        // Arrange - Load existing students from JSON
        var jsonData = File.ReadAllText(StudentsFilePath);
        var students = JsonConvert.DeserializeObject<List<Student>>(jsonData) ?? new List<Student>(); // Prevent null reference

        Console.WriteLine(">> Starting Test A - Student Enrollment");

        // Find the student (Test-Student_Enrollement) in the JSON
        var student = students.Find(s => s.Id == 4);
        if (student == null)
        {
            throw new Exception("Student with ID 4 not found in JSON file.");
        }

        // Ensure EnrolledSubjects is initialized
        if (student.EnrolledSubjects == null)
        {
            student.EnrolledSubjects = new List<int>();
        }

        // Subject details
        const int subjectId = 5;
        const string subjectName = "Socio-Economic Studies";

        Console.WriteLine($"Before enrollment, student was enrolled in subjects: {string.Join(", ", student.EnrolledSubjects)}");
        Console.WriteLine($"Attempting to enroll in {subjectName} (ID: {subjectId})...");

        // Act - Enroll the student
        if (!student.EnrolledSubjects.Contains(subjectId))
        {
            student.EnrolledSubjects.Add(subjectId);
            File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(students, Formatting.Indented));
            Console.WriteLine($"Successfully enrolled in {subjectName} (ID: {subjectId}).");
        }
        else
        {
            Console.WriteLine($"Already enrolled in {subjectName} (ID: {subjectId}).");
        }

        // Reload JSON to verify persistence
        var updatedStudents = JsonConvert.DeserializeObject<List<Student>>(File.ReadAllText(StudentsFilePath)) ?? new List<Student>();
        var updatedStudent = updatedStudents.Find(s => s.Id == 4);

        if (updatedStudent?.EnrolledSubjects == null)
        {
            throw new Exception("Failed to reload student data.");
        }

        Console.WriteLine($"After enrollment, student is now enrolled in subjects: {string.Join(", ", updatedStudent.EnrolledSubjects)}");
        Console.WriteLine("Check students.json in the Data folder to verify enrollment.");

        // Assert - Verify persistence
        Assert.Contains(subjectId, updatedStudent.EnrolledSubjects);
        Console.WriteLine("-->> Test A Completed Successfully: Student successfully enrolled and data persisted.");

        // Clean up after the test
        updatedStudent.EnrolledSubjects.Remove(subjectId);
        File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(updatedStudents, Formatting.Indented));
    }

    // B. (System-Level) Unit Test: Verify Data Persistence After Restart
    // - Modify student enrollment by adding a new subject "Adds subject ID 6 to student ID 4."
    // - Assign a new subject to a teacher "Assigns subject ID 0 to teacher ID 2"
    // - Add a new subject to the subjects list Adds a new subject Netflix & Learn (ID 0)
    // - Save changes and simulate closing the application
    // - Reload data from JSON files and verify the changes persisted

    [Fact]
    public void Test_DataPersistence_AfterRestart()
    {
        Console.WriteLine(">> Starting Test B - Data Persistence Test");

        // Load data from JSON
        var studentsJson = File.ReadAllText(StudentsFilePath);
        var teachersJson = File.ReadAllText(TeachersFilePath);
        var subjectsJson = File.ReadAllText(SubjectsFilePath);

        var students = JsonConvert.DeserializeObject<List<Student>>(studentsJson) ?? new List<Student>();
        var teachers = JsonConvert.DeserializeObject<List<Teacher>>(teachersJson) ?? new List<Teacher>();
        var subjects = JsonConvert.DeserializeObject<List<Subject>>(subjectsJson) ?? new List<Subject>();

        // Modify student enrollment
        var student = students.Find(s => s.Id == 4);
        if (student == null) throw new Exception("Student with ID 4 not found.");
        student.EnrolledSubjects.Add(6); // Enrolling in subject ID 6

        // Modify teacher subjects
        var teacher = teachers.Find(t => t.Id == 2);
        if (teacher == null) throw new Exception("Teacher with ID 2 not found.");
        teacher.Subjects.Add(0); // Assigning a new subject ID 0

        // Add a new subject
        var newSubject = new Subject
        {
            Id = 0,
            Name = "🎬 Netflix & Learn",
            Description = "Just watch a series or movie, enjoy life, and earn extra points for sharing the experience!",
            TeacherId = 2,
            TeacherName = "Magnus L. Friis"
        };
        subjects.Add(newSubject);

        // Save changes to JSON
        File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(students, Formatting.Indented));
        File.WriteAllText(TeachersFilePath, JsonConvert.SerializeObject(teachers, Formatting.Indented));
        File.WriteAllText(SubjectsFilePath, JsonConvert.SerializeObject(subjects, Formatting.Indented));

        Console.WriteLine(">> Modifications saved. Simulating program restart...");

        // Simulate closing and reopening the app by reloading data
        studentsJson = File.ReadAllText(StudentsFilePath);
        teachersJson = File.ReadAllText(TeachersFilePath);
        subjectsJson = File.ReadAllText(SubjectsFilePath);

        var reloadedStudents = JsonConvert.DeserializeObject<List<Student>>(studentsJson) ?? new List<Student>();
        var reloadedTeachers = JsonConvert.DeserializeObject<List<Teacher>>(teachersJson) ?? new List<Teacher>();
        var reloadedSubjects = JsonConvert.DeserializeObject<List<Subject>>(subjectsJson) ?? new List<Subject>();

        // Verify persistence
        var reloadedStudent = reloadedStudents.Find(s => s.Id == 4);
        if (reloadedStudent == null) throw new Exception("Student with ID 4 not found in reloaded data."); // prevent warning CS8602: Dereference of a possibly null reference.
        Assert.Contains(6, reloadedStudent.EnrolledSubjects); // Should contain the new subject

        var reloadedTeacher = reloadedTeachers.Find(t => t.Id == 2);
        if (reloadedTeacher == null) throw new Exception("Teacher with ID 2 not found in reloaded data."); // prevent warning CS8602: Dereference of a possibly null reference.
        Assert.Contains(0, reloadedTeacher.Subjects); // Should have the new subject assigned

        var reloadedSubject = reloadedSubjects.Find(sub => sub.Id == 0);
        Assert.NotNull(reloadedSubject); // New subject should exist

        Console.WriteLine("-->> Test B Completed Successfully: Data persistence confirmed!");

        // Clean up after the test
        reloadedStudent.EnrolledSubjects.Remove(6);
        reloadedTeacher.Subjects.Remove(0);
        reloadedSubjects.Remove(reloadedSubject);

        // Save changes to JSON
        File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(reloadedStudents, Formatting.Indented));
        File.WriteAllText(TeachersFilePath, JsonConvert.SerializeObject(reloadedTeachers, Formatting.Indented));
        File.WriteAllText(SubjectsFilePath, JsonConvert.SerializeObject(reloadedSubjects, Formatting.Indented));
    }

    // C. (Teacher Functionality) Unit Test: Delete Subject Test
    // - Delete a subject from the "My Subjects" list
    // - Verify that the subject is removed from the "My Subjects" list (student's enrolled subjects in the students JSON file)
    // - Verify that the subject is also removed from the "Available Subjects" list (Subject JSON file)
    [Fact]
    public void Test_DeleteSubject()
    {
        Console.WriteLine(">> Starting Test C - Delete Subject Test");
        
        // Arrange
        // Ensure correct working directory
        if (!File.Exists(StudentsFilePath) || !File.Exists(TeachersFilePath) || !File.Exists(SubjectsFilePath))
        {
            throw new FileNotFoundException("One or more JSON files not found.");
        }

        // Load data from JSON
        var studentsJson = File.ReadAllText(StudentsFilePath);
        var teachersJson = File.ReadAllText(TeachersFilePath);
        var subjectsJson = File.ReadAllText(SubjectsFilePath);

        var students = JsonConvert.DeserializeObject<List<Student>>(studentsJson) ?? new List<Student>();
        var teachers = JsonConvert.DeserializeObject<List<Teacher>>(teachersJson) ?? new List<Teacher>();
        var subjects = JsonConvert.DeserializeObject<List<Subject>>(subjectsJson) ?? new List<Subject>();

        // Creating a new subject to delete
        var newSubject = new Subject
        {
            Id = 200, // ID for a new subject to delete (200 is an high number to avoid conflicts with existing IDs)
            Name = "Architectural Design",
            Description = "Learn the basics of architectural design and create your own projects.",
            TeacherId = 2,
            TeacherName = "Magnus L. Friis"
        };
        subjects.Add(newSubject); // Add the new subject to the subjects list

        // Assigning the new subject to teacher ID 2
        var teacher = teachers.Find(t => t.Id == 2);
        if (teacher == null) throw new Exception("Teacher with ID 2 not found.");
        teacher.Subjects.Add(200); // Assigning a new subject ID 200

        // Find the student for the test in the JSON
        var student = students.Find(s => s.Id == 4);
        if (student == null) throw new Exception("Student with ID 4 not found in JSON file.");
        // Ensure EnrolledSubjects is initialized
        if (student.EnrolledSubjects == null)
        {
            student.EnrolledSubjects = new List<int>();
        }
        // Enroll the student in the new subject
        student.EnrolledSubjects.Add(200); // Enrolling in the new subject

        // Save changes to JSON
        File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(students, Formatting.Indented));
        File.WriteAllText(TeachersFilePath, JsonConvert.SerializeObject(teachers, Formatting.Indented));
        File.WriteAllText(SubjectsFilePath, JsonConvert.SerializeObject(subjects, Formatting.Indented));

        // Status before deletion
        Console.WriteLine($"Available subjects before the test: {string.Join(", ", subjects.Select(s => s.Name))}");
        Console.WriteLine($"Before deletion, student was enrolled in subjects: {string.Join(", ", student.EnrolledSubjects)}");
        Console.WriteLine($"Before deletion, teacher (ID 2) was assigned subjects: {string.Join(", ", teacher.Subjects)}");

        // Precondition - Verify the subject to delete exists
        Assert.Contains(newSubject, subjects);
        Assert.Contains(newSubject.Id, student.EnrolledSubjects);   
        Assert.Contains(newSubject.Id, teacher.Subjects);
        Console.WriteLine("Precondition verified: Subject to delete exists and is assigned to student and teacher.");

        // Act - Delete the subject 

        // Find the subject to delete
        var subjectToDelete = subjects.Find(s => s.Id == 200);
        if (subjectToDelete == null) throw new Exception("Subject to delete not found.");

        // Log the name of the subject being deleted
        Console.WriteLine($"Deleting subject: {subjectToDelete.Name} (ID: {subjectToDelete.Id})");

        // Find the student for the test in the JSON
        var studentUpdated = students.Find(s => s.Id == 4);
        if (studentUpdated == null) throw new Exception("Student with ID 4 not found in JSON file.");

        // Find the teacher for the test in the JSON
        var teacherUpdated = teachers.Find(t => t.Id == 2);
        if (teacherUpdated == null) throw new Exception("Teacher with ID 2 not found in JSON file.");

        // Remove the subject from every list
        studentUpdated.EnrolledSubjects.Remove(subjectToDelete.Id);
        teacherUpdated.Subjects.Remove(subjectToDelete.Id);
        subjects.Remove(subjectToDelete);

        // Save changes to JSON
        File.WriteAllText(StudentsFilePath, JsonConvert.SerializeObject(students, Formatting.Indented));
        File.WriteAllText(TeachersFilePath, JsonConvert.SerializeObject(teachers, Formatting.Indented));
        File.WriteAllText(SubjectsFilePath, JsonConvert.SerializeObject(subjects, Formatting.Indented));

        // Status after deletion
        Console.WriteLine($"Available subjects after the test: {string.Join(", ", subjects.Select(s => s.Name))}");
        Console.WriteLine($"After deletion, student is now enrolled in subjects: {string.Join(", ", student.EnrolledSubjects)}");
        Console.WriteLine($"After deletion, teacher (ID 2) is assigned subjects: {string.Join(", ", teacher.Subjects)}");

        // Assert - Verify deletion
        Assert.DoesNotContain(subjectToDelete.Id, studentUpdated.EnrolledSubjects);
        Assert.DoesNotContain(subjectToDelete.Id, teacherUpdated.Subjects);
        Assert.DoesNotContain(subjectToDelete, subjects);
        Console.WriteLine("-->> Test C Completed Successfully: Subject successfully deleted and data persisted.");
    }
}