using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int GradeId { get; set; }

    public int AccountId { get; set; }

    public string Fullname { get; set; } = null!;

    public bool Status { get; set; }

    public string? Description { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;
}
