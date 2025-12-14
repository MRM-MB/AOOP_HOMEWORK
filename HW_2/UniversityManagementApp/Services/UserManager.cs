using System.Linq;
using System.Collections.ObjectModel;
using UniversityManagementApp.Models;

namespace UniversityManagementApp.Services;

public class UserManager
{
    private readonly DataService _dataService;

    public UserManager(DataService dataService)
    {
        _dataService = dataService;
    }

    public Student? AuthenticateStudent(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        var students = _dataService.LoadStudentsData();
        var student = students.FirstOrDefault(s => s.Username == username);

        if (student == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, student.Password))
        {
            return null;
        }

        return student;
    }

    public Student? CreateStudentAccount(string name, string username, string password, string confirmpassword)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmpassword))
            return null;

        var students = _dataService.LoadStudentsData();
        var student = students.FirstOrDefault(s => s.Username == username);

        if (student == null)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Create new student
            student = new Student
            {
                Id = students.Count + 1,
                Username = username,
                Password = hashedPassword,
                Name = name // Modified line
            };
            students.Add(student);
            _dataService.SaveStudentsData(students);
            return student;
        }
        else
        {
            return null;
        }
    }

    public Teacher? AuthenticateTeacher(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        var teachers = _dataService.LoadTeachersData();
        var teacher = teachers.FirstOrDefault(t => t.Username == username);

        if (teacher == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, teacher.Password))
        {
            return null;
        }

        return teacher;
    }

    public Teacher? CreateTeacherAccount(string name, string username, string password, string confirmpassword)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmpassword))
            return null;

        var teachers = _dataService.LoadTeachersData();
        var teacher = teachers.FirstOrDefault(t => t.Username == username);

        if (teacher == null)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Create new teacher
            teacher = new Teacher
            {
                Id = teachers.Count + 1,
                Username = username,
                Password = hashedPassword,
                Name = name // Modified line
            };
            teachers.Add(teacher);
            _dataService.SaveTeachersData(teachers);
            return teacher;
        }
        else
        {
            return null;
        }
    }

    public ObservableCollection<Subject> GetSubjects()
    {
        var subjects = _dataService.LoadSubjectsData();
        return new ObservableCollection<Subject>(subjects);
    }
}