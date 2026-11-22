using System.ComponentModel.DataAnnotations;

namespace StudentGrades.Models;

public class StudentGradeView
{
    public int StudentKey { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; } = string.Empty;

    [Display(Name = "Grade")]
    public decimal Grade { get; set; }

    [Display(Name = "Remarks")]
    public string Remarks { get; set; } = string.Empty;
}
