using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGrades.Data;

namespace StudentGrades.Controllers;

public class StudentGradesController : Controller
{
    private readonly AppDbContext _db;
    public StudentGradesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, string? remarks)
    {
        var query = _db.StudentGrades.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => EF.Functions.ILike(x.StudentName, $"%{search.Trim()}%"));
        }

        if (remarks == "PASS" || remarks == "FAIL")
        {
            query = query.Where(x => x.Remarks == remarks);
        }
        else
        {
            remarks = "ALL";
        }

        ViewBag.Search = search;
        ViewBag.Remarks = remarks;

        var results = await query
            .OrderBy(x => x.StudentName)
            .ThenBy(x => x.SubjectName)
            .ToListAsync();

        return View(results);
    }
}
