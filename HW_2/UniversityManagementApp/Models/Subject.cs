using System.Collections.Generic;
namespace UniversityManagementApp.Models;

public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
}