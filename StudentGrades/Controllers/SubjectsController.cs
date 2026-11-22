using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGrades.Data;
using StudentGrades.Models;

namespace StudentGrades.Controllers;

public class SubjectsController : Controller
{
    private readonly AppDbContext _db;
    public SubjectsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var subjects = await _db.Subjects.OrderBy(s => s.SubjectName).ToListAsync();
        return View(subjects);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SubjectName")] Subject subject)
    {
        await CheckDuplicateName(subject);
        if (!ModelState.IsValid) return View(subject);

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();
        TempData["Message"] = $"Subject \"{subject.SubjectName}\" added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var subject = await _db.Subjects.FindAsync(id);
        return subject == null ? NotFound() : View(subject);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("SubjectKey,SubjectName")] Subject subject)
    {
        if (id != subject.SubjectKey) return NotFound();
        await CheckDuplicateName(subject);
        if (!ModelState.IsValid) return View(subject);

        var existing = await _db.Subjects.FindAsync(id);
        if (existing == null) return NotFound();

        existing.SubjectName = subject.SubjectName;
        await _db.SaveChangesAsync();
        TempData["Message"] = "Subject updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _db.Subjects.FindAsync(id);
        if (subject == null) return NotFound();
        ViewBag.StudentCount = await _db.Students.CountAsync(s => s.SubjectKey == id);
        return View(subject);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var subject = await _db.Subjects.FindAsync(id);
        if (subject == null) return NotFound();

        if (await _db.Students.AnyAsync(s => s.SubjectKey == id))
        {
            TempData["Error"] = $"Cannot delete \"{subject.SubjectName}\" because students are recorded under it. Delete or move those records first.";
            return RedirectToAction(nameof(Index));
        }

        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Subject deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckDuplicateName(Subject subject)
    {
        if (string.IsNullOrWhiteSpace(subject.SubjectName)) return;
        var name = subject.SubjectName.Trim();
        subject.SubjectName = name;
        var exists = await _db.Subjects.AnyAsync(s =>
            s.SubjectKey != subject.SubjectKey && s.SubjectName.ToLower() == name.ToLower());
        if (exists) ModelState.AddModelError(nameof(Subject.SubjectName), "This subject already exists.");
    }
}
