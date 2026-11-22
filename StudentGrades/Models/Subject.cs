using System.ComponentModel.DataAnnotations;

namespace StudentGrades.Models;

// Table: mst_subject
public class Subject
{
    [Key]
    [Display(Name = "Subject Key")]
    public int SubjectKey { get; set; }

    [Required(ErrorMessage = "Subject name is required.")]
    [StringLength(100)]
    [Display(Name = "Subject Name")]
    public string SubjectName { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
