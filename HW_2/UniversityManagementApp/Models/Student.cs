using System.Collections.Generic;
namespace UniversityManagementApp.Models;

public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public List<int> EnrolledSubjects { get; set; } = new();
}
