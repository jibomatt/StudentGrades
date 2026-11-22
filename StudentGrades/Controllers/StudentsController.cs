using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentGrades.Data;
using StudentGrades.Models;

namespace StudentGrades.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _db;
    public StudentsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var students = await _db.Students
            .Include(s => s.Subject)
            .OrderBy(s => s.StudentName)
            .ToListAsync();
        return View(students);
    }

    public async Task<IActionResult> Create()
    {
        await LoadSubjects();
        return View(new Student());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StudentName,SubjectKey,Grade")] Student student)
    {
        if (!ModelState.IsValid)
        {
            await LoadSubjects(student.SubjectKey);
            return View(student);
        }

        student.StudentName = student.StudentName.Trim();
        _db.Students.Add(student);
        await _db.SaveChangesAsync(); 
        TempData["Message"] = $"Grade for {student.StudentName} saved ({student.Remarks}).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student == null) return NotFound();
        await LoadSubjects(student.SubjectKey);
        return View(student);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StudentKey,StudentName,SubjectKey,Grade")] Student student)
    {
        if (id != student.StudentKey) return NotFound();
        if (!ModelState.IsValid)
        {
            await LoadSubjects(student.SubjectKey);
            return View(student);
        }

        var existing = await _db.Students.FindAsync(id);
        if (existing == null) return NotFound();

        existing.StudentName = student.StudentName.Trim();
        existing.SubjectKey = student.SubjectKey;
        existing.Grade = student.Grade;
        await _db.SaveChangesAsync();   // Remarks is recalculated by the database
        TempData["Message"] = $"Record for {existing.StudentName} updated ({existing.Remarks}).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var student = await _db.Students.Include(s => s.Subject).FirstOrDefaultAsync(s => s.StudentKey == id);
        return student == null ? NotFound() : View(student);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student != null)
        {
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            TempData["Message"] = "Record deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSubjects(int? selected = null)
    {
        var subjects = await _db.Subjects.OrderBy(s => s.SubjectName).ToListAsync();
        ViewBag.Subjects = new SelectList(subjects, nameof(Subject.SubjectKey), nameof(Subject.SubjectName), selected);
        ViewBag.HasSubjects = subjects.Count > 0;
    }
}
