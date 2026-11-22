# Student Grades (ASP.NET Core MVC + EF Core + PostgreSQL)

CRUD for `mst_subject` and `mst_student`, plus a grade report built on the database view
`vw_student_grades` with search by student name and an All / PASS / FAIL filter.

## How the rules are enforced

| Requirement | How |
|---|---|
| Remarks = PASS if grade >= 75, else FAIL | `remarks` is a PostgreSQL **generated column**: `CASE WHEN grade >= 75 THEN 'PASS' ELSE 'FAIL' END` |
| User cannot enter remarks | No remarks field is posted (`[BindNever]`, left out of `[Bind]`), and PostgreSQL rejects any manual value |
| View: student name, subject name, grade, remarks | Database view `vw_student_grades`, mapped to `StudentGradeView`, shown at `/StudentGrades` |
| Search by student name | Case-insensitive partial match (`ILIKE`) |
| Filter All / PASS / FAIL | Dropdown on the report page |

## Part A: Run it locally

1. Install the **.NET 8 SDK** and the EF Core CLI tool:
   ```
   dotnet tool install --global dotnet-ef --version 8.*
   ```
2. Create the project:
   ```
   dotnet new mvc -n StudentGrades
   cd StudentGrades
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.10
   dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.10
   ```
   (Visual Studio: *Create new project > ASP.NET Core Web App (Model-View-Controller)*, .NET 8, then add the two packages with NuGet.)
3. Copy in the files from this folder. Delete `Views/Home/Index.cshtml` and `Views/Home/Privacy.cshtml`.
4. Create an empty database in PostgreSQL (pgAdmin or psql):
   ```sql
   CREATE DATABASE student_grades_db;
   ```
5. Put your password in `appsettings.json` > `ConnectionStrings:DefaultConnection`.
6. Create the migration (this generates the `Migrations` folder, which you must commit):
   ```
   dotnet ef migrations add InitialCreate
   ```
7. Run:
   ```
   dotnet run
   ```
   On startup the app applies migrations (creates `mst_subject` and `mst_student`) and creates the view `vw_student_grades`.
8. Test: add 2 or 3 subjects, then add students with grades like 90, 75 and 74.99. Check that the report shows PASS / PASS / FAIL, then try the search and the filter.

## Part B: Put it on GitHub

```
git init
git add .
git commit -m "Student grades app"
git branch -M main
git remote add origin https://github.com/<you>/StudentGrades.git
git push -u origin main
```

## Part C: Free database (Neon)

1. Sign up at https://neon.com and create a project (pick the region closest to your app host).
2. Copy the connection string. It looks like
   `postgresql://user:password@ep-xxx.region.aws.neon.tech/neondb?sslmode=require`
   The app accepts this URL format directly through the `DATABASE_URL` variable.

## Part D: Free app hosting (Render)

1. Sign up at https://render.com with GitHub.
2. Click **New > Web Service** and pick your repo.
3. Language / runtime: **Docker** (it uses the `Dockerfile` in this repo). Instance type: **Free**.
4. Under **Environment Variables**, add:
   - `DATABASE_URL` = the Neon connection string from Part C
5. Click **Create Web Service**. When the build finishes, open `https://<your-app>.onrender.com`.

Notes:
- Free Render services sleep after a period of inactivity. The first request after that can take 30 to 60 seconds.
- Tables and the view are created automatically on the first start, so you don't run anything against Neon by hand.
- If you change the models later, run `dotnet ef migrations add <Name>`, commit, and push. Render redeploys automatically.

## Project structure

```
Controllers/  HomeController, SubjectsController, StudentsController, StudentGradesController
Data/         AppDbContext.cs (table/column mapping, generated column, view SQL)
Models/       Subject.cs, Student.cs, StudentGradeView.cs
Views/        Subjects/*, Students/*, StudentGrades/Index.cshtml, Shared/_Layout + partials
Program.cs    DB setup, auto-migrate + create view, PORT handling for hosting
Dockerfile    Used by Render
```
