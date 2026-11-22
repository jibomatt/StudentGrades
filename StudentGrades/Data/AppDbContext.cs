using Microsoft.EntityFrameworkCore;
using StudentGrades.Models;

namespace StudentGrades.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentGradeView> StudentGrades => Set<StudentGradeView>();

    // SQL for the database view. Run on startup with CREATE OR REPLACE (safe to re-run).
    public const string CreateViewSql = @"
CREATE OR REPLACE VIEW vw_student_grades AS
SELECT st.student_key,
       st.student_name,
       sb.subject_name,
       st.grade,
       st.remarks
FROM mst_student st
JOIN mst_subject sb ON sb.subject_key = st.subject_key;";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subject>(e =>
        {
            e.ToTable("mst_subject");
            e.HasKey(x => x.SubjectKey);
            e.Property(x => x.SubjectKey).HasColumnName("subject_key").UseIdentityByDefaultColumn();
            e.Property(x => x.SubjectName).HasColumnName("subject_name").HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.SubjectName).IsUnique();
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.ToTable("mst_student");
            e.HasKey(x => x.StudentKey);
            e.Property(x => x.StudentKey).HasColumnName("student_key").UseIdentityByDefaultColumn();
            e.Property(x => x.StudentName).HasColumnName("student_name").HasMaxLength(100).IsRequired();
            e.Property(x => x.SubjectKey).HasColumnName("subject_key").IsRequired();
            e.Property(x => x.Grade).HasColumnName("grade").HasColumnType("numeric(5,2)").IsRequired();

            // Remarks is a PostgreSQL GENERATED column, so it can never be entered manually.
            e.Property(x => x.Remarks)
             .HasColumnName("remarks")
             .HasMaxLength(4)
             .HasComputedColumnSql("CASE WHEN grade >= 75 THEN 'PASS' ELSE 'FAIL' END", stored: true);

            e.HasOne(x => x.Subject)
             .WithMany(s => s.Students)
             .HasForeignKey(x => x.SubjectKey)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Keyless entity mapped to the view (EF does not create it in migrations).
        modelBuilder.Entity<StudentGradeView>(e =>
        {
            e.HasNoKey();
            e.ToView("vw_student_grades");
            e.Property(x => x.StudentKey).HasColumnName("student_key");
            e.Property(x => x.StudentName).HasColumnName("student_name");
            e.Property(x => x.SubjectName).HasColumnName("subject_name");
            e.Property(x => x.Grade).HasColumnName("grade");
            e.Property(x => x.Remarks).HasColumnName("remarks");
        });
    }
}
