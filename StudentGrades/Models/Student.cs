using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentGrades.Models;

// Table: mst_student
public class Student
{
    [Key]
    [Display(Name = "Student Key")]
    public int StudentKey { get; set; }

    [Required(ErrorMessage = "Student name is required.")]
    [StringLength(100)]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a subject.")]
    [Display(Name = "Subject")]
    public int? SubjectKey { get; set; }

    [Required(ErrorMessage = "Grade is required.")]
    [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100.")]
    [Display(Name = "Grade")]
    public decimal? Grade { get; set; }

    // Computed by PostgreSQL (generated column): grade >= 75 -> PASS, else FAIL.
    // [BindNever] stops the user from posting a value for it.
    [BindNever]
    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    [ValidateNever]
    public Subject? Subject { get; set; }
}
