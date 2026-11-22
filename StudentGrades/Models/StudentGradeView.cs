using System.ComponentModel.DataAnnotations;

namespace StudentGrades.Models;

// Maps to the database view: vw_student_grades (read-only, no key)
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
