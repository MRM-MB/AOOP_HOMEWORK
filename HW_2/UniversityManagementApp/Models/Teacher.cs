using System.Collections.Generic;
namespace UniversityManagementApp.Models;

public class Teacher
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public List<int> Subjects { get; set; } = new();
}