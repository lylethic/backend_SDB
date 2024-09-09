using System;
using System.Collections.Generic;

namespace server.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public int AcademicYearId { get; set; }

    public string SubjectName { get; set; } = null!;

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ICollection<ChiTietSoDauBai> ChiTietSoDauBais { get; set; } = new List<ChiTietSoDauBai>();

    public virtual ICollection<SubjectAssignment> SubjectAssignments { get; set; } = new List<SubjectAssignment>();
}
