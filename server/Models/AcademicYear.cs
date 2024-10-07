using System;
using System.Collections.Generic;

namespace server.Models;

public partial class AcademicYear
{
    public int AcademicYearId { get; set; }

    public string DisplayAcademicYearName { get; set; } = null!;

    public DateOnly YearStart { get; set; }

    public DateOnly YearEnd { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BiaSoDauBai> BiaSoDauBais { get; set; } = new List<BiaSoDauBai>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
